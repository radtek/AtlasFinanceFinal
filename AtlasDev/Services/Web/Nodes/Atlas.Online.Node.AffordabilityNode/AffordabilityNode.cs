using Atlas.Online.Node.Core;
using MassTransit;
using Ninject;
using Ninject.Extensions.Logging;
using System;
using Atlas.Common.Extensions;
using DevExpress.Xpo;
using Atlas.Online.Data.Models.Definitions;
using System.Linq;
using Atlas.Enumerators;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Atlas.Common.Utils;
using Atlas.Domain.DTO;
using Atlas.Domain.Structures;
using Atlas.Online.Node.AffordabilityNode.EasyNetQ;
using Atlas.RabbitMQ.Messages.Online;
using Atlas.RabbitMQ.Messages.Notification;
using Magnum;
using Atlas.Online.Node.AffordabilityNode.OrchestrationService;

namespace Atlas.Online.Node.AffordabilityNode
{
  public sealed class AffordabilityServiceNode : AbstractService<AffordabilityServiceNode, AffordabilityMessage, AccountVerificationMessage>
  {
    private IKernel _kernel;
    private static object obj = new object();
    public AffordabilityServiceNode(ILogger ilogger, IKernel kernel)
      : base(ilogger)
    {
      this._kernel = kernel;
    }

    public override void Start()
    {
      base.Start();

      var atlasOnlineServiceBus = _kernel.Get<AtlasOnlineServiceBus>();

      // Setup subscription handler for specific message type
      atlasOnlineServiceBus.GetServiceBus().Subscribe<AffordabilityMessage>("queue_AffordabilityMessage", Handle);

      ServiceLocator.SetServiceLocator(_kernel);
    }

    public override void Stop()
    {
      base.Stop();
    }

    public override void Handle(AffordabilityMessage message)
    {
      using (_kernel.BeginBlock())
      {
        AffordabilityOption option = null;
        Application application = null;

        new AccountService.AccountServerClient("AccountServer.NET").Using((cli) =>
        {
          using (var uow = new UnitOfWork())
          {
            List<ACC_AffordabilityDTO> affordabilityCollection = new List<ACC_AffordabilityDTO>();

            application = new XPQuery<Application>(uow).FirstOrDefault(a => a.AccountId == message.AccountId);

            ACC_AffordabilityDTO affordability = new ACC_AffordabilityDTO();
            affordability.AffordabilityCategory = new ACC_AffordabilityCategoryDTO()
            {
              Description = Account.AffordabilityCategoryType.Income.ToStringEnum(),
              AffordabilityCategoryId = Account.AffordabilityCategoryType.Income.ToInt(),
              AffordabilityCategoryType = Account.AffordabilityCategoryType.Income
            };
            affordability.Amount = application.TotalIncome;

            affordabilityCollection.Add(affordability);

            affordability = new ACC_AffordabilityDTO();
            affordability.AffordabilityCategory = new ACC_AffordabilityCategoryDTO()
            {
              Description = Account.AffordabilityCategoryType.Expense.ToStringEnum(),
              AffordabilityCategoryId = Account.AffordabilityCategoryType.Expense.ToInt(),
              AffordabilityCategoryType = Account.AffordabilityCategoryType.Expense
            };
            affordability.Amount = application.TotalExpenses;

            affordabilityCollection.Add(affordability);

            cli.SaveAffordabilityItem(message.AccountId, affordabilityCollection);


            string error = string.Empty;
            int result = -1;
            option = cli.GetAffordabilityOption(message.AccountId, out error, out result);

            if (option == null)
            {
              _logger.Warn(string.Format("No option was calculated for application [{0}]", message.ApplicationId));

              if (cli.UpdateAccountStatus(message.AccountId, Account.AccountStatus.Declined, Account.AccountStatusReason.Affordability, Account.AccountStatusSubReason.AffordabilityNoOptions))
              {
                Application.UpdateStatus(uow, application.ApplicationId, Account.AccountStatus.Declined);
              }

              // Set the new source of the message
              message.Source = NodeType.Nodes.Affordability;
              message.MessageId = message.MessageId;

              // Set Destination of message
              message.Destination = NodeType.Nodes.Sink;

              // Add route history.
              var routeHistoryId = this.AddRouteHistory(message);

              uow.CommitChanges();

              Dictionary<string, string> dict = new Dictionary<string, string>();

              dict.Add("{Name}", application.Client.Firstname);
              dict.Add("{Surname}", application.Client.Surname);

              string compiled = string.Empty;

              new OrchestrationServiceClient("OrchestrationService.NET").Using(client =>
              {
                compiled = client.GetCompiledTemplate(Notification.NotificationTemplate.Declined_Affordability, dict);
              });

              ServiceLocator.Get<AtlasOnlineServiceBus>()
                  .GetServiceBus().Publish<EmailNotifyMessage>(new EmailNotifyMessage(CombGuid.Generate())
                  {
                    ActionDate = DateTime.Now,
                    Body = compiled,
                    CreatedAt = DateTime.Now,
                    From = "noreply@atlasonline.co.za",
                    IsHTML = true,
                    Priority = Notification.NotificationPriority.High,
                    Subject = string.Format("Your Atlas Online application [{0}] has been Declined", application.AccountNo),
                    To = new XPQuery<UserProfile>(uow).FirstOrDefault(p => p.UserId == application.Client.UserId).Email
                  });
            }
            else
            {
              if (option.AffordabilityOptionId == 0)
              {
                _logger.Fatal(string.Format("Option did not have accompanying affordabilityoptionId for application [{0}]", message.ApplicationId));
              }
              else
              {
                // TODO: CHECK_BYPASS Override check bypass here

                _logger.Info(string.Format("Option for Amount [{0}], Capital [{1}], Instalment [{2}] over period [{3}] for application [{4}]", option.Amount.ToString("C"),
                  option.CapitalAmount.ToString("C"), option.Instalment.Value.ToString("C"), option.Period, message.ApplicationId));

                if (option.AffordabilityOptionType == Account.AffordabilityOptionType.RequestedOption)
                  cli.AcceptAffordabilityOption(message.AccountId, option.AffordabilityOptionId, out error, out result);

                Affordability affordabilityOption = null;
                if (application.Affordability == null)
                {
                  affordabilityOption = new Affordability(uow);
                  affordabilityOption.Application = application;
                  affordabilityOption.CapitalAmount = option.CapitalAmount;
                  affordabilityOption.Amount = option.Amount;
                  affordabilityOption.Instalment = option.Instalment;
                  affordabilityOption.OptionType = option.AffordabilityOptionType;
                  affordabilityOption.RepaymentAmount = (decimal)option.TotalPayBack;
                  affordabilityOption.TotalFees = option.TotalFees;
                  affordabilityOption.OptionId = option.AffordabilityOptionId;
                  affordabilityOption.Save();
                  uow.CommitChanges();
                }

                application.Affordability = affordabilityOption;
                application.Save();

                // Set the new source of the message
                message.Source = NodeType.Nodes.Affordability;
                message.MessageId = message.MessageId;

                // Set Destination of message
                message.Destination = NodeType.Nodes.AccountVerification;

                // Add route history.
                var routeHistoryId = this.AddRouteHistory(message);

                uow.CommitChanges();
                // Route to next node
                this.Publish(new AccountVerificationMessage(message.CorrelationId)
                {
                  AccountId = message.AccountId,
                  ClientId = message.ClientId,
                  CreatedAt = DateTime.Now,
                  Destination = NodeType.Nodes.AccountVerification,
                  RouteHistoryId = routeHistoryId,
                  MessageId = message.MessageId,
                  Source = NodeType.Nodes.Affordability
                });
              }
            }
          }
        });
      }
    }

    public override void Publish(AccountVerificationMessage message)
    {
      ServiceLocator.Get<AtlasOnlineServiceBus>()
                  .GetServiceBus().Publish(message);
    }

    public override long AddRouteHistory(dynamic message)
    {
      return base.AddRouteHistory((AffordabilityMessage)message);
    }
  }
}
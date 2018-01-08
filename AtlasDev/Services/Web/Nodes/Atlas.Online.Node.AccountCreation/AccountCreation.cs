using Atlas.Online.Node.Core;
using MassTransit;
using Ninject;
using Ninject.Extensions.Logging;
using System;
using Atlas.Online.Data.Models.Definitions;
using Atlas.Enumerators;
using DevExpress.Xpo;
using System.Linq;
using Atlas.Common.Extensions;
using Atlas.Domain.DTO;
using Atlas.Online.Node.AccountCreation.AccountServer;
using Atlas.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atlas.RabbitMQ.Messages.Notification;
using Magnum;
using Atlas.RabbitMQ.Messages.Online;
using Atlas.Domain.Structures;
using Atlas.Online.Node.AccountCreation.EasyNetQ;
using Atlas.Online.Node.AccountCreation.OrchestrationService;
using AccountInfo = Atlas.Online.Node.AccountCreation.AccountServer.AccountInfo;
using ACC_PolicyDTO = Atlas.Online.Node.AccountCreation.OrchestrationService.ACC_PolicyDTO;


namespace Atlas.Online.Node.AccountCreation
{
  public sealed class AccountCreationServiceNode : AbstractService<AccountCreationServiceNode, AccountCreationMessage, FraudPreventionMessage>
  {
    private IKernel _kernel;
		private static Object obj = new Object();
    public AccountCreationServiceNode(ILogger ilogger, IKernel kernel)
      : base(ilogger)
    {
      this._kernel = kernel;
    }

    public override void Start()
    {
      base.Start();

      var atlasOnlineServiceBus = _kernel.Get<AtlasOnlineServiceBus>();

      // Setup subscription handler for specific message type
      atlasOnlineServiceBus.GetServiceBus().Subscribe<AccountCreationMessage>("queue_AccountCreationMessage", Handle);

      ServiceLocator.SetServiceLocator(_kernel);
        }

    public override void Stop()
    {
      base.Stop();
    }

    public override void Publish(FraudPreventionMessage message)
    {
      ServiceLocator.Get<AtlasOnlineServiceBus>()
                  .GetServiceBus().Publish(message);
    }

    public override void Handle(AccountCreationMessage message)
    {
      try
      {
        using (_kernel.BeginBlock())
        {
          long? routeHistoryId = null;
          AccountInfo account = null;
          long? personId = null;
          List<ACC_PolicyDTO> policyCollection = null;
          bool hasPolicy = false;
          bool technicalError = false;
          string techErrorMsg = string.Empty;

          using (var uow = new UnitOfWork())
          {
            var client = new XPQuery<Client>(uow).FirstOrDefault(c => c.ClientId == message.ClientId);

            if (client == null)
              throw new Exception(string.Format("Handle() - Client [{0}] does not exist", message.ClientId));

            var application = client.Applications.OfType<Application>().FirstOrDefault(p => p.IsCurrent);

            if (application == null)
              throw new Exception(string.Format("Handle() - Active account for client {0} was not found", client.ClientId));

            _logger.Info(string.Format(":: Application [{0}] Client: [{1}]", application.ApplicationId, message.ClientId));

            personId = client.PersonId;

            new AccountServerClient("AccountServer.NET").Using((cli) =>
            {
              account = cli.GetAccount(((long)client.PersonId));

              if (account == null)
              {
                account = CreateAccount(cli, application, AccountPeriodFrequency.Daily, client);
                cli.WorkflowStart(account.AccountId);
                UpdateAccount(uow, application, account);

                _logger.Info(string.Format(":: Account Created [{0}] Client [{1}] Linked To [{2}]", account.AccountId, client.ClientId, application.ApplicationId));
              }
              else
              {
                _logger.Info(string.Format(":: Account Status [{0}] Client [{1}]", account.Status.ToStringEnum(), client.ClientId));

                if (account.Status == AccountAccountStatus.Inactive ||
                   account.Status == AccountAccountStatus.Pending ||
                   account.Status == AccountAccountStatus.Approved ||
                   account.Status == AccountAccountStatus.Open ||
                   account.Status == AccountAccountStatus.PreApproved ||
                   account.Status == AccountAccountStatus.Review)
                {
                  if (application.AccountId != account.AccountId)
                  {
                    _logger.Fatal(string.Format(":: Application [{0}] mismatch with recent Acccount [{1}] Client [{2}]", application.AccountId, account.AccountId, client.ClientId));
                    SinkRoute(message);
                    technicalError = true;
                    techErrorMsg = string.Format(":: Active application [{0}] does not match most recent account [{1}] for client [{2}]", application.AccountId, account.AccountId, client.ClientId);
                  }
                }
                else
                {
                  account = CreateAccount(cli, application, AccountPeriodFrequency.Daily, client);
                  cli.WorkflowStart(account.AccountId);

                  if (application.AccountId == null)
                  {
                    _logger.Info(string.Format(":: Account Created [{0}] Client [{1}] linke to Application [{2}]", account.AccountId, client.ClientId, application.ApplicationId));

                    UpdateAccount(uow, application, account);
                  }
                  else
                  {
                    techErrorMsg = string.Format(":: Link Failure Account {0} to Application {1} Application linked to Account {2}", account.AccountId, application.ApplicationId, application.AccountId);
                    technicalError = true;
                  }
                }
              }

              if (!technicalError)
              {
                List<AccountPolicy> companyPolicyCollection = null;

                new OrchestrationServiceClient("OrchestrationService.NET").Using(
                (orch) =>
                {
                  companyPolicyCollection = orch.CompanyPolicies((long)account.AccountId);
                  policyCollection = orch.AccountPolicies(((long)personId));
                });

                if (policyCollection != null && policyCollection.Count > 0 || companyPolicyCollection != null && companyPolicyCollection.Count > 0)
                  hasPolicy = true;
              }

              if (hasPolicy && !technicalError)
              {
                cli.UpdateAccountStatus(account.AccountId, AccountAccountStatus.Declined, AccountAccountStatusReason.CompanyPolicy, null);
                Application.UpdateStatus(uow, application.ApplicationId, Account.AccountStatus.Declined);
                uow.CommitChanges();
                _logger.Fatal(string.Format(":: Account Creation halted {0} due to policy(s): {1}", client.ClientId, string.Join(",", policyCollection.Select(o => o.Description).ToArray())));

                Dictionary<string, string> dict = new Dictionary<string, string>();

                dict.Add("{Name}", application.Client.Firstname);
                dict.Add("{Surname}", application.Client.Surname);
                dict.Add("{AccountNo}", account.AccountNo);

                string compiled = string.Empty;

                new OrchestrationServiceClient("OrchestrationService.NET").Using(pri =>
                {
                  compiled = pri.GetCompiledTemplate(NotificationNotificationTemplate.Declined_CompanyPolicy, dict);
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

                SinkRoute(message);
              }
              else if (!hasPolicy && technicalError)
              {
                ServiceLocator.Get<AtlasOnlineServiceBus>()
                  .GetServiceBus().Publish(new EmailNotifyMessage(CombGuid.Generate())
                {
                  CreatedAt = DateTime.Now,
                  ActionDate = DateTime.Now,
                  From = "fabian@atcorp.co.za",
                  Body = techErrorMsg,
                  Priority = Notification.NotificationPriority.High,
                  Subject = "Technical Error",
                  To = "lee@atcorp.co.za;fabian@atcorp.co.za",
                  IsHTML = false
                });

                _logger.Fatal(techErrorMsg);
                cli.UpdateAccountStatus(account.AccountId, AccountAccountStatus.Technical_Error, null, null);

                Application.UpdateStatus(uow, application.ApplicationId, Account.AccountStatus.Technical_Error);

                SinkRoute(message);

                uow.CommitChanges();
              }
              else if (!hasPolicy && !technicalError)
              {
                // Set the new source of the message
                message.Source = NodeType.Nodes.AccountCreation;
                message.MessageId = message.MessageId;

                // Set Destination of message
                message.Destination = NodeType.Nodes.FraudPrevention;

                // Add route history.
                routeHistoryId = this.AddRouteHistory(message);

                this.Publish(new FraudPreventionMessage(message.CorrelationId)
                {
                  AccountId = account.AccountId,
                  ApplicationId = application.ApplicationId,
                  CreatedAt = DateTime.Now,
                  PersonId = (long)personId,
                  RouteHistoryId = routeHistoryId,
                  ClientId = message.ClientId,
                  Source = NodeType.Nodes.AccountCreation,
                  Destination = NodeType.Nodes.FraudPrevention,
                  MessageId = message.MessageId
                });

                cli.WorkflowComplete(account.AccountId);
              }
            });
          }
        }
      }
      catch (Exception ex)
      {
        _logger.Fatal(string.Format(":: Failed - ClientId: {0}, MessageId: {1}, Stack: {2}, Message:{3}", message.ClientId, message.MessageId, ex.StackTrace.ToString(), ex.Message));
      }
    }

    internal void SinkRoute(dynamic message)
    {
      // Set the new source of the message
      message.Source = NodeType.Nodes.AccountCreation;
      message.MessageId = message.MessageId;

      // Set Destination of message
      message.Destination = NodeType.Nodes.Sink;

      // Add route history.
      this.AddRouteHistory(message);
    }

    internal AccountInfo CreateAccount(AccountServerClient cli, Application application, AccountPeriodFrequency frequency, Client client)
    {
      var account = cli.SaveAccount(application.Amount, application.Period, frequency,
                                    client.PersonId == null ? (long?)null : (long)client.PersonId,  Atlas.Online.Node.AccountCreation.AccountServer.GeneralHost.Atlas_Online);


      return account;
    }

    internal void UpdateAccount(UnitOfWork uow, Application application, AccountInfo account)
    {
      application.AccountId = account.AccountId;
      application.AccountNo = account.AccountNo;
      application.Save();
      uow.CommitChanges();
    }
   
    public override long AddRouteHistory(dynamic message)
    {
      return base.AddRouteHistory((AccountCreationMessage)message);
    }
  }
}
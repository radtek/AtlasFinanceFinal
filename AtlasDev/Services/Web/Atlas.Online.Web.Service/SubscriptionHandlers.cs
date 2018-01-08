using Atlas.Enumerators;
using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Service.AccountService;
using Atlas.RabbitMQ.Messages.Online;
using Common.Logging;
using DevExpress.Xpo;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Online.Web.Common.Extensions;
using Atlas.Online.Web.Service.EasyNetQ;
using Atlas.Online.Web.Service.OrchestrationService;
using Atlas.RabbitMQ.Messages.Notification;
using Magnum;
using Ninject.Activation.Blocks;

namespace Atlas.Online.Web.Service
{
  public class SubscriptionHandlers
  {
    private readonly ILog _log = LogManager.GetLogger(typeof(SubscriptionHandlers));

    /// <summary>
    /// Update the account information from back office platofmr
    /// </summary>
    public void UpdateApplicationInformation(UpdateApplicationInformation message)
    {
      _log.Info(
          string.Format(":: [SubscriptionHandlers] - UpdateApplicationInformation recieved request to {0} ",
              message.Type.ToString()));
      if (message.Type ==
          RabbitMQ.Messages.Online.UpdateApplicationInformation.MessageType.PaymentDateAndAffordabilityRequest)
      {
        AffordabilityOption option = null;
        Application application = null;

        new AccountService.AccountServerClient("AccountServer.NET").Using((cli) =>
        {
          using (var uow = new UnitOfWork())
          {
            List<ACC_AffordabilityDTO> affordabilityCollection = new List<ACC_AffordabilityDTO>();

            application =
                        new XPQuery<Application>(uow).FirstOrDefault(a => a.AccountId == message.AccountId);

            if (application == null)
              _log.Fatal(
                          string.Format(
                              ":: [SubscriptionHandlers] - UpdateApplicationInformation could not locate application with AccountId {0}",
                              message.AccountId));

            string error = string.Empty;
            int result;

            option = cli.GetAffordabilityOption(message.AccountId, out error, out result);

            if (option == null)
            {
              _log.Warn(
                          string.Format(
                              ":: [SubscriptionHandlers] - UpdateApplicationInformation - No option was calculated for application [{0}]",
                              application.ApplicationId));

              if (cli.UpdateAccountStatus(message.AccountId, AccountAccountStatus.Declined,
                          AccountAccountStatusReason.Affordability,
                          AccountAccountStatusSubReason.AffordabilityNoOptions))
                Application.UpdateStatus(uow, application.ApplicationId,
                            Account.AccountStatus.Declined);

              uow.CommitChanges();

              Dictionary<string, string> dict = new Dictionary<string, string>();

              dict.Add("{Name}", application.Client.Firstname);
              dict.Add("{Surname}", application.Client.Surname);

              string compiled = string.Empty;

              new OrchestrationServiceClient("OrchestrationService.NET").Using(client =>
                      {
                      compiled =
                                  client.GetCompiledTemplate(
                                      Notification.NotificationTemplate.Declined_Affordability, dict);
                    });

              ServiceLocator.Get<AtlasOnlineServiceBus>().GetServiceBus().Publish(new EmailNotifyMessage(CombGuid.Generate())
              {
                ActionDate = DateTime.Now,
                Body = compiled,
                CreatedAt = DateTime.Now,
                From = "noreply@atlasonline.co.za",
                IsHTML = true,
                Priority = Notification.NotificationPriority.High,
                Subject =
                                string.Format("Your Atlas Online application [{0}] has been Declined",
                                    application.AccountNo),
                To =
                                new XPQuery<UserProfile>(uow).FirstOrDefault(
                                    p => p.UserId == application.Client.UserId).Email
              });
            }
            else
            {
              if (option.AffordabilityOptionId == 0)
              {
                _log.Fatal(
                            string.Format(
                                ":: [SubscriptionHandlers] - UpdateApplicationInformation  - Option did not have accompanying affordabilityoptionId for application [{0}]",
                                application.ApplicationId));
              }
              else
              {
                _log.Info(
                            string.Format(
                                ":: [SubscriptionHandlers] - UpdateApplicationInformation  - Option for Amount [{0}], Capital [{1}], Instalment [{2}] over period [{3}] for application [{4}]",
                                option.Amount.ToString("C"),
                                option.CapitalAmount.ToString("C"), option.Instalment.Value.ToString("C"),
                                option.Period, application.ApplicationId));

                if (option.AffordabilityOptionType == AccountAffordabilityOptionType.RequestedOption)
                  cli.AcceptAffordabilityOption(message.AccountId, option.AffordabilityOptionId,
                              out error, out result);

                Affordability affordabilityOption = null;

                affordabilityOption = new Affordability(uow);
                affordabilityOption.Application = application;
                affordabilityOption.CapitalAmount = option.CapitalAmount;
                affordabilityOption.Amount = option.Amount;
                affordabilityOption.Instalment = option.Instalment;
                affordabilityOption.OptionType =
                            (Atlas.Enumerators.Account.AffordabilityOptionType)
                                Enum.ToObject(typeof(Atlas.Enumerators.Account.AffordabilityOptionType),
                                    (int)option.AffordabilityOptionType);
                affordabilityOption.RepaymentAmount = (decimal)option.TotalPayBack;
                affordabilityOption.TotalFees = option.TotalFees;
                affordabilityOption.OptionId = option.AffordabilityOptionId;
                affordabilityOption.Save();


                application.Affordability = affordabilityOption;
                application.Save();
                uow.CommitChanges();
              }
            }
          }
        });
      }
      else if (message.Type ==
               RabbitMQ.Messages.Online.UpdateApplicationInformation.MessageType
                   .AffordabilityRejectionDeclineLoan)
      {

      }
    }
  }
}

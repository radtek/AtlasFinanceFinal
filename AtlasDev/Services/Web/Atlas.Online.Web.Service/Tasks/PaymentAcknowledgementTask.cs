using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Service.AccountService;
using Common.Logging;
using DevExpress.Xpo;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Common.Extensions;
using Atlas.Online.Web.Service.EasyNetQ;
using Atlas.RabbitMQ.Messages.Notification;
using Magnum;
using WCFExtensions = Atlas.Online.Web.Common.Extensions.WCFExtensions;

namespace Atlas.Online.Web.Service.Tasks
{
  [DisallowConcurrentExecution]
  public class PaymentAcknowledgementTask : IJob
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(PaymentAcknowledgementTask));

    public void Execute(IJobExecutionContext context)
    {
      _log.Info(string.Format(":: [Tasks] {0} Executing...", context.JobDetail.Key.Name));

      using (var uow = new UnitOfWork())
      {
        var applicationCollection = new XPQuery<Application>(uow).Where(a => a.Status == Enumerators.Account.AccountStatus.Open).ToList();

        _log.Info(string.Format(":: [Tasks] {0} Found {1} accounts to check for payment acknowledgement.", context.JobDetail.Key.Name, applicationCollection.Count));

        if (applicationCollection.Count > 0)
        {
          WCFExtensions.Using(new AccountServerClient("AccountServer.NET"), client =>
          {
            foreach (var application in applicationCollection)
            {
              var notificationSent = new XPQuery<NotificationLog>(uow).Any(n => n.Application.ApplicationId == application.ApplicationId && n.Task == NotificationLog.NotificationTask.PaymentAcknowledgementTask);

              if (!notificationSent)
              {
                string error = string.Empty;
                int result;
                AccountInfo accountInfo = null;

                if (application.AccountId != null)
                  accountInfo = client.GetAccountInfo((long)application.AccountId, out error, out result);

                if (accountInfo.Status == AccountAccountStatus.Closed
                      && application.Status == Enumerators.Account.AccountStatus.Open)
                {

                  _log.Info(string.Format(":: [Tasks] {0} Account {1} has been paid up", context.JobDetail.Key.Name, application.AccountNo));
                  var affordability = new XPQuery<Affordability>(uow).FirstOrDefault(p => p.AffordabilityId == application.Affordability.AffordabilityId);

                  if (affordability != null)
                  {
                    affordability.Arrears = client.GetOverdueAmount((long)application.AccountId);
                    affordability.Save();
                  }

                  ServiceLocator.Get<AtlasOnlineServiceBus>().GetServiceBus().Publish<SMSNotifyMessage>(new SMSNotifyMessage(CombGuid.Generate())
                  {
                    ActionDate = DateTime.Now,
                    Body = string.Format("Thank you for paying your Atlas Online account {0}.", application.AccountNo),
                    CreatedAt = DateTime.Now,
                    Priority = Enumerators.Notification.NotificationPriority.High,
                    To = application.Client.Contacts.OfType<Contact>().FirstOrDefault(p => p.ContactType.ContactTypeId== Enumerators.General.ContactType.CellNo.ToInt()).Value
                  });

                  ServiceLocator.Get<AtlasOnlineServiceBus>().GetServiceBus().Publish<EmailNotifyMessage>(new EmailNotifyMessage(CombGuid.Generate())
                  {
                    ActionDate = DateTime.Now,
                    Body = string.Format("Thank you for paying your Atlas Online account {0}.", application.AccountNo),
                    From = "noreply@atlasonline.co.za",
                    IsHTML = true,
                    NotificationType = Enumerators.Notification.NotificationTemplate.Paid_Up,
                    Subject = string.Format("Thank you for paying your Atlas Online account.", application.AccountNo),
                    CreatedAt = DateTime.Now,
                    Priority = Enumerators.Notification.NotificationPriority.High,
                    To = new XPQuery<UserProfile>(uow).FirstOrDefault(p => p.UserId == application.Client.UserId).Email
                  });

                  // Close the account
                  Application.UpdateStatus(uow, (long)application.ApplicationId, Enumerators.Account.AccountStatus.Closed);
                  Application.UpdateIsCurrent(uow, application.ApplicationId, false);

                  // Save Notifications
                  new NotificationLog(uow)
                  {
                    Application = application,
                    CreateDate = DateTime.Now,
                    Task = NotificationLog.NotificationTask.PaymentAcknowledgementTask
                  }.Save();

                  uow.CommitChanges();
                }
              }
            }
          });
        }
      }
      _log.Info(string.Format(":: [Tasks] {0} Finished Executing.", context.JobDetail.Key.Name));
    }
  }
}
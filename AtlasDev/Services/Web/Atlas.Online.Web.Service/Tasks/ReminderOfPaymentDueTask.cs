using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Service.AccountService;
using DevExpress.Xpo;
using log4net;
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
  public sealed class ReminderofPaymentDueTask : IJob
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(ReminderofPaymentDueTask));
    private const int DELAY_NOTIFICATION = 1;

    public void Execute(IJobExecutionContext context)
    {
      _log.Info(string.Format(":: [Tasks] {0} Executing...", context.JobDetail.Key.Name));

      using (var uow = new UnitOfWork())
      {
        var applicationCollection = new XPQuery<Application>(uow).Where(a => a.Status == Enumerators.Account.AccountStatus.Open).ToList();

        _log.Info(string.Format(":: [Tasks] {0} Found {1} accounts to check for due payments.", context.JobDetail.Key.Name, applicationCollection.Count));

        if (applicationCollection.Count > 0)
        {
          WCFExtensions.Using(new AccountServerClient("AccountServer.NET"), client =>
            {
              foreach (var application in applicationCollection)
              {

                var notificationSent = new XPQuery<NotificationLog>(uow).Any(n => n.Application.ApplicationId == application.ApplicationId && n.Task == NotificationLog.NotificationTask.ReminderOfPaymentDueTask);

                if (!notificationSent)
                {
                  string error = string.Empty;
                  int result;
                  AccountInfo accountInfo = null;

                  if (application.AccountId != null)
                    accountInfo = client.GetAccountInfo((long)application.AccountId, out error, out result);

                  if (accountInfo.RepaymentDate.Subtract(DateTime.Now).TotalDays >= DELAY_NOTIFICATION)
                  {
                    ServiceLocator.Get<AtlasOnlineServiceBus>().GetServiceBus().Publish<SMSNotifyMessage>(new SMSNotifyMessage(CombGuid.Generate())
                    {
                      ActionDate = DateTime.Now,
                      Body = string.Format("This is a friendly reminder that your loan {0} is due on {1} with a payment of {2}", application.AccountNo, accountInfo.RepaymentDate.ToShortDateString(), accountInfo.RepaymentAmount),
                      CreatedAt = DateTime.Now,
                      Priority = Enumerators.Notification.NotificationPriority.High,
                      To = application.Client.Contacts.OfType<Contact>().FirstOrDefault(p => p.ContactType.ContactTypeId == Enumerators.General.ContactType.CellNo.ToInt()).Value
                    });

                    ServiceLocator.Get<AtlasOnlineServiceBus>().GetServiceBus().Publish<EmailNotifyMessage>(new EmailNotifyMessage(CombGuid.Generate())
                    {
                      ActionDate = DateTime.Now,
                      Body = string.Format("This is a friendly reminder that your loan {0} is due on {1} with a payment of {2}", application.AccountNo, accountInfo.RepaymentDate.ToShortDateString(), accountInfo.RepaymentAmount),
                      From = "noreply@atlasonline.co.za",
                      IsHTML = true,
                      NotificationType = Enumerators.Notification.NotificationTemplate.Payment,
                      Subject = "Friendly Reminder on your payment due",
                      CreatedAt = DateTime.Now,
                      Priority = Enumerators.Notification.NotificationPriority.High,
                      To = new XPQuery<UserProfile>(uow).FirstOrDefault(p => p.UserId == application.Client.UserId).Email
                    });

                    new NotificationLog(uow)
                    {
                      Application = application,
                      CreateDate = DateTime.Now,
                      Task = NotificationLog.NotificationTask.ReminderOfPaymentDueTask
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

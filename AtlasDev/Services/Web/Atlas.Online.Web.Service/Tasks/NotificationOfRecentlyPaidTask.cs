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
using Atlas.RabbitMQ.Messages.Notification;
using Magnum;
using Atlas.Online.Web.Service.OrchestrationService;
using Atlas.Enumerators;
using Atlas.Online.Web.Service.EasyNetQ;
using Contact = Atlas.Online.Data.Models.Definitions.Contact;
using WCFExtensions = Atlas.Online.Web.Common.Extensions.WCFExtensions;

namespace Atlas.Online.Web.Service.Tasks
{
  [DisallowConcurrentExecution]
  public sealed class NotificationOfRecentlyPaidTask : IJob
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(NotificationOfRecentlyPaidTask));

    public void Execute(IJobExecutionContext context)
    {
      _log.Info(string.Format(":: [Tasks] {0} Executing...", context.JobDetail.Key.Name));

      using (var uow = new UnitOfWork())
      {
        var applicationCollection = new XPQuery<Application>(uow).Where(a => a.Status == Enumerators.Account.AccountStatus.Approved && a.IsCurrent && a.AccountId != null).ToList();

        _log.Info(string.Format(":: [Tasks] {0} found {1} approved accounts pending payment notification...", context.JobDetail.Key.Name, applicationCollection.Count));

        WCFExtensions.Using(new AccountServerClient("AccountServer.NET"), client =>
        {
          foreach (var application in applicationCollection)
          {
            var notificationSent = new XPQuery<NotificationLog>(uow).Any(n => n.Application.ApplicationId == application.ApplicationId && n.Task == NotificationLog.NotificationTask.NotificationOfRecentlyPaidTask);

            if (!notificationSent)
            {
              string error = string.Empty;
              int result;

              var account = client.GetAccountInfo((long)application.AccountId, out error, out result);

              if (account == null)
                continue;

              if (account.Status == AccountAccountStatus.Open && application.Status == Enumerators.Account.AccountStatus.Approved)
              {
                Application.UpdateStatus(uow, (long)application.ApplicationId, Enumerators.Account.AccountStatus.Open);
                _log.Info(string.Format(":: [Task] {0} Application {1} updating status to Open", context.JobDetail.Key.Name, application.ApplicationId));

                Dictionary<string, string> dict = new Dictionary<string, string>();

                dict.Add("{Name}", application.Client.Firstname);
                dict.Add("{Surname}", application.Client.Surname);
                dict.Add("{LoanAmount}", account.LoanAmount.ToString("#.##"));
                dict.Add("{LoanTerm}", account.Period.ToString());
                dict.Add("{RepaymentAmount}", account.RepaymentAmount.ToString("dd/MM/yyyy"));
                dict.Add("{RepaymentDate}", account.RepaymentDate.ToString("dd/MM/yyyy"));

                string compiled = string.Empty;

                WCFExtensions.Using(new OrchestrationServiceClient("OrchestrationService.NET"), cli =>
                {
                  compiled = cli.GetCompiledTemplate(Notification.NotificationTemplate.Payment, dict);
                });


                ServiceLocator.Get<AtlasOnlineServiceBus>().GetServiceBus().Publish<SMSNotifyMessage>(new SMSNotifyMessage(CombGuid.Generate())
                {
                  ActionDate = DateTime.Now,
                  Body = "Congratulations! Your Atlas Online loan has been approved. Your money will be in your bank account within a few minutes. ",
                  CreatedAt = DateTime.Now,
                  Priority = Enumerators.Notification.NotificationPriority.High,
                  To = application.Client.Contacts.OfType<Contact>().FirstOrDefault(p => p.ContactType.ContactTypeId == Enumerators.General.ContactType.CellNo.ToInt()).Value
                });

                ServiceLocator.Get<AtlasOnlineServiceBus>().GetServiceBus().Publish<EmailNotifyMessage>(new EmailNotifyMessage(CombGuid.Generate())
                {
                  ActionDate = DateTime.Now,
                  From = "noreply@atlasonline.co.za",
                  IsHTML = true,
                  Body = compiled,
                  NotificationType = Enumerators.Notification.NotificationTemplate.Payment,
                  Subject = "Congratulations! Your Atlas Online loan has been approved.",
                  CreatedAt = DateTime.Now,
                  Priority = Enumerators.Notification.NotificationPriority.High,
                  To = new XPQuery<UserProfile>(uow).FirstOrDefault(p => p.UserId == application.Client.UserId).Email
                });

                // Save Notifications
                new NotificationLog(uow)
                {
                  Application = application,
                  CreateDate = DateTime.Now,
                  Task = NotificationLog.NotificationTask.NotificationOfRecentlyPaidTask
                }.Save();
              }
              uow.CommitChanges();
            }
          }
        });
      }
      _log.Info(string.Format(":: [Tasks] {0} Finished Executing.", context.JobDetail.Key.Name));
    }
  }
}

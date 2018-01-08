using Atlas.Enumerators;
using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Service.OrchestrationService;
using Atlas.RabbitMQ.Messages.Notification;
using DevExpress.Xpo;
using log4net;
using Magnum;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Online.Web.Common.Extensions;
using System.Configuration;
using Atlas.Online.Web.Service.EasyNetQ;

namespace Atlas.Online.Web.Service.Tasks
{
  [DisallowConcurrentExecution]
  public class NotificationOfInactiveAccountTask : IJob
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(NotificationOfInactiveAccountTask));
    private const double EXPIRE_HOURS = 24.00D;
    private const int DELAY_MIN_NOTIFICATION = 30;
    private string NO_REPLY = ConfigurationManager.AppSettings["noreply"];
    private const string ACCOUNT_CANCELLED = "Your Atlas Online application has been cancelled.";
    private const string ACCOUNT_EXPIRY = "Your Atlas Online application will expire in 24 hours.";

    public void Execute(IJobExecutionContext context)
    {
      _log.Info(string.Format(":: [Tasks] {0} Executing...", context.JobDetail.Key.Name));

      using (var uow = new UnitOfWork())
      {
        //List<Enumerators.Account.AccountStatus> statuses = new List<Enumerators.Account.AccountStatus>();
        //statuses.Add(Enumerators.Account.AccountStatus.Inactive);

        var applicationCollection = new XPQuery<Application>(uow).Where(a => a.Status == Account.AccountStatus.Inactive).ToList();

        if (applicationCollection.Count > 0)
        {
          foreach (var application in applicationCollection)
          {
            TimeSpan ts = (DateTime.Now - application.CreateDate);

            var notificationLog = new XPQuery<NotificationLog>(uow).Where(p =>
                                                                          p.Application.ApplicationId == application.ApplicationId && p.Task
                                                                          == NotificationLog.NotificationTask.NotificationOfInactiveAccountTask)
                                                                          .OrderBy(p => p.CreateDate).ToList();

            var sentToday = notificationLog.Any(p => p.CreateDate.Date == DateTime.Now.Date);
            if (sentToday)
              continue;
            else
            {
            
              // Cancel the application
              if (notificationLog.Count >= 1)
              {
                _log.Warn(string.Format(":: [Tasks] {0} Cancel Notification Application Expired: ({1}), Day: ({2})", context.JobDetail.Key.Name, application.ApplicationId, notificationLog.Count));


                if (ts.TotalHours >= EXPIRE_HOURS)
                  SendNotification(uow, application, NO_REPLY, ACCOUNT_CANCELLED, Notification.NotificationTemplate.Application_Expired);
              }
              else if (notificationLog.Count == 0 && ts.TotalHours >= EXPIRE_HOURS)
              {
                _log.Warn(string.Format(":: [Tasks] {0} Cancel Notification Application Expired: ({1})", context.JobDetail.Key.Name, application.ApplicationId));

                SendNotification(uow, application, NO_REPLY, ACCOUNT_CANCELLED, Notification.NotificationTemplate.Application_Expired);
              }
              else if (ts.TotalMinutes >= DELAY_MIN_NOTIFICATION && ts.TotalHours < EXPIRE_HOURS && notificationLog.Count == 0)
              {
                _log.Warn(string.Format(":: [Tasks] {0} Cancel Notification Application Expiring: ({1})", context.JobDetail.Key.Name, application.ApplicationId));

                  SendNotification(uow, application, NO_REPLY, ACCOUNT_EXPIRY, Notification.NotificationTemplate.Application_Expiring);
              }

              new NotificationLog(uow)
              {
                Application = application,
                CreateDate = DateTime.Now,
                Task = NotificationLog.NotificationTask.NotificationOfInactiveAccountTask
              }.Save();
            }
            uow.CommitChanges();
          }
        }
      }

      _log.Info(string.Format(":: [Tasks] {0} Finished Executing.", context.JobDetail.Key.Name));
    }

    private void SendNotification(UnitOfWork uow, Application application, string from, string subject, Notification.NotificationTemplate notificationTemplate)
    {
      if (notificationTemplate == Notification.NotificationTemplate.Application_Expired)
      {
        Application.UpdateStatus(uow, application.ApplicationId, Enumerators.Account.AccountStatus.Cancelled);
        Application.UpdateIsCurrent(uow, application.ApplicationId, false);
      }      

      Dictionary<string, string> dict = new Dictionary<string, string>();

      dict.Add("{Name}", application.Client.Firstname);
      dict.Add("{Surname}", application.Client.Surname);
      dict.Add("{AccountNo}", application.AccountNo);

      string compiled = string.Empty;

      new OrchestrationServiceClient("OrchestrationService.NET").Using(client =>
      {
        compiled = client.GetCompiledTemplate(notificationTemplate, dict);
      });

      ServiceLocator.Get<AtlasOnlineServiceBus>().GetServiceBus().Publish<EmailNotifyMessage>(new EmailNotifyMessage(CombGuid.Generate())
      {
        ActionDate = DateTime.Now,
        Body = compiled,
        From = from,
        IsHTML = true,
        NotificationType = notificationTemplate,
        Subject = subject,
        CreatedAt = DateTime.Now,
        Priority = Enumerators.Notification.NotificationPriority.High,
        To = new XPQuery<UserProfile>(uow).FirstOrDefault(p => p.UserId == application.Client.UserId).Email
      });
    }
  }
}
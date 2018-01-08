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
using Atlas.Enumerators;
using Atlas.Online.Web.Service.EasyNetQ;

namespace Atlas.Online.Web.Service.Tasks
{
  [DisallowConcurrentExecution]
  public sealed class QuoteSevenDayExpireTask : IJob
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(QuoteSevenDayExpireTask));
    private const int EXPIRE_IN_DAYS = 7;
    private const double DELAY_TOTAL_MIN = 60.00D;

    public void Execute(IJobExecutionContext context)
    {
      _log.Info(string.Format(":: [Tasks] {0} Executing...", context.JobDetail.Key.Name));

      using (var uow = new UnitOfWork())
      {
        List<Enumerators.Account.AccountStatus> statuses = new List<Enumerators.Account.AccountStatus>();
        statuses.Add(Enumerators.Account.AccountStatus.PreApproved);

        var applicationCollection = new XPQuery<Application>(uow).Where(a => statuses.Contains(a.Status)).ToList();


        if (applicationCollection.Count > 0)
        {
          foreach (var application in applicationCollection)
          {
            TimeSpan ts = (application.CreateDate - DateTime.Now);
            if (ts.TotalMinutes >= DELAY_TOTAL_MIN)
            {
              var notificationLog = new XPQuery<NotificationLog>(uow).Where(p => p.Application.ApplicationId == application.ApplicationId && p.Task
                                                                                                                  == NotificationLog.NotificationTask.QuoteSevenDayExpireTask).ToList();
              if (notificationLog.Count < EXPIRE_IN_DAYS)
              {
                var sentToday = notificationLog.Any(p => p.CreateDate.Date == DateTime.Now.Date);
                if (sentToday)
                  continue;
                else
                {
                  Dictionary<string, string> dict = new Dictionary<string, string>();

                  dict.Add("{Name}", application.Client.Firstname);
                  dict.Add("{Surname}", application.Client.Surname);
                  dict.Add("{delay}", (EXPIRE_IN_DAYS - notificationLog.Count).ToString());
                  dict.Add("{days}", (EXPIRE_IN_DAYS - notificationLog.Count) <= 1 ? "day" : "days");
                  dict.Add("{AccountNo}", application.AccountNo);

                  string compiled = string.Empty;

                  new OrchestrationServiceClient("OrchestrationService.NET").Using(client =>
                  {
                    compiled = client.GetCompiledTemplate(Notification.NotificationTemplate.Quote_Expire_Daily, dict);
                  });

                  ServiceLocator.Get<AtlasOnlineServiceBus>().GetServiceBus().Publish<EmailNotifyMessage>(new EmailNotifyMessage(CombGuid.Generate())
                  {
                    ActionDate = DateTime.Now,
                    Body = compiled,
                    From = "noreply@atlasonline.co.za",
                    IsHTML = true,
                    NotificationType = Enumerators.Notification.NotificationTemplate.Quote_Expire_Daily,
                    Subject = string.Format("Your Atlas Online Quotation will expire in {0} {1}", (EXPIRE_IN_DAYS - notificationLog.Count), (EXPIRE_IN_DAYS - notificationLog.Count) <= 1 ? "day" : "days"),
                    CreatedAt = DateTime.Now,
                    Priority = Enumerators.Notification.NotificationPriority.High,
                    To = new XPQuery<UserProfile>(uow).FirstOrDefault(p => p.UserId == application.Client.UserId).Email
                  });

                  new NotificationLog(uow)
                  {
                    Application = application,
                    CreateDate = DateTime.Now,
                    Task = NotificationLog.NotificationTask.QuoteSevenDayExpireTask
                  }.Save();
                }
                uow.CommitChanges();
              }
            }
          }
        }
      }

      _log.Info(string.Format(":: [Tasks] {0} Finished Executing.", context.JobDetail.Key.Name));
    }
  }
}

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
using Atlas.Online.Web.Common.Extensions;
using Atlas.RabbitMQ.Messages.Notification;
using Magnum;
using Atlas.Online.Web.Service.OrchestrationService;
using Atlas.Enumerators;
using Atlas.Online.Web.Service.EasyNetQ;


namespace Atlas.Online.Web.Service.Tasks
{
  [DisallowConcurrentExecution]
  public sealed class QuotationExpiredTask : IJob
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(QuotationExpiredTask));

    public void Execute(IJobExecutionContext context)
    {
      _log.Info(string.Format(":: [Tasks] {0} Executing...", context.JobDetail.Key.Name));

      using (var uow = new UnitOfWork())
      {
        List<Enumerators.Account.AccountStatus> statuses = new List<Enumerators.Account.AccountStatus>();
        statuses.Add(Enumerators.Account.AccountStatus.PreApproved);

        var applicationCollection = new XPQuery<Application>(uow).Where(a => statuses.Contains(a.Status) && a.CreateDate < DateTime.Now.AddDays(7 * -1)).ToList();

        _log.Info(string.Format(":: [Tasks] {0} found {1} accounts to expire...", context.JobDetail.Key.Name, applicationCollection.Count));

        if (applicationCollection.Count > 0)
        {
          new AccountServerClient().Using(client =>
          {
            foreach (var application in applicationCollection)
            {
              _log.Info(string.Format(":: [Task] {0} Application {1} is {2} days old, expired so cancelling...", context.JobDetail.Key.Name, application.ApplicationId, DateTime.Now.Subtract(application.CreateDate).TotalDays));

              var notificationSent = new XPQuery<NotificationLog>(uow).Any(p => p.Application.ApplicationId == application.ApplicationId && p.Task == NotificationLog.NotificationTask.QuotationExpiredTask);

              if (!notificationSent)
              {
                // Update Web DB sttus
                Application.UpdateStatus(uow, application.ApplicationId, Enumerators.Account.AccountStatus.Cancelled);
                Application.UpdateIsCurrent(uow, application.ApplicationId, false);

                if (application.AccountId != null)
                {
                  client.UpdateAccountStatus((long)application.AccountId, AccountAccountStatus.Cancelled, null, null);
                  // Reject the affordability if they have one
                  if (application.Affordability != null)
                  {
                    string error = string.Empty;
                    int result;
                    client.RejectAffordabilityOption((long)application.AccountId, application.Affordability.OptionId, out error, out result);
                  }

                  Dictionary<string, string> dict = new Dictionary<string, string>();

                  dict.Add("{Name}", application.Client.Firstname);
                  dict.Add("{Surname}", application.Client.Surname);
                  dict.Add("{AccountNo}", application.AccountNo);

                  string compiled = string.Empty;

                  new OrchestrationServiceClient("OrchestrationService.NET").Using(cli =>
                  {
                    compiled = cli.GetCompiledTemplate(Notification.NotificationTemplate.Quote_Expired, dict);
                  });

                  ServiceLocator.Get<AtlasOnlineServiceBus>().GetServiceBus().Publish<EmailNotifyMessage>(new EmailNotifyMessage(CombGuid.Generate())
                  {
                    ActionDate = DateTime.Now,
                    Body = compiled,
                    From = "noreply@atlasonline.co.za",
                    IsHTML = true,
                    NotificationType = Enumerators.Notification.NotificationTemplate.Quote_Expired,
                    Subject = "Quotation Expired",
                    CreatedAt = DateTime.Now,
                    Priority = Enumerators.Notification.NotificationPriority.High,
                    To = new XPQuery<UserProfile>(uow).FirstOrDefault(p => p.UserId == application.Client.UserId).Email
                  });

                  new NotificationLog(uow)
                  {
                    Application = application,
                    CreateDate = DateTime.Now,
                    Task = NotificationLog.NotificationTask.QuotationExpiredTask
                  }.Save();
                }
              }
              uow.CommitChanges();
              _log.Info(string.Format(":: [Task] {0} Application {1} is {2} days old, cancelled.", context.JobDetail.Key.Name, application.ApplicationId, DateTime.Now.Subtract(application.CreateDate).TotalDays));
            }
          });
        }
      }
      _log.Info(string.Format(":: [Tasks] {0} Finished Executing.", context.JobDetail.Key.Name));
    }
  }
}

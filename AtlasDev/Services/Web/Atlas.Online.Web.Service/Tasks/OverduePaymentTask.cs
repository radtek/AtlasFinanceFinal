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
  public class OverduePaymentTask : IJob
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(OverduePaymentTask));

    public void Execute(IJobExecutionContext context)
    {
      _log.Info(string.Format(":: [Tasks] {0} Executing...", context.JobDetail.Key.Name));

      using (var uow = new UnitOfWork())
      {
        var applicationCollection = new XPQuery<Application>(uow).Where(a => a.Status == Enumerators.Account.AccountStatus.Open).ToList();

        _log.Info(string.Format(":: [Tasks] {0} Found {1} accounts to check for overdue payments.", context.JobDetail.Key.Name, applicationCollection.Count));

        if (applicationCollection.Count > 0)
        {
          WCFExtensions.Using(new AccountServerClient("AccountServer.NET"), client =>
          {
            foreach (var application in applicationCollection)
            {
              var notificationSent = new XPQuery<NotificationLog>(uow).Any(n => n.Application.ApplicationId == application.ApplicationId && n.Task == NotificationLog.NotificationTask.OverduePaymentTask);

              if (!notificationSent)
              {
                Decimal amount = 0.0M;

                if (application.AccountId != null)
                  amount = client.GetOverdueAmount((long)application.AccountId);

                if (amount > 0.0M)
                {
                  _log.Info(string.Format(":: [Tasks] {0} Account {1} is currently overdue with an ammount {2}", context.JobDetail.Key.Name, application.AccountNo, amount));

                  var affordability = new XPQuery<Affordability>(uow).FirstOrDefault(p => p.AffordabilityId == application.Affordability.AffordabilityId);

                  if (affordability != null)
                  {
                    affordability.Arrears = amount;
                    affordability.Save();
                  }
                  string error = string.Empty;
                  int result = 0;

                  var accountInfo = client.GetAccountInfo((long)application.AccountId, out error, out result);

                  Dictionary<string, string> dict = new Dictionary<string, string>();

                  dict.Add("{Name}", application.Client.Firstname);
                  dict.Add("{Surname}", application.Client.Surname);
                  dict.Add("{AccountNo}", application.AccountNo);
                  dict.Add("{RepaymentDate}", accountInfo.FirstInstalmentDate.ToString("dd/MM/yyyy"));
                  dict.Add("{RepaymentAmount}", accountInfo.RepaymentAmount.ToString("#.##"));
                  dict.Add("{OverdueAmount}", amount.ToString("#.##"));

                  string compiled = string.Empty;

                  WCFExtensions.Using(new OrchestrationServiceClient("OrchestrationService.NET"), cli =>
                  {
                    compiled = cli.GetCompiledTemplate(Notification.NotificationTemplate.Overdue_Account, dict);
                  });


                  ServiceLocator.Get<AtlasOnlineServiceBus>().GetServiceBus().Publish<SMSNotifyMessage>(new SMSNotifyMessage(CombGuid.Generate())
                  {
                    ActionDate = DateTime.Now,
                    Body = string.Format("This is a reminder that your Atlas Online account {0} is now overdue with an oustanding amount of {1}", application.AccountNo, amount.ToString("#.##")),
                    CreatedAt = DateTime.Now,
                    Priority = Enumerators.Notification.NotificationPriority.High,
                    To = application.Client.Contacts.OfType<Contact>().FirstOrDefault(p => p.ContactType.ContactTypeId == Enumerators.General.ContactType.CellNo.ToInt()).Value
                  });

                  ServiceLocator.Get<AtlasOnlineServiceBus>().GetServiceBus().Publish<EmailNotifyMessage>(new EmailNotifyMessage(CombGuid.Generate())
                  {
                    ActionDate = DateTime.Now,
                    Body = compiled,
                    From = "noreply@atlasonline.co.za",
                    IsHTML = true,
                    NotificationType = Enumerators.Notification.NotificationTemplate.Overdue_Account,
                    Subject = string.Format("Your Atlas Online account {0} is now overdue.", application.AccountNo),
                    CreatedAt = DateTime.Now,
                    Priority = Enumerators.Notification.NotificationPriority.High,
                    To = new XPQuery<UserProfile>(uow).FirstOrDefault(p => p.UserId == application.Client.UserId).Email
                  });

                  // Save Notifications
                  new NotificationLog(uow)
                  {
                    Application = application,
                    CreateDate = DateTime.Now,
                    Task = NotificationLog.NotificationTask.OverduePaymentTask
                  }.Save();
                }

                uow.CommitChanges();
              }
            }
          });
        }
      }
      _log.Info(string.Format(":: [Tasks] {0} Finished Executing.", context.JobDetail.Key.Name));
    }
  }
}

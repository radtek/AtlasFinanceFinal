using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Service.OrchestrationService;
using DevExpress.Xpo;
using log4net;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Online.Web.Common.Extensions;
using Atlas.Domain.Structures;
using Atlas.Online.Web.Service.AccountService;

namespace Atlas.Online.Web.Service.Tasks
{
  [DisallowConcurrentExecution]
  public class ReviewTechnicalToInactiveTask : IJob
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(ReviewTechnicalToInactiveTask));

    public void Execute(IJobExecutionContext context)
    {
      _log.Info(string.Format(":: [Tasks] {0} Executing...", context.JobDetail.Key.Name));

      List<Enumerators.Account.AccountStatus> statuses = new List<Enumerators.Account.AccountStatus>();
      statuses.Add(Enumerators.Account.AccountStatus.Technical_Error);
      statuses.Add(Enumerators.Account.AccountStatus.Review);

      using (var uow = new UnitOfWork())
      {
        var applicationCollection = new XPQuery<Application>(uow).Where(a => statuses.Contains(a.Status) && a.CreateDate >= DateTime.Now.AddDays(-7)).ToList();

        foreach(var app in applicationCollection)
        {
          string error = string.Empty;
          int result = -100;
          AccountService.AccountInfo info = null;
          new AccountServerClient("AccountServer.NET").Using(client =>
          {
            info = client.GetAccountInfo((long)app.AccountId, out error, out result);
          });
          if (info.Status == AccountAccountStatus.Inactive)
          {
            _log.Info(string.Format(":: [Tasks] {0} Web Account {1} has status {2} updating to status of {3} from core account {4}...", context.JobDetail.Key.Name, app.ApplicationId,
              app.Status.ToStringEnum(), info.Status.ToStringEnum(), info.AccountId));
            app.Status = Enumerators.Account.AccountStatus.Inactive;
            app.Step = 4; // Double check..
            app.Save();
            uow.CommitChanges();
          }

          // Maybe send notification to client??
        }
      }

      _log.Info(string.Format(":: [Tasks] {0} Finished Executing.", context.JobDetail.Key.Name));
    }
  }
}
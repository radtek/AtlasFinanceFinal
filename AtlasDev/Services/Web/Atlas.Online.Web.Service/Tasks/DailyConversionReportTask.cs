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
  public class DailyConversionReportTask : IJob
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(NotificationOfInactiveAccountTask));

    public void Execute(IJobExecutionContext context)
    {
      _log.Info(string.Format(":: [Tasks] {0} Executing...", context.JobDetail.Key.Name));

      using (var uow = new UnitOfWork())
      {
        
      }
    }
  }
}
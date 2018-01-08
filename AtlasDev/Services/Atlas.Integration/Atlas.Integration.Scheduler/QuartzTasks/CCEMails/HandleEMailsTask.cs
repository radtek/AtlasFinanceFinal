using System;

using Quartz;

using Atlas.Integration.Scheduler.QuartzTasks.CCEMails;
using Atlas.Common.Interface;


namespace Atlas.Integration.Scheduler.QuartzTasks
{
  /// <summary>
  /// Checks for waiting POP3 message, processes via scorecard and redirects message to InDox/call-centre
  /// </summary>
  [DisallowConcurrentExecution]
  internal class HandleEMailsTask : IJob
  {
    public HandleEMailsTask(ILogging log, IConfigSettings config)
    {
      _log = log;
      _config = config;
    }


    public void Execute(IJobExecutionContext context)
    {
      try
      {
        _log.Information("ProcessEMails.Execute starting");

        EMailUtils.POP3QueueNewMessages(_log, _config);
        var processedOk = EMailUtils.ProcessQueued(_log, _config);
        EMailUtils.SendOutBox(_log, _config);
        EMailUtils.POP3DeleteMessages(_log, _config, processedOk);
      }
      catch (Exception err)
      {
        _log.Error(err, "Execute");
      }
      _log.Information("ProcessEMails.Execute completed");
    }


    private readonly ILogging _log;
    private readonly IConfigSettings _config;
  }
}

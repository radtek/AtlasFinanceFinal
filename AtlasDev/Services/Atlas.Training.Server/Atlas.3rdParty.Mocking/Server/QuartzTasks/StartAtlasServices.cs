using System;

using Serilog;
using Quartz;


namespace Atlas.Server.Training.QuartzTasks
{
  [DisallowConcurrentExecution]
  internal class StartAtlasServices: IJob
  {
    public void Execute(IJobExecutionContext context)
    {
      var methodName = "StartAtlasServices.Execute";

      try
      {
        _log.Information("{MethodName} started", methodName);
        ServiceUtils.StartStopServices(_log, true);
      }
      catch (Exception err)
      {
        _log.Error(err, "Execute");
      }

      _log.Information("{MethodName} completed", methodName);
    }


    #region Private fields

    private static readonly ILogger _log = Log.ForContext<StartAtlasServices>();

    #endregion

  }
}

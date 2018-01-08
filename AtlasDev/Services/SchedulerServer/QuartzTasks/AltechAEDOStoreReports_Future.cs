using System;

using Quartz;

using Atlas.Common.Interface;


namespace SchedulerServer.QuartzTasks
{
  [DisallowConcurrentExecution]
  public class AltechAEDOStoreReports_Future : IJob
  {
    public AltechAEDOStoreReports_Future(ILogging log, IConfigSettings config)
    {
      _log = log;
      _config = config;
    }

   
    public void Execute(IJobExecutionContext context)
    {
      try
      {
        _log.Information("Altech AEDO Store Future Report Starting");        
        SchedulerServer.AltechNuPay.AEDO.ImportReports(_log, _config, true);
        _log.Information("Altech AEDO Store Future Report Complete");
      }
      catch (Exception err)
      {
        _log.Error(err, "Altech AEDO Store Future Report Failed");
      }
    }
    

    private readonly ILogging _log;

    private readonly IConfigSettings _config;

  }
}

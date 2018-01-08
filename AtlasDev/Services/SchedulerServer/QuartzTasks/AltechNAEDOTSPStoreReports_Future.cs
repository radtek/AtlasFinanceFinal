using System;

using Quartz;

using Atlas.Common.Interface;


namespace SchedulerServer.QuartzTasks
{
  [DisallowConcurrentExecution]
  public class AltechNAEDOTSPStoreReports_Future : IJob
  {
    public AltechNAEDOTSPStoreReports_Future(ILogging log, IConfigSettings config)
    {
      _log = log;
      _config = config;
    }


    public void Execute(IJobExecutionContext context)
    {
      try
      {
        _log.Information("Altech NAEDO & TSP Store Future Report Starting");
        SchedulerServer.AltechNuPay.NAEDO.ImportReports(_log, _config, true);
        _log.Information("Altech NAEDO & TSP Store Future Reports Complete");
      }
      catch (Exception err)
      {
        _log.Error(err, "Altech NAEDO & TSP Store Future Reports Failed");
      }
    }


    private readonly ILogging _log;

    private readonly IConfigSettings _config;

  }
}

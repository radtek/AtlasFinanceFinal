using System;

using Quartz;

using Atlas.Common.Interface;


namespace SchedulerServer.QuartzTasks
{
  [DisallowConcurrentExecution]
  public class AltechNAEDOTSPStoreReports : IJob
  {
    public AltechNAEDOTSPStoreReports(ILogging log, IConfigSettings config)
    {
      _log = log;
      _config = config;
    }


    public void Execute(IJobExecutionContext context)
    {
      try
      {
        _log.Information("Altech NAEDO & TSP Store Reports Starting");        
        SchedulerServer.AltechNuPay.NAEDO.ImportReports(_log, _config);
        _log.Information("Altech NAEDO & TSP Store Reports Complete");
      }
      catch (Exception err)
      {
        _log.Error(err, "Altech NAEDO & TSP Store Reports Failed");
      }
    }


    private readonly ILogging _log;

    private readonly IConfigSettings _config;

  }
}

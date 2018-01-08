using System;

using Quartz;

using Atlas.Common.Interface;


namespace SchedulerServer.QuartzTasks
{
  [DisallowConcurrentExecution]
  public class AltechAEDOStoreReports : IJob
  {
    public AltechAEDOStoreReports(ILogging log, IConfigSettings config)
    {
      _log = log;
      _config = config;
    }


    public void Execute(IJobExecutionContext context)
    {
      try
      {
        _log.Information("Altech AEDO Store Reports Starting");                
        SchedulerServer.AltechNuPay.AEDO.ImportReports(_log, _config);
        _log.Information("Altech AEDO Store Reports Complete");
      }
      catch (Exception err)
      {
        _log.Error(err, "Altech AEDO Store Reports Failed");
      }
    } 

     
    private readonly ILogging _log;

    private readonly IConfigSettings _config;

  }
}

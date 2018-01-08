using System;

using Atlas.Common.Interface;


namespace Atlas.Evolution.Server
{
  internal class MainService
  {
    public MainService(ILogging log, IConfigSettings config)
    {
      _log = log;
      _config = config;
    }


    internal bool Start()
    {
      try
      {
        _log.Information("Starting...");

        //#region Testing        
        //(new QuartzTasks.CreateDailyBatch(_log, _config)).Execute(null);       
        //(new QuartzTasks.CreateMonthlyBatch(_log, _config)).Execute(null);
        //(new QuartzTasks.UploadPendingBatches(_log, _config)).Execute(null);
        //#endregion

        _log.Information("Started");
        return true;
      }
      catch (Exception err)
      {
        _log.Error(err, "Start()");
        return false;
      }
    }


    internal bool Stop()
    {
      try
      {
        _log.Information("Stopping...");
      }
      catch (Exception err)
      {
        _log.Error(err, "Stop()");
        return false;
      }

      _log.Information("Stopped");
      return true;
    }


    /// <summary>
    /// Logging
    /// </summary>
    private readonly ILogging _log;
    private readonly IConfigSettings _config;
  }
}
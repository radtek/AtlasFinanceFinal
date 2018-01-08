using System;

using Atlas.Common.Interface;


namespace Atlas.Accounts.Reports
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
        #region Testing
        //(new QuartzTasks.DayBeforeOverdues(_log, _config)).Execute(null);
        //(new QuartzTasks.ManualReceipts(_log, _config)).Execute(null);
        //(new QuartzTasks.AVS_SP_Report(_log, _config)).Execute(null);
        //(new QuartzTasks.FirstInstallOverdue(_log, _config)).Execute(null);
        //(new QuartzTasks.Receipts85Percent(_log, _config)).Execute(null);
        //(new QuartzTasks.Audit_Trail(_log, _config)).Execute(null);
        #endregion

        _log.Information("Service started");

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
      _log.Warning("Service stopped");
      return true;
    }

    
    private readonly ILogging _log;
    private readonly IConfigSettings _config;

  }
}

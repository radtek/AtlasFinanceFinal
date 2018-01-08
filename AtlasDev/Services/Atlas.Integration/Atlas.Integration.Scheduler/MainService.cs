using System;

using Atlas.Common.Interface;


namespace Atlas.Integration.Scheduler
{
  class MainService
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
        var check = new QuartzTasks.Opportunities.HandleOpportunitiesTask(_log, _config);
        check.Execute(null);

        //var email = new HandleEMailsTask(_log, _config);
        //email.Execute(null);
        #endregion

        return true;
      }
      catch (Exception err)
      {
        return false;
      }
    }


    internal bool Stop()
    {
      try
      {
      
      }
      catch (Exception err)
      {
        _log.Error(err, "Error while shutting down");
      }

      return true;    
    }


    private readonly ILogging _log;
    private readonly IConfigSettings _config;

  }
}

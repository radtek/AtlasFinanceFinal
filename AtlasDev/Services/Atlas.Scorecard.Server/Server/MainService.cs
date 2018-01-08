using System;

using Atlas.Common.Interface;
using Atlas.Servers.Common.WCF;
using Atlas.ThirdParty.CS.Bureau.WCF.WCF_DI;


namespace Atlas.ThirdParty.CS.Bureau
{
  internal class MainService
  {    
    public MainService(ILogging log, IScorecardServiceHost scorecardServiceHost)
    {
      _log = log;
      serviceHost_Credit = scorecardServiceHost;
    }


    public bool Start()
    {
      try
      {       
        serviceHost_Credit.Open();
        serviceHost_Credit.LogEndpoints(_log);
        _log.Information("Successfully loaded 'ScorecardServer' service");
              
        return true;
      }
      catch (Exception err)
      {        
        _log.Fatal(err, "Failed to load 'ScorecardServer' service");
        return false;
      }
    }


    public bool Stop()
    {
      try
      {
        if (serviceHost_Credit != null)
        {
          serviceHost_Credit.Close();
          serviceHost_Credit = null;
          _log.Information("ScorecardServer service Closed");
        }
      }
      catch (Exception err)
      {
        _log.Fatal(err, "Error trying to stop 'ScorecardServer' service");        
      }

      return true;
    }

    
    private IBaseServiceHost serviceHost_Credit;

    private readonly ILogging _log;
    
  }
}

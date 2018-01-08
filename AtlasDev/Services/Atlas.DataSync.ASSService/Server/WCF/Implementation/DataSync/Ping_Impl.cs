using System;
using System.Collections.Generic;

using Atlas.DataSync.WCF.Interface;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;
using ASSServer.DbUtils;


namespace ASSServer.WCF.Implementation.DataSync
{
  
  public static class Ping_Impl
  {
    public static PingResult Execute(ILogging log, ICacheServer cache, IConfigSettings config, SourceRequest sourceRequest)
    {
      var methodName = "Ping";
     
      try
      {
        ASS_BranchServer_Cached server;
        string errorMessage;
        if (!Checks.VerifyBranchServerRequest(log, sourceRequest, out server, out errorMessage))
        {
          log.Warning(new Exception(errorMessage), "{MethodName}- {@Request}", methodName, sourceRequest);
          return new PingResult() { MachineAuthorised = false, ErrorMessage = errorMessage };
        }
                
        return new PingResult() { MachineAuthorised = server.MachineAuthorised };
      }
      catch (Exception err)
      {
        log.Error(err, "{MethodName}- {@Request}", methodName, sourceRequest);
        DbRepos.LogASSBranchServerEvent(0, DateTime.Now, methodName, err.Message, 5);
        return new PingResult() { MachineAuthorised = false, ErrorMessage = err.Message };
      }
    }

  }
}

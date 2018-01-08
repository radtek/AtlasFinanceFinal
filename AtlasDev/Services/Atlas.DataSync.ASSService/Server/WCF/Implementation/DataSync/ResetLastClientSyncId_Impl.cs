using System;
using System.Collections.Generic;

using Atlas.DataSync.WCF.Interface;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;
using ASSServer.DbUtils;


namespace ASSServer.WCF.Implementation.DataSync
{  
  public static class ResetLastClientSyncId_Impl
  {
    public static bool Execute(ILogging log, ICacheServer cache, IConfigSettings config, SourceRequest sourceRequest)
    {
      var methodName = "ResetLastClientSyncId";
    
      try
      {
        log.Information("{MethodName} starting, {@Request}", methodName, sourceRequest);

        ASS_BranchServer_Cached server;
        string errorMessage;
        if (!Checks.VerifyBranchServerRequest(log, sourceRequest, out server, out errorMessage))
        {
          log.Warning(new Exception(errorMessage), "{MethodName}- {@Request}", methodName, sourceRequest);
          return false;
        }

        server.LastProcessedClientRecId = 0;
        server.LastProcessedClientAuditRecId = 0;
        server.LastSyncDT = DateTime.MinValue;
        cache.Set(new List<ASS_BranchServer_Cached> { server });
        
        return true;
      }
      catch (Exception err)
      {
        log.Error(err, "{MethodName}- {@Request}", methodName, sourceRequest);
        DbRepos.LogASSBranchServerEvent(0, DateTime.Now, methodName, err.Message, 5);
        return false;
      }
    }

  }
}

using System;

using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.DataSync.WCF.Interface;
using Atlas.Cache.Interfaces.Classes;
using ASSServer.DbUtils;


namespace ASSServer.WCF.Implementation.DataSync
{  
  internal class BranchLastAuditRecId_Impl
  {
    internal static long Execute(ILogging log, ICacheServer cache, IConfigSettings config, SourceRequest sourceRequest)
    {
      var methodName = "BranchLastAuditRecId";
    
      try
      {
        ASS_BranchServer_Cached server;
        string errorMessage;
        if (!Checks.VerifyBranchServerRequest(log, sourceRequest, out server, out errorMessage))
        {
          log.Warning(new Exception(errorMessage), "{MethodName}- {@Request}", methodName, sourceRequest);
          return -1;
        }

        return server.LastProcessedClientAuditRecId;
       
      }
      catch (Exception err)
      {
        log.Error(err, "{MethodName}- {@Request}", methodName, sourceRequest);
        DbRepos.LogASSBranchServerEvent(0, DateTime.Now, methodName, err.Message, 5);
        return -1;
      }
    }
  }
}

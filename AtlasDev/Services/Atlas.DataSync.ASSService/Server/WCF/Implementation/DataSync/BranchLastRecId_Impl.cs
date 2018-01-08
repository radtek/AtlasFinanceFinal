using System;

using Atlas.DataSync.WCF.Interface;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;
using ASSServer.DbUtils;


namespace ASSServer.WCF.Implementation.DataSync
{  
  public static class BranchLastRecId_Impl
  {
    public static Int64 Execute(ILogging log, ICacheServer cache, IConfigSettings config, SourceRequest sourceRequest)
    {
      var methodName = "BranchLastRecId";
     
      try
      {
        ASS_BranchServer_Cached server;
        string errorMessage;
        if (!Checks.VerifyBranchServerRequest(log, sourceRequest, out server, out errorMessage))
        {
          log.Warning(new Exception(errorMessage), "{MethodName}- {@Request}", methodName, sourceRequest);
          return -1;
        }

        return server.LastProcessedClientRecId;     
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

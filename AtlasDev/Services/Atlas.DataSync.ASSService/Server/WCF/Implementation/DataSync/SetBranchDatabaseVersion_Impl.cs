using System;

using Atlas.DataSync.WCF.Interface;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;
using ASSServer.DbUtils;
using ASSServer.Utils.PSQL.DbfImport;


namespace ASSServer.WCF.Implementation.DataSync
{  
  public static class SetBranchDatabaseVersion_Impl
  {
    public static bool Execute(ILogging log, ICacheServer cache, IConfigSettings config, SourceRequest sourceRequest, string clientDbVersion)
    {     
      var methodName = "SetBranchDatabaseVersion";
      try
      {        
        ASS_BranchServer_Cached server;
        string errorMessage;
        if (!Checks.VerifyBranchServerRequest(log, sourceRequest, out server, out errorMessage))
        {
          log.Warning(new Exception(errorMessage), "{MethodName}- {@Request}", methodName, sourceRequest);
          return false;
        }

        if (string.IsNullOrEmpty(clientDbVersion))
        {
          log.Error(new ArgumentNullException("clientDbVersion"), "{MethodName}- {@Request}", methodName, sourceRequest);
          return false;
        }

        SetBranchRunningDBVersion.Execute(cache, server.BranchServerId, clientDbVersion);

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

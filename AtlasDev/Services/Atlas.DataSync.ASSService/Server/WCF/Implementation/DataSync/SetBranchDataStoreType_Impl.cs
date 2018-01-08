using System;
using System.Collections.Generic;

using Atlas.DataSync.WCF.Interface;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;
using ASSServer.Utils.PSQL.DbfImport;
using ASSServer.DbUtils;


namespace ASSServer.WCF.Implementation.DataSync
{  
  public static class SetBranchDataStoreType_Impl
  {
    public static bool Execute(ILogging log, ICacheServer cache, IConfigSettings config, SourceRequest sourceRequest, bool sql)
    {
      var methodName = "SetBranchDataStoreType";
     
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
        var branch = cache.Get<BRN_Branch_Cached>(server.Branch.Value);

        var processInfo = new List<string>();
        var result = MarkBranchAsPSQL.Execute(cache, branch.BranchId, processInfo);
        foreach (var item in processInfo)
        {
          log.Error(item);
        }

        return result;
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

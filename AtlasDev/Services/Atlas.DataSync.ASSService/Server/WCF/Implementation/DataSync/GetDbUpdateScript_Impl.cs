using System;
using System.Collections.Generic;

using ASSServer.DbUtils;
using ASSServer.Utils.PSQL;
using Atlas.DataSync.WCF.Interface;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;


namespace ASSServer.WCF.Implementation.DataSync
{
  public static class GetDbUpdateScript_Impl
  {
    public static List<VerUpdateScripts> Execute(ILogging log, ICacheServer cache, IConfigSettings config, SourceRequest sourceRequest, string clientDbVersion)
    {
      var methodName = "GetDbUpdateScript";

      var result = new List<VerUpdateScripts>();
      try
      {
        ASS_BranchServer_Cached server;
        string errorMessage;
        if (!Checks.VerifyBranchServerRequest(log, sourceRequest, out server, out errorMessage))
        {
          log.Warning(new Exception(errorMessage), "{MethodName}- {@Request} ", methodName, sourceRequest);
          return null;
        }

        if (server.UseDBVersion == 0 || server.RunningDBVersion == 0)
        {
          return null;
        }

        GetBranchDbUpdateScript.Execute(cache, clientDbVersion, out result, out errorMessage);
        return result;
      }
      catch (Exception err)
      {
        log.Error(err, "{MethodName}- {@Request}", methodName, sourceRequest);
        DbRepos.LogASSBranchServerEvent(0, DateTime.Now, methodName, err.Message, 5);
        return null;
      }
    }
  }
}

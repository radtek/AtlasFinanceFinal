using System;
using System.Linq;
using System.Collections.Generic;

using Atlas.DataSync.WCF.Interface;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;
using ASSServer.DbUtils;


namespace ASSServer.WCF.Implementation.DataSync
{
  
  public static class LogEvents_Impl
  {
    public static void Execute(ILogging log, ICacheServer cache, IConfigSettings config, SourceRequest sourceRequest, List<LogEvent> events)
    {      
      var methodName = "LogEvents";
      try
      {
        if (events == null)
        {
          return;
        }
     
        log.Warning("{MethodName}- {@Request}: {@Events}", methodName, sourceRequest, events);

        ASS_BranchServer_Cached server;
        string checkErrorMessage;
        if (!Checks.VerifyBranchServerRequest(log, sourceRequest, out server, out checkErrorMessage))
        {
          log.Warning(new Exception(checkErrorMessage), "{MethodName}- {@Request}", methodName, sourceRequest);
          return;
        }

        if (events == null || events.Count == 0)
        {
          log.Error(methodName, new ArgumentNullException("events"));
          return;
        }

        foreach (var logEvent in events)
        {
          DbRepos.LogASSBranchServerEvent(server.BranchServerId, logEvent.RaisedDT, logEvent.Task, logEvent.EventMessage, logEvent.Severity);
        }
      }
      catch (Exception err)
      {
        log.Error(err, "{MethodName}- {@Request}", methodName, sourceRequest);
      }
    }

  }
}

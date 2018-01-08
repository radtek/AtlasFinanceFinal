using System;
using System.Linq;
using System.Collections.Generic;

using Atlas.DataSync.WCF.Interface;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;


namespace ASSServer.WCF.Implementation.DataSync
{
  public static class UploadCurrentBranchRecId_Impl
  {
    public static void Execute(ILogging log, ICacheServer cache, IConfigSettings config, SourceRequest sourceRequest, Int64 recId)
    {
      var methodName = "UploadCurrentBranchRecId";
      var pos = "Checking server";
      try
      {
        #region Check parameters
        ASS_BranchServer_Cached server;
        string errorMessage;
        if (!Checks.VerifyBranchServerRequest(log, sourceRequest, out server, out errorMessage))
        {
          log.Warning(new Exception(errorMessage), "{MethodName}- {@Request}", methodName, sourceRequest);
          return;
        }

        if (recId < 1)
        {
          log.Error("{MethodName} called with invalid recId: {RecId}", methodName, recId);
          return;
        }
        #endregion
        
        if (recId != server.ClientClientCurrentRecId)
        {
          if (recId < server.ClientClientCurrentRecId)
          {
            log.Error("Server {@Server} RecId is now lower than previous value: {OldRecId}, {NewRecId}", server.Branch, server.ClientClientCurrentRecId, recId);
          }
          if (Math.Abs(recId - server.LastProcessedClientRecId) > 2000)
          {
            log.Warning("Server {@Server} is falling behind: Central: {CentralRecId}, Branch: {BranchRecId}", server.Branch, server.LastProcessedClientRecId, recId);
          }

          pos = "Setting recid";
          server.ClientClientCurrentRecId = recId;
          pos = "Setting cache";
          cache.Set(new List<ASS_BranchServer_Cached> { server });
        }
      }
      catch (Exception err)
      {
        log.Error(err, "{MethodName}- {@Request} @ {Pos}", methodName, sourceRequest, pos);
      }
    }
  }

}

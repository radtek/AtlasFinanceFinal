using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using Atlas.DataSync.WCF.Interface;
using Atlas.Common.Utils;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;
using ASSServer.DbUtils;
using ASSServer.WCF.Implementation.DataSync.Utils;
using Atlas.Cache.DataUtils;


namespace ASSServer.WCF.Implementation.DataSync
{
  internal static class UploadBranchRowChanges_Impl
  {
    internal static bool Execute(ILogging log, ICacheServer cache, IConfigSettings config,
      SourceRequest sourceRequest, string clientDbVersion, Int64 lastClientRecId, byte[] binDataSet)
    {
      var process = System.Diagnostics.Stopwatch.StartNew();
      var methodName = "UploadBranchRowChanges";
    
      try
      {        
        ASS_BranchServer_Cached server;
        string errorMessage;
        if (!Checks.VerifyBranchServerRequest(log, sourceRequest, out server, out errorMessage))
        {
          log.Warning(new Exception(errorMessage), "{MethodName}- {@Request}", methodName, sourceRequest);
          return false;
        }        
        var branch = cache.Get<BRN_Branch_Cached>(server.Branch.Value);        
        var legacyBranchNum = branch.LegacyBranchNum.PadLeft(3, '0');
                
        DataSet dataSet = null;
        if (binDataSet != null && binDataSet.Length > 0)
        {
          var timer = System.Diagnostics.Stopwatch.StartNew();
          dataSet = (!string.IsNullOrEmpty(sourceRequest.AppVer) && string.Compare(sourceRequest.AppVer, "1.2.0.0") >= 0) ?
            ASSServer.Utils.Serialization.FastJsonSerializer.DeserializeFromBytesJson<DataSet>(binDataSet, true) :
            (DataSet)Serialization.DeserializeFromBytes(binDataSet, true);
          timer.Stop();
          log.Information("Deserialized table in {timer}ms", timer.ElapsedMilliseconds);

          if (dataSet == null || dataSet.Tables == null || dataSet.Tables.Count == 0)
          {
            log.Error(new ArgumentNullException("dataSet"), methodName);
            return false;
          }
          log.Information("{MethodName}- Tables: {Tables}, Binary size: {Size}", methodName, dataSet.Tables.Count, binDataSet.Length);
        }

        if (string.IsNullOrEmpty(clientDbVersion) || !Regex.IsMatch(clientDbVersion, "^[0-9A-Z]{5}$"))
        {
          log.Error(new ArgumentOutOfRangeException("clientDbVersion", string.Format("Value: '{0}'", clientDbVersion)), methodName);
          return false;
        }

        var serverDbVersion = CacheUtils.GetCurrentDbVersion(cache);
        if (serverDbVersion.DBVersion != clientDbVersion)
        {
          log.Error(new Exception(string.Format("Cannot sync due to DB mismatch: Client DB version: {0}, Server DB version: {1}", clientDbVersion, serverDbVersion)),
            "{@Request}", sourceRequest);
          return false;
        }

        if (dataSet != null)
        {
          var dataTimer = System.Diagnostics.Stopwatch.StartNew();
          var schemaName = string.Format("br{0}", legacyBranchNum.ToLower().PadLeft(3, '0'));
          if (!DatasetToSQL.Execute(log, cache, config, schemaName, dataSet, false, legacyBranchNum, out errorMessage))
          {
            log.Error(new Exception(errorMessage), "{Branch}- DatasetToSQL for branch failed", legacyBranchNum);
            return false;
          }
          dataTimer.Stop();
          log.Information("DatasetToSQL.Execute (brxxx) took {elapsed}ms", dataTimer.ElapsedMilliseconds);

          dataTimer.Start();
          if (!DatasetToSQL.Execute(log, cache, config, "company", dataSet, true, legacyBranchNum, out errorMessage))
          {
            log.Error(new Exception(errorMessage), "{Branch} DatasetToSQL for company failed", legacyBranchNum);
            return false;
          }
          dataTimer.Stop();
          log.Information("DatasetToSQL.Execute (company) took {elapsed}ms", dataTimer.ElapsedMilliseconds);
        }

        if (lastClientRecId > 0 && lastClientRecId > server.LastProcessedClientRecId)
        {
          server.LastProcessedClientRecId = lastClientRecId;
        }
        else if (lastClientRecId > 0 && lastClientRecId < server.LastProcessedClientRecId)
        {
          log.Error("{MethodName}- LastProcessedClientRecId mismatch. Client: {ClientLastProcessedClientRecId}, Server: {ServerLastProcessedClientRecId}",
            methodName, lastClientRecId, server.LastProcessedClientRecId);
        }

        server.LastSyncDT = DateTime.Now;
                
        var foundVer = cache.GetAll<ASS_DbUpdateScript_VerString_Cached>()?.FirstOrDefault(s => s.DBVersion == clientDbVersion);
        if (foundVer != null)
        {
          server.RunningDBVersion = foundVer.DbUpdateScriptId;
        }        
        cache.Set(new List<ASS_BranchServer_Cached> { server });
        
        process.Stop();
        if (binDataSet != null)
        {
          log.Information("{MethodName}- Took {process}ms", methodName, process.ElapsedMilliseconds);
        }
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

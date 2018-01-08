using System;
using System.ServiceModel;
using System.Collections.Generic;

using Atlas.DataSync.WCF.Interface;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Cache.DataUtils;


namespace ASSServer.WCF.Implementation.DataSync
{
  /// <summary>
  /// Implementation of ASS Data Sync services
  /// </summary>
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class DataSyncServer : IDataSyncServer
  {
    public DataSyncServer(ILogging log, IConfigSettings config, ICacheServer cache)
    {
      _log = log;
      _config = config;
      _cache = cache;
    }


    /// <summary>
    /// Gets the PostgreSQL DDL scripts to be run on the client, to update the branch SQL system to match the latest DB version
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <param name="dbLocalVersion">Branch version</param>
    /// <returns>List of scripts to execute to update the branch PostgreSQL</returns>
    public List<VerUpdateScripts> GetDbUpdateScript(SourceRequest sourceRequest, string clientDbVersion)
    {
      return GetDbUpdateScript_Impl.Execute(_log, _cache, _config, sourceRequest, clientDbVersion);
    }


    /// <summary>
    /// Get settings for this branch server- usually SQL username, password, etc.
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <returns>List of branch settings to use</returns>
    public List<KeyValueItem> GetBranchSettings(SourceRequest sourceRequest)
    {
      return GetBranchSettings_Impl.Execute(_log, _cache, _config, sourceRequest);
    }


    /// <summary>
    /// Upload row changes to central company and branch schema- uses binary serialized dataset
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <param name="clientDbVersion">The branch DB version</param>
    /// <param name="lastClientRecId">Last client record change tracking recId in the dataset</param>
    /// <param name="binDataSet">The records changed- each DataTable is a table containing full details of records changed</param>
    /// <returns>true if successfully update central PSQL</returns>
    public bool UploadBranchRowChanges(SourceRequest sourceRequest, string clientDbVersion, Int64 lastClientRecId, byte[] binDataSet)
    {
      return UploadBranchRowChanges_Impl.Execute(_log, _cache, _config, sourceRequest, clientDbVersion, lastClientRecId, binDataSet);
    }


    /// <summary>
    /// Branch ASS Server- Gets any 'master' table lookup changes, since client last processed
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <param name="lastClientRecIdProcessed"></param>
    /// <param name="serverLastRecId">The last RecId processed in this DataSet</param>
    /// <param name="clientDbVersion">The client's current ASS DB version</param>
    /// <returns>byte array of a compressed, binary serialized DataSet</returns>

    public MasterTableRowChanges GetMasterRowChangesSince(SourceRequest sourceRequest, string clientDbVersion, Int64 lastClientRecIdProcessed)
    {
      return GetMasterRowChangesSince_Impl.Execute(_log, _cache, _config, sourceRequest, clientDbVersion, lastClientRecIdProcessed);
    }


    /// <summary>
    /// Simple Ping- is this machine authorised to make use of these services?
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <returns></returns>
    public PingResult Ping(SourceRequest sourceRequest)
    {
      return Ping_Impl.Execute(_log, _cache, _config, sourceRequest);
    }


    /// <summary>
    /// Returns the last successfully processed branch recid value (set by UploadBranchRowChanges successful completion) for the branch
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <returns>Listing of SR_RECNO values for this branch, for each table</returns>
    public Int64 BranchLastRecId(SourceRequest sourceRequest)
    {
      return BranchLastRecId_Impl.Execute(_log, _cache, _config, sourceRequest);
    }


    /// <summary>
    /// Indicates to server the lrep_tracking recid, for the server to be able to determine how far behind
    /// the branch is running.
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <param name="recId"></param>
    public void UploadCurrentBranchRecId(SourceRequest sourceRequest, Int64 recId)
    {
      UploadCurrentBranchRecId_Impl.Execute(_log, _cache, _config, sourceRequest, recId);
    }


    /// <summary>
    /// Returns listing of master tables, which are managed centrally- client needs this, so as not to bother sending changes on these tables
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <returns></returns>
    public List<string> GetMasterTableNames(SourceRequest sourceRequest)
    {
      return GetMasterTableNames_Impl.Execute(_log, _cache, _config, sourceRequest);
    }


    /// <summary>
    /// Marks branch as SQL or DBASE, on successful conversion
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <param name="sql"></param>
    /// <returns></returns>
    public bool SetBranchDataStoreType(SourceRequest sourceRequest, bool sql)
    {
      return SetBranchDataStoreType_Impl.Execute(_log, _cache, _config, sourceRequest, sql);
    }


    /// <summary>
    /// Used by branch when restoring a PostgreSQL dump, that the sync ID's need to restarted
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <returns></returns>
    public bool ResetLastClientSyncId(SourceRequest sourceRequest)
    {
      return ResetLastClientSyncId_Impl.Execute(_log, _cache, _config, sourceRequest);
    }


    /// <summary>
    /// Client indicates they have updated their db version- update their ASS replication info table and XPO server table
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <param name="clientDbVersion"></param>
    /// <returns></returns>
    public bool SetBranchDatabaseVersion(SourceRequest sourceRequest, string clientDbVersion)
    {
      return SetBranchDatabaseVersion_Impl.Execute(_log, _cache, _config, sourceRequest, clientDbVersion);
    }


    /// <summary>
    /// ASS Branch server- Logs a client error
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <param name="events"></param>
    public void LogEvents(SourceRequest sourceRequest, List<LogEvent> events)
    {
      LogEvents_Impl.Execute(_log, _cache, _config, sourceRequest, events);
    }


    /// <summary>
    /// Get the IP for the branch server.
    /// Used to determine the backup 
    /// </summary>
    /// <param name="branchCode"></param>
    /// <returns>The branch server's IP4 address: 192.168.x.x</returns>
    public string GetBranchServerIP(string branchCode)
    {
      return GetBranchServerIP_Impl.Execute(_log, _cache, branchCode);
    }


    /// <summary>
    /// Returns server date/time
    /// </summary>
    /// <returns>THe current UTC -2 date/time </returns>
    public DateTime GetServerDateTime()
    {
      return DateTime.Now;
    }


    /// <summary>
    /// Returns the current database version the system is currently on.
    /// Allows the client to determine if database update scripts need to be downloaded and processed on the branch DB.
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <returns>The current version of the server database system</returns>
    public string GetServerDatabaseVersion(SourceRequest sourceRequest)
    {
      var methodName = "GetServerDatabaseVersion";

      try
      {
        return CacheUtils.GetCurrentDbVersion(_cache)?.DBVersion;
      }
      catch (Exception err)
      {
        _log.Error(err, "{MethodName}- {@Request}", methodName, sourceRequest);
        return null;
      }
    }


    /// <summary>
    /// Returns the current value for the server's master table change tracking recid (ASS_MasterTableChangeTracking.RecId).
    /// Allows the client to determine if there are any master table rows to be downloaded and processed into the branch DB.
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <returns>The maximum value for ASS_MasterTableChangeTracking.RecId</returns>
    public Int64 GetServerMasterSyncId(SourceRequest sourceRequest)
    {
      return GetServerMasterSyncId_Impl.Execute(_log, sourceRequest);
    }



    #region Private fields

    private readonly ILogging _log;
    private readonly IConfigSettings _config;
    private readonly ICacheServer _cache;

    #endregion



    public long BranchLastAuditRecId(SourceRequest sourceRequest)
    {
      return BranchLastAuditRecId_Impl.Execute(_log, _cache, _config, sourceRequest);
    }


    public bool UploadBranchAuditChanges(SourceRequest sourceRequest, string clientDbVersion, long lastClientAuditRecId, List<lrep_audit> audit)
    {
      return UploadBranchAuditChanges_Impl.Execute(_log, _cache, _config, sourceRequest, clientDbVersion, lastClientAuditRecId, audit);
    }
  }

}

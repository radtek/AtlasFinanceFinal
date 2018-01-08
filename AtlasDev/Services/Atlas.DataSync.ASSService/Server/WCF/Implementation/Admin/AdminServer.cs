using System;
using System.ServiceModel;

using Atlas.DataSync.WCF.Interface;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;


namespace ASSServer.WCF.Implementation.Admin
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class AdminServer : IAdminServer
  {
    public AdminServer(ILogging log, IConfigSettings config,ICacheServer cache)
    {
      _log = log;
      _config = config;
      _cache = cache;
    }


    /// <summary>
    /// Returns serialized DataTable for masterTableName- used by Master GUI for editing master tables
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <returns>Binary, compressed serialized DataTable</returns>    
    public byte[] GetMasterTableData(SourceRequest sourceRequest, string masterTableName)
    {
      return GetMasterTableData_Impl.Execute(_log, _cache, _config, sourceRequest, masterTableName);
    }


    /// <summary>
    /// Updates/inserts records in specified master table- Used by Master GUI for upload changed rows
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <param name="dataTable">Serialized DataTable</param>
    /// <param name="masterTableName">Table to update</param>
    /// <returns></returns>    
    public bool UploadMasterTableRow(SourceRequest sourceRequest, string masterTableName, byte[] dataTable)
    {
      return UploadMasterTableRow_Impl.Execute(_log, _cache, _config, sourceRequest, masterTableName, dataTable);
    }


    /// <summary>
    /// This is to be  called after xHarbour uploads a table
    /// </summary>
    /// <param name="tableName">Name of the table which was uploaded</param>
    /// <returns></returns>

    public bool XHarbourConvertedAMasterTable(SourceRequest sourceRequest, string tableName)
    {
      return XHarbourConvertedAMasterTable_Impl.Execute(_log, _cache, _config, sourceRequest, tableName);
    }


    /// <summary>
    /// New database version- apply to server and record
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <param name="version">The new database version</param>
    /// <param name="description">The update description</param>
    /// <param name="sqlUpdateScript">The PostgreSQL script (schema-less), which must be run to upgrade the database</param>
    /// <returns>true if successfully updated</returns>    
    public bool UpgradeDatabase(SourceRequest sourceRequest, string version, string description, string sqlUpdateScript)
    {
      return UpgradeDatabase_Impl.Execute(_log, _cache, _config, sourceRequest, version, description, sqlUpdateScript);
    }


    public void UpdatedBranchServer(SourceRequest sourceRequest, string legacyBranchNum)
    {
      UpdatedBranchServer_Impl.Execute(_log, _cache, sourceRequest, legacyBranchNum);
    }


    #region Private vars

    private readonly ILogging _log;
    private readonly IConfigSettings _config;
    private readonly ICacheServer _cache;

    #endregion

  }
}

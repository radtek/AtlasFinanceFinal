/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    WCF Data synchronization services for ASS data- Interface
 *   
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *  
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;


namespace Atlas.DataSync.WCF.Interface
{
  [ServiceContract(Namespace = "urn:Atlas/ASS/DataSync/Data/2013/08")]
  public interface IDataSyncServer
  {
    #region Routines for branch, for use with the initial dBASE upload/conversion process

    [OperationContract]
    bool ResetLastClientSyncId(SourceRequest sourceRequest);

    [OperationContract]
    bool SetBranchDataStoreType(SourceRequest sourceRequest, bool sql);

    [OperationContract]
    bool SetBranchDatabaseVersion(SourceRequest sourceRequest, string clientDbVersion);

    [OperationContract]
    Int64 GetServerMasterSyncId(SourceRequest sourceRequest);

    [OperationContract]
    void UploadCurrentBranchRecId(SourceRequest sourceRequest, Int64 recId);
    
    #endregion


    #region General sync routines

    [OperationContract]
    List<VerUpdateScripts> GetDbUpdateScript(SourceRequest sourceRequest, string clientDbVersion);

    [OperationContract]
    List<KeyValueItem> GetBranchSettings(SourceRequest sourceRequest);

    [OperationContract]
    PingResult Ping(SourceRequest sourceRequest);

    [OperationContract]
    List<string> GetMasterTableNames(SourceRequest sourceRequest);

    [OperationContract]
    string GetServerDatabaseVersion(SourceRequest sourceRequest);

    #endregion
    

    #region General DataSet based branch data sync calls

    // The client must ensure the serialized DataSet does not exceed 5MB! In cases of prolonged
    // downtime, care must be taken to keep the upload size below 5MB...
    [OperationContract]
    bool UploadBranchRowChanges(SourceRequest sourceRequest, string clientDbVersion, Int64 lastClientRecId, byte[] binDataSet);

    [OperationContract]
    MasterTableRowChanges GetMasterRowChangesSince(SourceRequest sourceRequest, string clientDbVersion, Int64 lastClientRecIdProcessed);
        
    [OperationContract]
    Int64 BranchLastRecId(SourceRequest sourceRequest);
      
    #endregion


    #region General functionality

    [OperationContract]
    void LogEvents(SourceRequest sourceRequest, List<LogEvent> events);

    [OperationContract]
    string GetBranchServerIP(string branchCode);

    [OperationContract]
    DateTime GetServerDateTime();
    
    #endregion

        
    #region Audit sync

    [OperationContract]
    Int64 BranchLastAuditRecId(SourceRequest sourceRequest);

    [OperationContract]
    bool UploadBranchAuditChanges(SourceRequest sourceRequest, string clientDbVersion, Int64 lastClientAuditRecId, List<lrep_audit> audit);
                  
    #endregion
  }

  /// <summary>
  /// Scripts required to update to each version
  /// </summary>
  [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ASSServer.WCF.Interface.DataSync")]
  public class VerUpdateScripts
  {
    [DataMember]
    public string Version { get; set; }
    [DataMember]
    public string SQLScript { get; set; }
  }


  /// <summary>
  /// Key/value class- not named KeyValuePair to avoid any chance of a conflict...
  /// </summary>
  [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ASSServer.WCF.Interface.DataSync")]
  public class KeyValueItem
  {
    [DataMember]
    public string Key { get; set; }
    [DataMember]
    public string Value { get; set; }
  }


  [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ASSServer.WCF.Interface.DataSync")]
  public class TableRecId
  {
    [DataMember]
    public string TableName { get; set; }
    [DataMember]
    public Int64 ClientRecId { get; set; }
  }


  /// <summary>
  /// Class to return information to client
  /// </summary>
  [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ASSServer.WCF.Interface.DataSync")]
  public class PingResult
  {
    /// <summary>
    /// Is this machine authorised to use ASS data sync services?
    /// </summary>
    [DataMember]
    public bool MachineAuthorised { get; set; }

    /// <summary>
    /// Any error message for the client, to assist with troubleshooting
    /// </summary>
    [DataMember]
    public string ErrorMessage { get; set; }
  }


  [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ASSServer.WCF.Interface.DataSync")]
  public class LogEvent
  {
    [DataMember]
    public DateTime RaisedDT { get; set; }
    [DataMember]
    public string Task { get; set; }
    [DataMember]
    public string EventMessage { get; set; }
    [DataMember]
    public int Severity { get; set; }
  }


  [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ASSServer.WCF.Interface.DataSync")]
  public class MasterTableRowChanges
  {
    [DataMember]
    public byte[] DataSet { get; set; }
    [DataMember]
    public Int64 ServerLastRecId { get; set; }
    [DataMember]
    public string ErrorMessage { get; set; }
  }

 
  [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ASSServer.WCF.Interface.DataSync")]
  public class TableSyncInfo
  {
    [DataMember]
    public string TableName { get; set; }
    [DataMember]
    public int SQLRDDRecId { get; set; }
  }

}
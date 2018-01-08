/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *   Authorises/monitors branch server machines for the Atlas ASS DB sync services
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
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;

using DevExpress.Xpo;

using Atlas.Domain.Security;



namespace Atlas.Domain.Model
{
  /// <summary>
  /// Authorises/monitors branch server machines for the Atlas ASS DB sync services
  /// </summary>
  public class ASS_BranchServer : XPCustomObject
  {   
    [Key(AutoGenerate = true)]
    public Int64 BranchServerId { get; set; }

    private BRN_Branch _Branch;
    [Persistent, Indexed]
    public BRN_Branch Branch
    {
      get { return _Branch; }
      set { SetPropertyValue("Branch", ref _Branch, value); }
    }

    private COR_Machine _Machine;
    [Persistent, Indexed]
    public COR_Machine Machine
    {
      get { return _Machine; }
      set { SetPropertyValue("Machine", ref _Machine, value); }
    }

    private bool _MachineAuthorised;
    [Persistent]
    public bool MachineAuthorised
    {
      get { return _MachineAuthorised; }
      set { SetPropertyValue("MachineAuthorised", ref _MachineAuthorised, value); }
    }

    private DateTime _UploadedDBDT;
    [Persistent]
    public DateTime UploadedDBDT
    {
      get { return _UploadedDBDT; }
      set { SetPropertyValue("UploadedDBDT", ref _UploadedDBDT, value); }
    }

    private DateTime _LastSyncDT;
    [Persistent]
    public DateTime LastSyncDT
    {
      get { return _LastSyncDT; }
      set { SetPropertyValue("LastSyncDT", ref _LastSyncDT, value); }
    }


    //private ASS_DbUpdateScript _RunningDBVersion;
    [Delayed(true)]
    [Persistent]
    public ASS_DbUpdateScript RunningDBVersion
    {
      get { return GetDelayedPropertyValue<ASS_DbUpdateScript>("RunningDBVersion"); }
      set { SetDelayedPropertyValue<ASS_DbUpdateScript>("RunningDBVersion", value); }
    }


    //private ASS_DbUpdateScript _UseDBVersion;
    [Persistent]
    [Delayed(true)]
    public ASS_DbUpdateScript UseDBVersion
    {
      get { return GetDelayedPropertyValue<ASS_DbUpdateScript>("UseDBVersion"); }
      set { SetDelayedPropertyValue<ASS_DbUpdateScript>("UseDBVersion", value); }
    }

    private string _LastError;
    [Persistent]
    public string LastError
    {
      get { return _LastError; }
      set { SetPropertyValue("LastError", ref _LastError, value); }
    }

    private Int64 _LastProcessedClientRecId;
    [Persistent]
    public Int64 LastProcessedClientRecId
    {
      get { return _LastProcessedClientRecId; }
      set { SetPropertyValue("LastProcessedClientRecId", ref _LastProcessedClientRecId, value); }
    }

    private Int64 _ClientCurrentRecId;
    [Persistent]
    public Int64 ClientClientCurrentRecId
    {
      get { return _ClientCurrentRecId; }
      set { SetPropertyValue("ClientCurrentRecId", ref _ClientCurrentRecId, value); }
    }

    private Int64 _ClientAuditCurrentRecId;
    [Persistent]
    public Int64 ClientAuditCurrentRecId
    {
      get { return _ClientAuditCurrentRecId; }
      set { SetPropertyValue("ClientAuditCurrentRecId", ref _ClientAuditCurrentRecId, value); }
    }

    private Int64 _LastProcessedClientAuditRecId;
    [Persistent]
    public Int64 LastProcessedClientAuditRecId
    {
      get { return _LastProcessedClientAuditRecId; }
      set { SetPropertyValue("LastProcessedClientAuditRecId", ref _LastProcessedClientAuditRecId, value); }
    }

    #region Constructors

    public ASS_BranchServer()
      : base()
    {
      // This constructor is used when an object is loaded from a persistent storage.
      // Do not place any code here.
    }

    public ASS_BranchServer(Session session)
      : base(session)
    {
      // This constructor is used when an object is loaded from a persistent storage.
      // Do not place any code here.
    }

    #endregion
  }

}
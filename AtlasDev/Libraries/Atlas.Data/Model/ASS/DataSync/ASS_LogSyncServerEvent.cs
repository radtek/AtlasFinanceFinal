/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *   Logs sync events- errors/warnings related to the sync process
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


namespace Atlas.Domain.Model
{
  /// <summary>
  /// Logs ASS Sync process errors (client and server)
  /// </summary>
  public class ASS_LogSyncServerEvent: XPCustomObject
  {    
    [Key(AutoGenerate = true)]
    public Int64 LogSyncServerErrId { get; set; }

    private DateTime? _RaisedDT;
    [Persistent, Indexed]
    public DateTime? RaisedDT
    {
      get { return _RaisedDT; }
      set { SetPropertyValue("RaisedDT", ref _RaisedDT, value); }
    }

    private ASS_BranchServer _Server;
    [Persistent, Indexed]
    public ASS_BranchServer Server
    {
      get { return _Server; }
      set { SetPropertyValue("Server", ref _Server, value); }
    }

    private string _Task;
    [Persistent, Size(100)]
    public string Task
    {
      get { return _Task; }
      set { SetPropertyValue("Task", ref _Task, value); }
    }

    private string _EventMesage;
    [Persistent, Size(10000)]
    public string EventMesage
    {
      get { return _EventMesage; }
      set { SetPropertyValue("EventMesage", ref _EventMesage, value); }
    }

    private int _Severity;
    [Persistent]
    public int Severity
    {
      get { return _Severity; }
      set { SetPropertyValue("Severity", ref _Severity, value); }
    }


    #region Constructors

    public ASS_LogSyncServerEvent()
      : base()
    {
      // This constructor is used when an object is loaded from a persistent storage.
      // Do not place any code here.
    }

    public ASS_LogSyncServerEvent(Session session)
      : base(session)
    {
      // This constructor is used when an object is loaded from a persistent storage.
      // Do not place any code here.
    }
    #endregion

  } 
}

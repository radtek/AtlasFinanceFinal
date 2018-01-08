
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for LogTCCTerminal
 * 
 * 
 *  Author:
 *  ------------------
 *     Fabian Franco-Roldan
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
using Atlas.Domain.Interface;

namespace Atlas.Domain.Model
{
  public class LogTCCTerminal : XPLiteObject
  {
    private Int64 _LogTCCTerminalId;
    [Key(AutoGenerate = true)]
    public Int64 LogTCCTerminalId
    {
      get { return _LogTCCTerminalId; }
      set { SetPropertyValue("LogTCCTerminalId", ref _LogTCCTerminalId, value); }
    }

    private TCCTerminal _Terminal;
    [Persistent("TCCTerminalId")]
    [Indexed(Name = "TCCTerminalId")]
    public TCCTerminal Terminal
    {
      get { return _Terminal; }
      set { SetPropertyValue("Terminal", ref _Terminal, value); }
    }    

    private DateTime _StartDT;
    [Persistent]
    [Indexed(Name = "IDX_STARTDT")]
    public DateTime StartDT
    {
      get { return _StartDT; }
      set { SetPropertyValue("StartDT", ref _StartDT, value); }
    }

    private DateTime _EndDT;
    [Persistent]
    public DateTime EndDT
    {
      get { return _EndDT; }
      set { SetPropertyValue("EndDT", ref _EndDT, value); }
    }

    private Enumerators.General.TCCLogRequestType _RequestType;
    [Persistent("RequestTypeId")]
    public Enumerators.General.TCCLogRequestType RequestType
    {
      get { return _RequestType; }
      set { SetPropertyValue("RequestType", ref _RequestType, value); }
    }
    
    private string _RequestParam;
    [Persistent, Size(int.MaxValue)]
    public string RequestParam
    {
      get { return _RequestParam; }
      set { SetPropertyValue("RequestParam", ref _RequestParam, value); }
    }

    private Enumerators.General.TCCLogRequestResult _ResultType;
    [Persistent]
    public Enumerators.General.TCCLogRequestResult ResultType
    {
      get  { return _ResultType; }
      set  { SetPropertyValue("ResultType", ref _ResultType, value); }
    }

    private string _MessageStore;
    [Persistent("ResultMessage"), Size(500)]
    private string MessageStore
    {
      get { return _MessageStore; }
      set { SetPropertyValue("MessageStore", ref _MessageStore, value); }
    }
    
    [NonPersistent]
    public string ResultMessage
    {
      get { return MessageStore; }
      set { MessageStore = !string.IsNullOrEmpty(value) && value.Length >= 500 ? value.Substring(0, 500) : value; }
    }

    private PER_Person _CreatedBy;
    [Persistent]
    public PER_Person CreatedBy
    {
      get { return _CreatedBy; }
      set { SetPropertyValue("CreatedBy", ref _CreatedBy, value); }
    }

    private DateTime? _CreatedDT;
    [Persistent]
    public DateTime? CreatedDT
    {
      get { return _CreatedDT; }
      set { SetPropertyValue("CreatedDT", ref _CreatedDT, value); }
    }

   
    #region Constructors

    public LogTCCTerminal() : base() { }
    public LogTCCTerminal(Session session) : base(session) { }

    #endregion
  }
}

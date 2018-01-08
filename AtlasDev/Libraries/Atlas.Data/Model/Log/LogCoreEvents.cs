/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for LogCoreEvents
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
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using Atlas.Domain.Interface;


namespace Atlas.Domain.Model
{
  public class LogCoreEvent : XPLiteObject
  {

    private Int64 _logCoreEventId;
    [Key(AutoGenerate = true)]
    public Int64 LogCoreEventId
    {
      get
      {
        return _logCoreEventId;
      }
      set
      {
        SetPropertyValue("LogCoreEventId", ref _logCoreEventId, value);
      }
    }

    private DateTime _eventDT;
    [Persistent]
    public DateTime EventDT
    {
      get
      {
        return _eventDT;
      }
      set
      {
        SetPropertyValue("EventDT", ref _eventDT, value);
      }
    }

    private string _thread;
    [Persistent, Size(255)]
    public string Thread
    {
      get
      {
        return _thread;
      }
      set
      {
        SetPropertyValue("Thread", ref _thread, value);
      }
    }

    private string _Level;
    [Persistent, Size(50)]
    public string Level
    {
      get
      {
        return _Level;
      }
      set
      {
        SetPropertyValue("Level", ref _Level, value);
      }
    }

    private Enumerators.General.ApplicationIdentifiers _appId;
    [Persistent]
    public Enumerators.General.ApplicationIdentifiers AppId
    {
      get
      {
        return _appId;
      }
      set
      {
        SetPropertyValue("AppId", ref _appId, value);
      }
    }

    private string _logger;
    [Persistent, Size(50)]
    public string Logger
    {
      get
      {
        return _logger;
      }
      set
      {
        SetPropertyValue("Logger", ref _logger, value);
      }
    }

    private string _message;
    [Persistent, Size(4000)]
    public string Message
    {
      get
      {
        return _message;
      }
      set
      {
        SetPropertyValue("Message", ref _message, value);
      }
    }

    private string _exception;
    [Persistent, Size(2000)]
    public string Exception
    {
      get
      {
        return _exception;
      }
      set
      {
        SetPropertyValue("Exception", ref _exception, value);
      }
    }

    private PER_Person _CreatedBy;
    [Persistent]
    public PER_Person CreatedBy
    {
      get
      {
        return _CreatedBy;
      }
      set
      {
        SetPropertyValue("CreatedBy", ref _CreatedBy, value);
      }
    }

    private DateTime? _CreatedDT;
    [Persistent]
    public DateTime? CreatedDT
    {
      get
      {
        return _CreatedDT;
      }
      set
      {
        SetPropertyValue("CreatedDT", ref _CreatedDT, value);
      }
    }

    #region Constructors

    public LogCoreEvent() : base() { }
    public LogCoreEvent(Session session) : base(session) { }

    #endregion

    public override void AfterConstruction()
    {
      base.AfterConstruction();
      // Place here your initialization code. 
    }
  }
}

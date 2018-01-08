
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for LogHWStatus
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
  [Indices("DeviceId", "DeviceId;HWStatus")]
  public class LogHWStatus : XPLiteObject
  {
    private Int64 _LogHWStatusId;
    [Key(AutoGenerate = true)]
    public Int64 LogHWStatusId
    {
      get
      {
        return _LogHWStatusId;
      }
      set
      {
        SetPropertyValue("LogHWStatusId", ref _LogHWStatusId, value);
      }
    }

    private Enumerators.General.DeviceType _DeviceType;
    [Persistent("DeviceTypeId")]
    public Enumerators.General.DeviceType DeviceType
    {
      get
      {
        return _DeviceType;
      }
      set
      {
        SetPropertyValue("DeviceType", ref _DeviceType, value);
      }
    }

    private Int64 _DeviceId;
    [Persistent]
    [Indexed(Name = "IDX_DEVICEID")]
    public Int64 DeviceId
    {
      get
      {
        return _DeviceId;
      }
      set
      {
        SetPropertyValue("DeviceId", ref _DeviceId, value);
      }
    }

    private DateTime _EventDT;
    [Persistent]
    public DateTime EventDT
    {
      get
      {
        return _EventDT;
      }
      set
      {
        SetPropertyValue("EventDT", ref _EventDT, value);
      }
    }

    private Enumerators.General.HWStatus _HWStatus;
    [Persistent("HWStatusId")]
    public Enumerators.General.HWStatus HWStatus
    {
      get
      {
        return _HWStatus;
      }
      set
      {
        SetPropertyValue("HWStatus", ref _HWStatus, value);
      }
    }

    private string _MessageStore;
    [Persistent("ResultMessage"), Size(500)]
    private string MessageStore
    {
      get
      {
        return _MessageStore;
      }
      set
      {
        SetPropertyValue("MessageStore", ref _MessageStore, value);
      }
    }

    [NonPersistent]
    public string ResultMessage
    {
      get
      {
        return MessageStore;
      }
      set
      {
        MessageStore = !string.IsNullOrEmpty(value) && value.Length >= 500 ? value.Substring(0, 500) : value;
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

    public LogHWStatus() : base() { }
    public LogHWStatus(Session session) : base(session) { }

    #endregion
  }
}

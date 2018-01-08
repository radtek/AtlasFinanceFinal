using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class PYT_ServiceSchedule : XPLiteObject
  {
    private int _serviceScheduleId;
    [Key(AutoGenerate=true)]
    public int ServiceScheduleId
    {
      get
      {
        return _serviceScheduleId;
      }
      set
      {
        SetPropertyValue("ServiceScheduleId", ref _serviceScheduleId, value);
      }
    }

    private PYT_Service _service;
    [Persistent("ServiceId")]
    public PYT_Service Service
    {
      get
      {
        return _service;
      }
      set
      {
        SetPropertyValue("Service", ref _service, value);
      }
    }

    private DateTime? _openTime;
    [Persistent]
    public DateTime? OpenTime
    {
      get
      {
        return _openTime;
      }
      set
      {
        SetPropertyValue("OpenTime", ref _openTime, value);
      }
    }

    private DateTime? _closeTime;
    [Persistent]
    public DateTime? CloseTime
    {
      get
      {
        return _closeTime;
      }
      set
      {
        SetPropertyValue("CloseTime", ref _closeTime, value);
      }
    }

    private bool _useSaturday;
    [Persistent]
    public bool UseSaturday
    {
      get
      {
        return _useSaturday;
      }
      set
      {
        SetPropertyValue("UseSaturday", ref _useSaturday, value);
      }
    }

    private bool _useSunday;
    [Persistent]
    public bool UseSunday
    {
      get
      {
        return _useSunday;
      }
      set
      {
        SetPropertyValue("UseSunday", ref _useSunday, value);
      }
    }

    #region Constructors

    public PYT_ServiceSchedule() : base() { }
    public PYT_ServiceSchedule(Session session) : base(session) { }

    #endregion
  }
}
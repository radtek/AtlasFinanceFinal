using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class AVS_ServiceSchedule : XPLiteObject
  {
    private Int32 _serviceScheduleId;
    [Key]
    public Int32 ServiceScheduleId
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

    private Host _host;
    [Persistent("HostId")]
    public Host Host
    {
      get
      {
        return _host;
      }
      set
      {
        SetPropertyValue("Host", ref _host, value);
      }
    }

    private AVS_Service _service;
    [Persistent("ServiceId")]
    public AVS_Service Service
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

    private AVS_ServiceType _serviceType;
    [Persistent("ServiceTypeId")]
    public AVS_ServiceType ServiceType
    {
      get
      {
        return _serviceType;
      }
      set
      {
        SetPropertyValue("ServiceType", ref _serviceType, value);
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

    public AVS_ServiceSchedule() : base() { }
    public AVS_ServiceSchedule(Session session) : base(session) { }

    #endregion
  }
}

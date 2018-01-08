using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public class DBT_HostService : XPLiteObject
  {
    private int _hostServiceId;
    [Key(AutoGenerate = true)]
    public int HostServiceId
    {
      get
      {
        return _hostServiceId;
      }
      set
      {
        SetPropertyValue("HostServiceId", ref _hostServiceId, value);
      }
    }

    private Host _host;
    [Persistent("HostId")]
    [Indexed]
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

    private DBT_Service _service;
    [Persistent("ServiceId")]
    [Indexed]
    public DBT_Service Service
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

    private bool _enabled;
    [Persistent]
    public bool Enabled
    {
      get
      {
        return _enabled;
      }
      set
      {
        SetPropertyValue("Enabled", ref _enabled, value);
      }
    }

    private DateTime? _disabledDate;
    [Persistent]
    public DateTime? DisabledDate
    {
      get
      {
        return _disabledDate;
      }
      set
      {
        SetPropertyValue("DisabledDate", ref _disabledDate, value);
      }
    }

    public DBT_HostService() : base() { }
    public DBT_HostService(Session session) : base(session) { }
  }
}
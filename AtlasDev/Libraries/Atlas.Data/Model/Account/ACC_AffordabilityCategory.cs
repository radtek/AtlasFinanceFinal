using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public sealed class ACC_AffordabilityCategory : XPLiteObject
  {
    private Int64 _affordabilityCategoryId;
    [Key(AutoGenerate = true)]
    public Int64 AffordabilityCategoryId
    {
      get
      {
        return _affordabilityCategoryId;
      }
      set
      {
        SetPropertyValue("AffordabilityCategoryId", ref _affordabilityCategoryId, value);
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

    private string _description;
    [Persistent, Size(20)]
    public string Description
    {
      get
      {
        return _description;
      }
      set
      {
        SetPropertyValue("Description", ref _description, value);
      }
    }

    private Enumerators.Account.AffordabilityCategoryType _affordabilityCategoryType;
    [Persistent("AffordabilityCategoryTypeId")]
    public Enumerators.Account.AffordabilityCategoryType AffordabilityCategoryType
    {
      get
      {
        return _affordabilityCategoryType;
      }
      set
      {
        SetPropertyValue("AffordabilityCategoryType", ref _affordabilityCategoryType, value);
      }
    }

    private int _ordinal;
    [Persistent("Ordinal")]
    public int Ordinal
    {
      get
      {
        return _ordinal;
      }
      set
      {
        SetPropertyValue("Ordinal", ref _ordinal, value);
      }
    }

    private bool _enabled;
    [Persistent("Enabled")]
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

    #region Constructors

    public ACC_AffordabilityCategory() : base() { }
    public ACC_AffordabilityCategory(Session session) : base(session) { }

    #endregion
  }
}

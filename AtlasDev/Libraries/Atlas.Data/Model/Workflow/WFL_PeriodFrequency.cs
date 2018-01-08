using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class WFL_PeriodFrequency : XPLiteObject
  {
    private int _periodFrequencyId;
    [Key]
    public int PeriodFrequencyId
    {
      get
      {
        return _periodFrequencyId;
      }
      set
      {
        SetPropertyValue("PeriodFrequencyId", ref _periodFrequencyId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Workflow.PeriodFrequency Type
    {
      get
      {
        return Name.FromStringToEnum<Enumerators.Workflow.PeriodFrequency>();
      }
      set
      {
        value = Name.FromStringToEnum<Enumerators.Workflow.PeriodFrequency>();
      }
    }

    private string _name;
    [Persistent, Size(10)]
    public string Name
    {
      get
      {
        return _name;
      }
      set
      {
        SetPropertyValue("Name", ref _name, value);
      }
    }

    #region Constructors

    public WFL_PeriodFrequency() : base() { }
    public WFL_PeriodFrequency(Session session) : base(session) { }

    #endregion
  }
}

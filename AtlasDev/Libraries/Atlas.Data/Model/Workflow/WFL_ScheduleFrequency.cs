using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class WFL_ScheduleFrequency : XPLiteObject
  {
    private int _scheduleFrequencyId;
    [Key]
    public int ScheduleFrequencyId
    {
      get
      {
        return _scheduleFrequencyId;
      }
      set
      {
        SetPropertyValue("ScheduleFrequencyId", ref _scheduleFrequencyId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Workflow.ScheduleFrequency Type
    {
      get
      {
        return Name.FromStringToEnum<Enumerators.Workflow.ScheduleFrequency>();
      }
      set
      {
        value = Name.FromStringToEnum<Enumerators.Workflow.ScheduleFrequency>();
      }
    }

    private string _name;
    [Persistent, Size(40)]
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

    public WFL_ScheduleFrequency() : base() { }
    public WFL_ScheduleFrequency(Session session) : base(session) { }

    #endregion
  }
}

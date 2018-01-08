using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class WFL_ScheduleProcessStatus : XPLiteObject
  {
    private int _scheduleProcessStatusId;
    [Key]
    public int ScheduleProcessStatusId
    {
      get
      {
        return _scheduleProcessStatusId;
      }
      set
      {
        SetPropertyValue("ScheduleProcessStatusId", ref _scheduleProcessStatusId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Workflow.ScheduleProcessStatus Status
    {
      get
      {
        return Name.FromStringToEnum<Enumerators.Workflow.ScheduleProcessStatus>();
      }
      set
      {
        value = Name.FromStringToEnum<Enumerators.Workflow.ScheduleProcessStatus>();
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

    public WFL_ScheduleProcessStatus() : base() { }
    public WFL_ScheduleProcessStatus(Session session) : base(session) { }

    #endregion
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class WFL_ScheduleProcessDay : XPLiteObject
  {
    private int _scheduleProcessDayId;
    [Key(AutoGenerate = true)]
    public int ScheduleProcessDayId
    {
      get
      {
        return _scheduleProcessDayId;
      }
      set
      {
        SetPropertyValue("ScheduleProcessDayId", ref _scheduleProcessDayId, value);
      }
    }



    private WFL_ScheduleProcess _scheduleProcess;
    [Indexed("Day", Unique = true)]
    [Persistent("ScheduleProcessId")]
    public WFL_ScheduleProcess ScheduleProcess
    {
      get
      {
        return _scheduleProcess;
      }
      set
      {
        SetPropertyValue("ScheduleProcess", ref _scheduleProcess, value);
      }
    }

    private WFL_BusinessDay _day;
    [Persistent("DayId")]
    public WFL_BusinessDay Day
    {
      get
      {
        return _day;
      }
      set
      {
        SetPropertyValue("Day", ref _day, value);
      }
    }

    #region Constructors

    public WFL_ScheduleProcessDay() : base() { }
    public WFL_ScheduleProcessDay(Session session) : base(session) { }

    #endregion
  }
}

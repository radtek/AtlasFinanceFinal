using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class WFL_ScheduleProcess : XPLiteObject
  {
    private int _scheduleProcessId;
    [Key(AutoGenerate = true)]
    public int ScheduleProcessId
    {
      get
      {
        return _scheduleProcessId;
      }
      set
      {
        SetPropertyValue("ScheduleProcessId", ref _scheduleProcessId, value);
      }
    }

    private WFL_Process _process;
    [Persistent("ProcessId")]
    public WFL_Process Process
    {
      get
      {
        return _process;
      }
      set
      {
        SetPropertyValue("Process", ref _process, value);
      }
    }

    private WFL_ScheduleFrequency _scheduleFrequency;
    [Persistent("ScheduleFrequencyId")]
    public WFL_ScheduleFrequency ScheduleFrequency
    {
      get
      {
        return _scheduleFrequency;
      }
      set
      {
        SetPropertyValue("ScheduleFrequency", ref _scheduleFrequency, value);
      }
    }

    private int _iteration;
    [Persistent]
    public int Iteration
    {
      get
      {
        return _iteration;
      }
      set
      {
        SetPropertyValue("Iteration", ref _iteration, value);
      }
    }

    private int _currentIteration;
    [Persistent]
    public int CurrentIteration
    {
      get
      {
        return _currentIteration;
      }
      set
      {
        SetPropertyValue("CurrentIteration", ref _currentIteration, value);
      }
    }

    private DateTime _start;
    [Persistent]
    public DateTime Start
    {
      get
      {
        return _start;
      }
      set
      {
        SetPropertyValue("Start", ref _start, value);
      }
    }

    private DateTime? _end;
    [Persistent]
    public DateTime? End
    {
      get
      {
        return _end;
      }
      set
      {
        SetPropertyValue("End", ref _end, value);
      }
    }

    private WFL_ScheduleProcessStatus _scheduleProcessStatus;
    [Persistent("ScheduleProcessStatusId")]
    public WFL_ScheduleProcessStatus ScheduleProcessStatus
    {
      get
      {
        return _scheduleProcessStatus;
      }
      set
      {
        SetPropertyValue("ScheduleProcessStatus", ref _scheduleProcessStatus, value);
      }
    }

    private DateTime? _lastExecutionDate;
    [Persistent]
    public DateTime? LastExecutionDate
    {
      get
      {
        return _lastExecutionDate;
      }
      set
      {
        SetPropertyValue("LastExecutionDate", ref _lastExecutionDate, value);
      }
    }

    private DateTime _nextExecutionDate;
    [Persistent]
    public DateTime NextExecutionDate
    {
      get
      {
        return _nextExecutionDate;
      }
      set
      {
        SetPropertyValue("NextExecutionDate", ref _nextExecutionDate, value);
      }
    }

    private string _lastResultMessage;
    [Persistent]
    public string LastResultMessage
    {
      get
      {
        return _lastResultMessage;
      }
      set
      {
        SetPropertyValue("LastResultMessage", ref _lastResultMessage, value);
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

    #region Constructors

    public WFL_ScheduleProcess() : base() { }
    public WFL_ScheduleProcess(Session session) : base(session) { }

    #endregion
  }
}

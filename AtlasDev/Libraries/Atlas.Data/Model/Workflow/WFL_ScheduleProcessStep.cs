using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class WFL_ScheduleProcessStep : XPLiteObject
  {
    private int _scheduleProcessStepId;
    [Key(AutoGenerate = true)]
    public int ScheduleProcessStepId
    {
      get
      {
        return _scheduleProcessStepId;
      }
      set
      {
        SetPropertyValue("ScheduleProcessStepId", ref _scheduleProcessStepId, value);
      }
    }

    private WFL_ScheduleProcess _scheduleProcess;
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

    private WFL_ProcessStep _processStep;
    [Persistent("ProcessStepId")]
    public WFL_ProcessStep ProcessStep
    {
      get
      {

        return _processStep;
      }
      set
      {
        SetPropertyValue("ProcessStep", ref _processStep, value);
      }
    }

    private WFL_PeriodFrequency _delayPeriodFrequency;
    [Persistent("DelayPeriodFrequencyId")]
    public WFL_PeriodFrequency DelayPeriodFrequency
    {
      get
      {

        return _delayPeriodFrequency;
      }
      set
      {
        SetPropertyValue("DelayPeriodFrequency", ref _delayPeriodFrequency, value);
      }
    }

    private int _delay;
    [Persistent]
    public int Delay
    {
      get
      {

        return _delay;
      }
      set
      {
        SetPropertyValue("Delay", ref _delay, value);
      }
    }

    private bool _useDecisionGate;
    [Persistent]
    public bool UseDecisionGate
    {
      get
      {

        return _useDecisionGate;
      }
      set
      {
        SetPropertyValue("UseDecisionGate", ref _useDecisionGate, value);
      }
    }

    private int _rank;
    [Persistent]
    public int Rank
    {
      get
      {

        return _rank;
      }
      set
      {
        SetPropertyValue("Rank", ref _rank, value);
      }
    }

    #region Constructors

    public WFL_ScheduleProcessStep() : base() { }
    public WFL_ScheduleProcessStep(Session session) : base(session) { }

    #endregion
  }
}

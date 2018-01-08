
using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Extensions;
using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  public class WFL_ProcessStep : XPLiteObject
  {
    private int _processStepId;
    [Key]
    public int ProcessStepId
    {
      get
      {
        return _processStepId;
      }
      set
      {
        SetPropertyValue("ProcessStepId", ref _processStepId, value);
      }
    }

    private WFL_Process _process;
    [Persistent("ProcessId")]
    [Association]
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

    private WFL_Trigger _triggerId;
    [Persistent("TriggerId")]
    public WFL_Trigger Trigger
    {
      get
      {
        return _triggerId;
      }
      set
      {
        SetPropertyValue("Trigger", ref _triggerId, value);
      }
    }

    private string _name;
    [Persistent, Size(50)]
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

    private string _namespace;
    [Persistent, Size(100)]
    public string Namespace
    {
      get
      {
        return _namespace;
      }
      set
      {
        SetPropertyValue("Namespace", ref _namespace, value);
      }
    }

    private bool _locked;
    [Persistent]
    public bool Locked
    {
      get
      {
        return _locked;
      }
      set
      {
        SetPropertyValue("Locked", ref _locked, value);
      }
    }

    private bool _jumpable;
    [Persistent]
    public bool Jumpable
    {
      get
      {
        return _jumpable;
      }
      set
      {
        SetPropertyValue("Jumpable", ref _jumpable, value);
      }
    }

    private WFL_PeriodFrequency _thresholdPeriodFrequency;
    [Persistent("ThresholdPeriodFrequencyId")]
    public WFL_PeriodFrequency ThresholdPeriodFrequency
    {
      get
      {

        return _thresholdPeriodFrequency;
      }
      set
      {
        SetPropertyValue("ThresholdPeriodFrequency", ref _thresholdPeriodFrequency, value);
      }
    }

    private int _threshold;
    [Persistent]
    public int Threshold
    {
      get
      {

        return _threshold;
      }
      set
      {
        SetPropertyValue("Threshold", ref _threshold, value);
      }
    }

    private bool _isParallelStep;
    [Persistent]
    public bool IsParallelStep
    {
      get
      {
        return _isParallelStep;
      }
      set
      {
        SetPropertyValue("IsParallelStep", ref _isParallelStep, value);
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

    private DateTime? _disableDate;
    [Persistent]
    public DateTime? DisableDate
    {
      get
      {
        return _disableDate;
      }
      set
      {
        SetPropertyValue("DisableDate", ref _disableDate, value);
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

    public WFL_ProcessStep() : base() { }
    public WFL_ProcessStep(Session session) : base(session) { }

    #endregion
  }
}
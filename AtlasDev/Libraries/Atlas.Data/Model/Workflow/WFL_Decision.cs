using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class WFL_Decision : XPLiteObject
  {
    private int _decisionId;
    [Key(AutoGenerate = true)]
    public int DecisionId
    {
      get
      {
        return _decisionId;
      }
      set
      {
        SetPropertyValue("DecisionId", ref _decisionId, value);
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

    private WFL_ConditionGroup _conditionGroup;
    [Persistent("ConditionGroupId")]
    public WFL_ConditionGroup ConditionGroup
    {
      get
      {
        return _conditionGroup;
      }
      set
      {
        SetPropertyValue("ConditionGroup", ref _conditionGroup, value);
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

    public WFL_Decision() : base() { }
    public WFL_Decision(Session session) : base(session) { }

    #endregion
  }
}

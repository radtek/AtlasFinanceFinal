using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class WFL_ConditionGroupResult : XPLiteObject
  {
    private long _conditionGroupResultId;
    [Key(AutoGenerate = true)]
    public long ConditionGroupResultId
    {
      get
      {
        return _conditionGroupResultId;
      }
      set
      {
        SetPropertyValue("ConditionGroupResultId", ref _conditionGroupResultId, value);
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

    private bool _result;
    [Persistent]
    public bool Result
    {
      get
      {
        return _result;
      }
      set
      {
        SetPropertyValue("Result", ref _result, value);
      }
    }

    private WFL_ConditionGroup _nextConditionGroup;
    [Persistent("NextConditionGroupId")]
    public WFL_ConditionGroup NextConditionGroup
    {
      get
      {
        return _nextConditionGroup;
      }
      set
      {
        SetPropertyValue("NextConditionGroup", ref _nextConditionGroup, value);
      }
    }

    private WFL_ProcessStep _outcomeProcessStep;
    [Persistent("OutcomeProcessStepId")]
    public WFL_ProcessStep OutcomeProcessStep
    {
      get
      {
        return _outcomeProcessStep;
      }
      set
      {
        SetPropertyValue("OutcomeProcessStep", ref _outcomeProcessStep, value);
      }
    }

    #region Constructors

    public WFL_ConditionGroupResult() : base() { }
    public WFL_ConditionGroupResult(Session session) : base(session) { }

    #endregion
  }
}

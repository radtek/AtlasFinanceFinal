
using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Extensions;
using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  public class WFL_Condition : XPLiteObject
  {
    private long _conditionId;
    [Key(AutoGenerate = true)]
    public long ConditionId
    {
      get
      {
        return _conditionId;
      }
      set
      {
        SetPropertyValue("ConditionId", ref _conditionId, value);
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

    private WFL_ConditionClassProperty _conditionClassProperty;
    [Persistent("ConditionClassPropertyId")]
    public WFL_ConditionClassProperty ConditionClassProperty
    {
      get
      {
        return _conditionClassProperty;
      }
      set
      {
        SetPropertyValue("ConditionClassProperty", ref _conditionClassProperty, value);
      }
    }

    private string _conditionValue;
    [Persistent]    
    public string ConditionValue
    {
        get
        {
            return _conditionValue;
        }
        set
        {
            SetPropertyValue("ConditionValue", ref _conditionValue, value);
        }
    }

    #region Constructors

    public WFL_Condition() : base() { }
    public WFL_Condition(Session session) : base(session) { }

    #endregion
  }
}

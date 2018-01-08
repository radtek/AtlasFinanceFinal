using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class WFL_ConditionGroup : XPLiteObject
  {
    private long _conditionGroupId;
    [Key(AutoGenerate = true)]
    public long ConditionGroupId
    {
      get
      {
        return _conditionGroupId;
      }
      set
      {
        SetPropertyValue("ConditionGroupId", ref _conditionGroupId, value);
      }
    }
    private string _expression;
    [Persistent, Size(500)]
    public string Expression
    {
      get
      {
        return _expression;
      }
      set
      {
        SetPropertyValue("Expression", ref _expression, value);
      }
    }

    #region Constructors

    public WFL_ConditionGroup() : base() { }
    public WFL_ConditionGroup(Session session) : base(session) { }

    #endregion
  }
}


using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Extensions;
using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  public class WFL_ConditionClassProperty : XPLiteObject
  {
    private long _conditionClassPropertyId;
    [Key(AutoGenerate = true)]
    public long ConditionClassPropertyId
    {
      get
      {
        return _conditionClassPropertyId;
      }
      set
      {
        SetPropertyValue("ConditionClassPropertyId", ref _conditionClassPropertyId, value);
      }
    }

    private WFL_ConditionPrimaryKey _conditionPrimaryKeyId;
    [Persistent("ConditionPrimaryKeyId")]
    public WFL_ConditionPrimaryKey ConditionPrimaryKey
    {
      get
      {
        return _conditionPrimaryKeyId;
      }
      set
      {
        SetPropertyValue("ConditionPrimaryKey", ref _conditionPrimaryKeyId, value);
      }
    }

    private string _property;
    [Persistent]
    public string Property
    {
      get
      {
        return _property;
      }
      set
      {
        SetPropertyValue("Property", ref _property, value);
      }
    }

    /// <summary>
    /// Tool Tip
    /// </summary>
    private string _description;
    [Persistent, Size(100)]
    public string Description
    {
      get
      {
        return _description;
      }
      set
      {
        SetPropertyValue("Description", ref _description, value);
      }
    }

    #region Constructors

    public WFL_ConditionClassProperty() : base() { }
    public WFL_ConditionClassProperty(Session session) : base(session) { }

    #endregion
  }
}

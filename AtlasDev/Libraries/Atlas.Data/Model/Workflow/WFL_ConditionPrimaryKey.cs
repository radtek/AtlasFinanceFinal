using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class WFL_ConditionPrimaryKey : XPLiteObject
  {
    private int _conditionPrimaryKeyId;
    [Key(AutoGenerate = true)]
    public int ConditionPrimaryKeyId
    {
      get
      {
        return _conditionPrimaryKeyId;
      }
      set
      {
        SetPropertyValue("ConditionPrimaryKeyId", ref _conditionPrimaryKeyId, value);
      }
    }

    private WFL_ConditionClass _conditionClass;
    [Persistent("ConditionClassId")]
    public WFL_ConditionClass ConditionClass
    {
      get
      {
        return _conditionClass;
      }
      set
      {
        SetPropertyValue("ConditionClass", ref _conditionClass, value);
      }
    }

    private WFL_DataExtType _dataExtType;
    [Persistent("PrimaryKeyDataExtTypeId")]
    public WFL_DataExtType PrimaryKeyDataExtType
    {
      get
      {
        return _dataExtType;
      }
      set
      {
        SetPropertyValue("PrimaryKeyDataExtType", ref _dataExtType, value);
      }
    }

    private string _primaryKeyProcessDataProperty;
    [Persistent, Size(100)]
    public string PrimaryKeyProcessDataProperty
    {
      get
      {
        return _primaryKeyProcessDataProperty;
      }
      set
      {
        SetPropertyValue("PrimaryKeyProcessDataProperty", ref _primaryKeyProcessDataProperty, value);
      }
    }

    #region Constructors

    public WFL_ConditionPrimaryKey() : base() { }
    public WFL_ConditionPrimaryKey(Session session) : base(session) { }

    #endregion
  }
}

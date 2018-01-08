using Atlas.Common.Extensions;
using Atlas.Enumerators;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public sealed class AVS_ExclusionRule : XPLiteObject
  {
    private Int32 _exclusionRuleId;
    [Key(AutoGenerate = true)]
    public Int32 ExclusionRuleId
    {
      get
      {
        return _exclusionRuleId;
      }
      set
      {
        SetPropertyValue("ExclusionRuleId", ref _exclusionRuleId, value);
      }
    }

    private AVS_Exclusion _exclusion;
    [Persistent("ExclusionId")]
    [Indexed]
    public AVS_Exclusion Exclusion
    {
      get
      {
        return _exclusion;
      }
      set
      {
        SetPropertyValue("Exclusion", ref _exclusion, value);
      }
    }

    private AVS.ResponseGroup _responseGroup;
    [Indexed]
    public AVS.ResponseGroup ResponseGroup
    {
      get
      {
        return _responseGroup;
      }
      set
      {
        SetPropertyValue("ResponseGroup", ref _responseGroup, value);
      }
    }


    private AVS.ResponseCode _responseCode;
    [Indexed]
    public AVS.ResponseCode ResponseCode
    {
      get
      {
        return _responseCode;
      }
      set
      {
        SetPropertyValue("ResponseCode", ref _responseCode, value);
      }
    }

    private bool _enabled;
    [Persistent]
    [Indexed]
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

    private DateTime _createDate;
    [Persistent]
    [Indexed]
    public DateTime CreateDate
    {
      get
      {
        return _createDate;
      }
      set
      {
        SetPropertyValue("CreateDate", ref _createDate, value);
      }
    }

    #region Constructors

    public AVS_ExclusionRule() : base() { }
    public AVS_ExclusionRule(Session session) : base(session) { }

    #endregion
  }
}

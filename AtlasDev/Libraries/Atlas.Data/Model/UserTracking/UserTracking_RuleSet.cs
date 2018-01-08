using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using Atlas.Domain.Interface;

namespace Atlas.Domain.Model
{
  public sealed class UserTracking_RuleSet : XPLiteObject
  {
    private Int64 _ruleSetId;
    [Key(AutoGenerate = true)]
    public Int64 RuleSetId
    {
      get
      {
        return _ruleSetId;
      }
      set
      {
        SetPropertyValue("RuleSetId", ref _ruleSetId, value);
      }
    }

    private Enumerators.Tracking.AlertType _alertType;
    public Enumerators.Tracking.AlertType AlertType
    {
      get
      {
        return _alertType;
      }
      set
      {
        SetPropertyValue("AlertType", ref _alertType, value);
      }
    }

    private Enumerators.Tracking.SeverityClassification _severityClassification;
    public Enumerators.Tracking.SeverityClassification SeverityClassification
    {
      get
      {
        return _severityClassification;
      }
      set
      {
        SetPropertyValue("SeverityClassification", ref _severityClassification, value);
      }
    }

    private Enumerators.Tracking.Elapse _elapse;
    public Enumerators.Tracking.Elapse Elapse
    {
      get
      {
        return _elapse;
      }
      set
      {
        SetPropertyValue("Elapse", ref _elapse, value);
      }
    }

    private int _value;
    public int Value
    {
      get
      {
        return _value;
      }
      set
      {
        SetPropertyValue("Value", ref _value, value);
      }
    }


    #region Constructors

    public UserTracking_RuleSet() : base() { }
    public UserTracking_RuleSet(Session session) : base(session) { }

    #endregion
  }
}
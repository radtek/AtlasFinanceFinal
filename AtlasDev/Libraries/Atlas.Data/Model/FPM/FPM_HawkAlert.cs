// -----------------------------------------------------------------------
// <copyright file="RiskEnquiry.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Atlas.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using DevExpress.Xpo;

  public sealed class FPM_HawkAlert : XPLiteObject
  {
    private Int64 _HawkAlertId;
    [Key(AutoGenerate = true)]
    public Int64 HawkAlertId
    {
      get
      {
        return _HawkAlertId;
      }
      set
      {
        SetPropertyValue("HawkAlertId", ref _HawkAlertId, value);
      }
    }

    private FPM_FraudScore _FraudScore;
    [Indexed]
    [Persistent("FraudScoreId")]
    public FPM_FraudScore FraudScore
    {
      get
      {
        return _FraudScore;
      }
      set
      {
        SetPropertyValue("FraudScore", ref _FraudScore, value);
      }
    }

    private string _HawkNo;
    [Persistent, Size(50)]
    public string HawkNo
    {
      get { return _HawkNo; }
      set
      {
        SetPropertyValue("HawkNo", ref _HawkNo, value);
      }
    }

    private string _HawkCode;
    [Persistent, Size(4)]
    public string HawkCode
    {
      get
      {
        return _HawkCode;
      }
      set
      {
        SetPropertyValue("HawkCode", ref _HawkCode, value);
      }
    }

    private string _HawkDescription;
    [Persistent, Size(50)]
    public string HawkDescription
    {
      get
      {
        return _HawkDescription;
      }
      set
      {
        SetPropertyValue("HawkDescription", ref _HawkDescription, value);
      }
    }

    private string _HawkFoundFor;
    [Persistent, Size(50)]
    public string HawkFoundFor
    {
      get
      {
        return _HawkFoundFor;
      }
      set
      {
        SetPropertyValue("HawkFoundFor", ref _HawkFoundFor, value);
      }
    }

    private string _DeceasedDate;
    [Persistent]
    public string DeceasedDate
    {
      get
      {
        return _DeceasedDate;
      }
      set
      {
        SetPropertyValue("DeceasedDate", ref _DeceasedDate, value);
      }
    }

    private string _SubscriberName;
    [Persistent, Size(45)]
    public string SubscriberName
    {
      get
      {
        return _SubscriberName;
      }
      set
      {
        SetPropertyValue("SubscriberName", ref _SubscriberName, value);
      }
    }

    private string _SubscriberReference;
    [Persistent, Size(15)]
    public string SubscriberReference
    {
      get
      {
        return _SubscriberReference;
      }
      set
      {
        SetPropertyValue("SubscriberReference", ref _SubscriberReference, value);
      }
    }

    private string _ContactName;
    [Persistent, Size(30)]
    public string ContactName
    {
      get
      {
        return _ContactName;
      }
      set
      {
        SetPropertyValue("ContactName", ref _ContactName, value);
      }
    }

    private string _ContactTelCode;
    [Persistent, Size(7)]
    public string ContactTelCode
    {
      get
      {
        return _ContactTelCode;
      }
      set
      {
        SetPropertyValue("ContactTelCode", ref _ContactTelCode, value);
      }
    }

    private string _ContactTelNo;
    [Persistent, Size(8)]
    public string ContactTelNo
    {
      get
      {
        return _ContactTelNo;
      }
      set
      {
        SetPropertyValue("ContactTelNo", ref _ContactTelNo, value);
      }
    }

    private string _VictimReference;
    [Persistent, Size(50)]
    public string VictimReference
    {
      get
      {
        return _VictimReference;
      }
      set
      {
        SetPropertyValue("VictimReference", ref _VictimReference, value);
      }
    }

    private string _VictimTelCode;
    [Persistent, Size(11)]
    public string VictimTelCode
    {
      get
      {
        return _VictimTelCode;
      }
      set
      {
        SetPropertyValue("VictimTelCode", ref _VictimTelCode, value);
      }
    }

    private string _VictimTelNo;
    [Persistent, Size(15)]
    public string VictimTelNo
    {
      get
      {
        return _VictimTelNo;
      }
      set
      {
        SetPropertyValue("VictimTelNo", ref _VictimTelNo, value);
      }
    }

    #region Constructors

    public FPM_HawkAlert() : base() { }
    public FPM_HawkAlert(Session session) : base(session) { }

    #endregion
  }
}

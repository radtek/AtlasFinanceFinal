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

  public sealed class FPM_FraudScore_Reason : XPLiteObject
  {
    private Int64 _FraudScoreReasonId;
    [Key(AutoGenerate = true)]
    public Int64 FraudScoreReasonId
    {
      get
      {
        return _FraudScoreReasonId;
      }
      set
      {
        SetPropertyValue("FraudScoreReasonId", ref _FraudScoreReasonId, value);
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

    private string _Description;
    [Persistent, Size(100)]
    public string Description
    {
      get { return _Description; }
      set
      {
        SetPropertyValue("Description", ref _Description, value);
      }
    }

    private string _ReasonCode;
    [Persistent, Size(8)]
    public string ReasonCode
    {
      get
      {
        return _ReasonCode;
      }
      set
      {
        SetPropertyValue("ReasonCode", ref _ReasonCode, value);
      }
    }

    #region Constructors

    public FPM_FraudScore_Reason() : base() { }
    public FPM_FraudScore_Reason(Session session) : base(session) { }

    #endregion
  }
}

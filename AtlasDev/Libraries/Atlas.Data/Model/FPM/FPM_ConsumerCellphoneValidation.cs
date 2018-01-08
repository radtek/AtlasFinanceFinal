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

  public sealed class FPM_ConsumerCellphoneValidation : XPLiteObject
  {
    private Int64 _ConsumerCellPhoneValidationID;
    [Key(AutoGenerate = true)]
    public Int64 ConsumerCellPhoneValidationID
    {
      get
      {
        return _ConsumerCellPhoneValidationID;
      }
      set
      {
        SetPropertyValue("ConsumerCellPhoneValidationID", ref _ConsumerCellPhoneValidationID, value);
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

    private string _CellularNumber;
    [Persistent, Size(20)]
    public string CellularNumber
    {
      get { return _CellularNumber; }
      set
      {
        SetPropertyValue("CellularNumber", ref _CellularNumber, value);
      }
    }

    private string _CellularVerification;
    [Persistent, Size(60)]
    public string CellularVerification
    {
      get
      {
        return _CellularVerification;
      }
      set
      {
        SetPropertyValue("CellularVerification", ref _CellularVerification, value);
      }
    }

    private string _CellularFirstUsed;
    [Persistent]
    public string CellularFirstUsed
    {
      get
      {
        return _CellularFirstUsed;
      }
      set
      {
        SetPropertyValue("CellularFirstUsed", ref _CellularFirstUsed, value);
      }
    }

    #region Constructors

    public FPM_ConsumerCellphoneValidation() : base() { }
    public FPM_ConsumerCellphoneValidation(Session session) : base(session) { }

    #endregion
  }
}

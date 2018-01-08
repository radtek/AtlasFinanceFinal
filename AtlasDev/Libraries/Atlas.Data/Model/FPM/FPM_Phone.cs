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

  public sealed class FPM_Phone : XPLiteObject
  {
    private Int64 _PhoneId;
    [Key(AutoGenerate = true)]
    public Int64 PhoneId
    {
      get
      {
        return _PhoneId;
      }
      set
      {
        SetPropertyValue("PhoneId", ref _PhoneId, value);
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

    private string _PhoneNo;
    [Persistent, Size(16)]
    public string PhoneNo
    {
      get { return _PhoneNo; }
      set
      {
        SetPropertyValue("PhoneNo", ref _PhoneNo, value);
      }
    }

    private string _PhoneTypeId;
    [Persistent]
    public string PhoneTypeId
    {
      get
      {
        return _PhoneTypeId;
      }
      set
      {
        SetPropertyValue("PhoneTypeId", ref _PhoneTypeId, value);
      }
    }

    private string _OtherDescription;
    [Persistent, Size(35)]
    public string OtherDescription
    {
      get
      {
        return _OtherDescription;
      }
      set
      {
        SetPropertyValue("OtherDescription", ref _OtherDescription, value);
      }
    }

    private DateTime? _InformationDate;
    [Persistent]
    public DateTime? InformationDate
    {
      get
      {
        return _InformationDate;
      }
      set
      {
        SetPropertyValue("InformationDate", ref _InformationDate, value);
      }
    }

    #region Constructors

    public FPM_Phone() : base() { }
    public FPM_Phone(Session session) : base(session) { }

    #endregion
  }
}

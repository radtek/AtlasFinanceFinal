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

  public sealed class FPM_FraudScore : XPLiteObject
  {
    private Int64 _FraudScoreId;
    [Key(AutoGenerate = true)]
    public Int64 FraudScoreId
    {
      get
      {
        return _FraudScoreId;
      }
      set
      {
        SetPropertyValue("FraudScoreId", ref _FraudScoreId, value);
      }
    }

    private BUR_Enquiry _enquiry;
    [Indexed]
    [Persistent("EnquiryId")]
    public BUR_Enquiry Enquiry
    {
      get
      {
        return _enquiry;
      }
      set
      {
        SetPropertyValue("Enquiry", ref _enquiry, value);
      }
    }

    private string _RecordSeq;
    [Persistent, Size(3)]
    public string RecordSeq
    {
      get { return _RecordSeq; }
      set
      {
        SetPropertyValue("RecordSeq", ref _RecordSeq, value);
      }
    }

    private string _Part;
    [Persistent, Size(3)]
    public string Part
    {
      get
      {
        return _Part;
      }
      set
      {
        SetPropertyValue("Part", ref _Part, value);
      }
    }

    private string _PartSeq;
    [Persistent, Size(5)]
    public string PartSeq
    {
      get
      {
        return _PartSeq;
      }
      set
      {
        SetPropertyValue("PartSeq", ref _PartSeq, value);
      }
    }

    private string _Rating;
    [Persistent, Size(6)]
    public string Rating
    {
      get
      {
        return _Rating;
      }
      set
      {
        SetPropertyValue("Rating", ref _Rating, value);
      }
    }

    private string _RatingDescription;
    [Persistent, Size(100)]
    public string RatingDescription
    {
      get
      {
        return _RatingDescription;
      }
      set
      {
        SetPropertyValue("RatingDescription", ref _RatingDescription, value);
      }
    }

    private string _IDNumber;
    [Persistent, Size(13)]
    public string IDNumber
    {
      get
      {
        return _IDNumber;
      }
      set
      {
        SetPropertyValue("IDNumber", ref _IDNumber, value);
      }
    }

    private string _BankAccountNo;
    [Persistent, Size(15)]
    public string BankAccountNo
    {
      get
      {

        return _BankAccountNo;
      }
      set
      {
        SetPropertyValue("BankAccountNo", ref _BankAccountNo, value);
      }
    }

    private string _CellNo;
    [Persistent, Size(11)]
    public string CellNo
    {
      get
      {

        return _CellNo;
      }
      set
      {
        SetPropertyValue("CellNo", ref _CellNo, value);
      }
    }

    private bool _Passed;
    [Persistent]
    public bool Passed
    {
      get
      {
        return _Passed;
      }
      set
      {
        SetPropertyValue("Passed", ref _Passed, value);
      }
    }

    private DateTime? _OverrideDate;
    [Persistent]
    public DateTime? OverrideDate
    {
      get
      {
        return _OverrideDate;
      }
      set
      {
        SetPropertyValue("OverrideDate", ref _OverrideDate, value);
      }
    }

    private PER_Person _OverrideUser;
    [Persistent]
    public PER_Person OverrideUser
    {
      get
      {
        return _OverrideUser;
      }
      set
      {
        SetPropertyValue("OverrideUser", ref _OverrideUser, value);
      }
    }

    private string _overrideReason;
    [Persistent, Size(500)]
    public string OverrideReason
    {
      get
      {

        return _overrideReason;
      }
      set
      {
        SetPropertyValue("OverrideReason", ref _overrideReason, value);
      }
    }

    private DateTime? _CreatedDate;
    [Persistent]
    public DateTime? CreatedDate
    {
      get
      {
        return _CreatedDate;
      }
      set
      {
        SetPropertyValue("CreatedDate", ref _CreatedDate, value);
      }
    }

    #region Constructors

    public FPM_FraudScore() : base() { }
    public FPM_FraudScore(Session session) : base(session) { }

    #endregion
  }
}

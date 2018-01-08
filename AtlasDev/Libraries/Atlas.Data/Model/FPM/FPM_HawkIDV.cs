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

  public sealed class FPM_HawkIDV : XPLiteObject
  {
    private Int64 _HawkIdvId;
    [Key(AutoGenerate = true)]
    public Int64 HawkIdvId
    {
      get
      {
        return _HawkIdvId;
      }
      set
      {
        SetPropertyValue("HawkIdvId", ref _HawkIdvId, value);
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

    private string _IDVerifiedCode;
    [Persistent, Size(2)]
    public string IDVerifiedCode
    {
      get { return _IDVerifiedCode; }
      set
      {
        SetPropertyValue("IDVerifiedCode", ref _IDVerifiedCode, value);
      }
    }

    private string _IDVerifiedDescription;
    [Persistent, Size(60)]
    public string IDVerifiedDescription
    {
      get
      {
        return _IDVerifiedDescription;
      }
      set
      {
        SetPropertyValue("IDVerifiedDescription", ref _IDVerifiedDescription, value);
      }
    }

    private string _VerifiedSurname;
    [Persistent, Size(45)]
    public string VerifiedSurname
    {
      get
      {
        return _VerifiedSurname;
      }
      set
      {
        SetPropertyValue("VerifiedSurname", ref _VerifiedSurname, value);
      }
    }

    private string _VerifiedForeName1;
    [Persistent, Size(15)]
    public string VerifiedForeName1
    {
      get
      {
        return _VerifiedForeName1;
      }
      set
      {
        SetPropertyValue("VerifiedForeName1", ref _VerifiedForeName1, value);
      }
    }

    private string _VerifiedForeName2;
    [Persistent, Size(15)]
    public string VerifiedForeName2
    {
      get
      {
        return _VerifiedForeName2;
      }
      set
      {
        SetPropertyValue("VerifiedForeName2", ref _VerifiedForeName2, value);
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


    #region Constructors

    public FPM_HawkIDV() : base() { }
    public FPM_HawkIDV(Session session) : base(session) { }

    #endregion
  }
}

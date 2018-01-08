using System;
using Atlas.Domain.Model;
using DevExpress.Xpo;

namespace Atlas.Domain.Ass.Models
{
  public class ASS_CiReportScore : XPLiteObject
  {
    private long _ciReportScoreId;

    [Key(AutoGenerate = true)]
    public long CiReportScoreId
    {
      get { return _ciReportScoreId; }
      set { SetPropertyValue("CiReportScoreId", ref _ciReportScoreId, value); }
    }

    private ASS_CiReportVersion _ciReportVersion;

    [Persistent("CiReportVersionId")]
    [Indexed]
    public ASS_CiReportVersion CiReportVersion
    {
      get { return _ciReportVersion; }
      set { SetPropertyValue("CiReportVersion", ref _ciReportVersion, value); }
    }

    private BRN_Branch _branch;

    [Persistent("BranchId")]
    [Indexed]
    public BRN_Branch Branch
    {
      get { return _branch; }
      set { SetPropertyValue("Branch", ref _branch, value); }
    }

    private DateTime _date;

    [Persistent]
    [Indexed]
    public DateTime Date
    {
      get { return _date; }
      set { SetPropertyValue("Date", ref _date, value); }
    }

    private int _payNo;

    [Persistent]
    public int PayNo
    {
      get { return _payNo; }
      set { SetPropertyValue("PayNo", ref _payNo, value); }
    }

    private int _scoreAboveXWeekly;

    [Persistent]
    public int ScoreAboveXWeekly
    {
      get { return _scoreAboveXWeekly; }
      set { SetPropertyValue("ScoreAboveXWeekly", ref _scoreAboveXWeekly, value); }
    }

    private int _scoreAboveXBiWeekly;

    [Persistent]
    public int ScoreAboveXBiWeekly
    {
      get { return _scoreAboveXBiWeekly; }
      set { SetPropertyValue("ScoreAboveXBiWeekly", ref _scoreAboveXBiWeekly, value); }
    }

    private int _scoreAboveXMonthly;

    [Persistent]
    public int ScoreAboveXMonthly
    {
      get { return _scoreAboveXMonthly; }
      set { SetPropertyValue("ScoreAboveXMonthly", ref _scoreAboveXMonthly, value); }
    }

    #region Constructors

    public ASS_CiReportScore()
    {
    }

    public ASS_CiReportScore(Session session)
      : base(session)
    {
    }

    #endregion
  }
}
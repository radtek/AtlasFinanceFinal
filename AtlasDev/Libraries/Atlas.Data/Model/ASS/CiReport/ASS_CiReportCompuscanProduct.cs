using System;
using DevExpress.Xpo;

namespace Atlas.Domain.Model.ASS.CiReport
{
  public class ASS_CiReportCompuscanProduct : XPLiteObject
  {
    private long _ciReportCompuscanProductId;

    [Key(AutoGenerate = true)]
    public long CiReportCompuscanProductId
    {
      get { return _ciReportCompuscanProductId; }
      set { SetPropertyValue("CiReportCompuscanProductId", ref _ciReportCompuscanProductId, value); }
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


    private int _1Month;

    [Persistent]
    public int OneMonth
    {
      get { return _1Month; }
      set { SetPropertyValue("1Month", ref _1Month, value); }
    }

    private int _oneMonthThin;

    [Persistent]
    public int OneMonthThin
    {
      get { return _oneMonthThin; }
      set { SetPropertyValue("OneMonthThin", ref _oneMonthThin, value); }
    }

    private int _oneMonthCapped;

    [Persistent]
    public int OneMonthCapped
    {
      get { return _oneMonthCapped; }
      set { SetPropertyValue("OneMonthCapped", ref _oneMonthCapped, value); }
    }

    private int _twoToFourMonth;

    [Persistent]
    public int TwoToFourMonth
    {
      get { return _twoToFourMonth; }
      set { SetPropertyValue("TwoToFourMonth", ref _twoToFourMonth, value); }
    }

    private int _fiveToSixMonth;

    [Persistent]
    public int FiveToSixMonth
    {
      get { return _fiveToSixMonth; }
      set { SetPropertyValue("FiveToSixMonth", ref _fiveToSixMonth, value); }
    }

    private int _twelveMonth;

    [Persistent]
    public int TwelveMonth
    {
      get { return _twelveMonth; }
      set { SetPropertyValue("TwelveMonth", ref _twelveMonth, value); }
    }

    private int _declined;

    [Persistent]
    public int Declined
    {
      get { return _declined; }
      set { SetPropertyValue("Declined", ref _declined, value); }
    }

    #region Constructors

    public ASS_CiReportCompuscanProduct()
    {
    }

    public ASS_CiReportCompuscanProduct(Session session)
      : base(session)
    {
    }

    #endregion
  }
}
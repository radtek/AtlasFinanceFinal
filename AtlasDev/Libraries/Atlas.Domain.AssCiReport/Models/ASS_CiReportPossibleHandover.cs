using System;
using Atlas.Domain.Model;
using DevExpress.Xpo;

namespace Atlas.Domain.Ass.Models
{
  public class ASS_CiReportPossibleHandover : XPLiteObject
  {
    private long _ciReportPossibleHandoverId;

    [Key(AutoGenerate = true)]
    public long CiReportPossibleHandoverId
    {
      get { return _ciReportPossibleHandoverId; }
      set { SetPropertyValue("CiReportPossibleHandoverId", ref _ciReportPossibleHandoverId, value); }
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

    private decimal _arrears;

    [Persistent]
    public decimal Arrears
    {
      get { return _arrears; }
      set { SetPropertyValue("Arrears", ref _arrears, value); }
    }

    private DateTime _oldestArrearsDate;

    [Persistent]
    [Indexed]
    public DateTime OldestArrearsDate
    {
      get { return _oldestArrearsDate; }
      set { SetPropertyValue("OldestArrearsDate", ref _oldestArrearsDate, value); }
    }

    private decimal _receivableThisMonth;

    [Persistent]
    public decimal ReceivableThisMonth
    {
      get { return _receivableThisMonth; }
      set { SetPropertyValue("ReceivableThisMonth", ref _receivableThisMonth, value); }
    }

    private decimal _receivedThisMonth;

    [Persistent]
    public decimal ReceivedThisMonth
    {
      get { return _receivedThisMonth; }
      set { SetPropertyValue("ReceivedThisMonth", ref _receivedThisMonth, value); }
    }

    private decimal _receivablePast;

    [Persistent]
    public decimal ReceivablePast
    {
      get { return _receivablePast; }
      set { SetPropertyValue("ReceivablePast", ref _receivablePast, value); }
    }

    private decimal _receivedPast;

    [Persistent]
    public decimal ReceivedPast
    {
      get { return _receivedPast; }
      set { SetPropertyValue("ReceivedPast", ref _receivedPast, value); }
    }

    private decimal _debtorsBookValue;

    [Persistent]
    public decimal DebtorsBookValue
    {
      get { return _debtorsBookValue; }
      set { SetPropertyValue("DebtorsBookValue", ref _debtorsBookValue, value); }
    }

    private decimal _flaggedOverdueValue;

    [Persistent]
    public decimal FlaggedOverdueValue
    {
      get { return _flaggedOverdueValue; }
      set { SetPropertyValue("FlaggedOverdueValue", ref _flaggedOverdueValue, value); }
    }

    private int _flaggedNoOfLoans;

    [Persistent]
    public int FlaggedNoOfLoans
    {
      get { return _flaggedNoOfLoans; }
      set { SetPropertyValue("FlaggedNoOfLoans", ref _flaggedNoOfLoans, value); }
    }

    private decimal _possibleHandOvers;

    [Persistent]
    public decimal PossibleHandOvers
    {
      get { return _possibleHandOvers; }
      set { SetPropertyValue("PossibleHandOvers", ref _possibleHandOvers, value); }
    }

    private decimal _nextPossibleHandOvers;

    [Persistent]
    public decimal NextPossibleHandOvers
    {
      get { return _nextPossibleHandOvers; }
      set { SetPropertyValue("NextPossibleHandOvers", ref _nextPossibleHandOvers, value); }
    }

    #region Constructors

    public ASS_CiReportPossibleHandover()
    {
    }

    public ASS_CiReportPossibleHandover(Session session)
      : base(session)
    {
    }

    #endregion
  }
}
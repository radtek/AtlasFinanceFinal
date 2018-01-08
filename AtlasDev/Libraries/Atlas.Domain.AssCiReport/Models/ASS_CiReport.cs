using System;
using Atlas.Domain.Model;
using DevExpress.Xpo;

namespace Atlas.Domain.Ass.Models
{
  public class ASS_CiReport : XPLiteObject
  {
    private long _ciReport;

    [Key(AutoGenerate = true)]
    public long CiReportId
    {
      get { return _ciReport; }
      set { SetPropertyValue("CiReportId", ref _ciReport, value); }
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

    private int _noOfLoans;

    [Persistent]
    public int NoOfLoans
    {
      get { return _noOfLoans; }
      set { SetPropertyValue("NoOfLoans", ref _noOfLoans, value); }
    }

    private decimal _cheque;

    [Persistent]
    public decimal Cheque
    {
      get { return _cheque; }
      set { SetPropertyValue("Cheque", ref _cheque, value); }
    }

    private decimal _chequeToday;

    [Persistent]
    public decimal ChequeToday
    {
      get { return _chequeToday; }
      set { SetPropertyValue("ChequeToday", ref _chequeToday, value); }
    }

    private int _branchLoans;

    [Persistent]
    public int BranchLoans
    {
      get { return _branchLoans; }
      set { SetPropertyValue("BranchLoans", ref _branchLoans, value); }
    }

    private int _salesRepLoans;

    [Persistent]
    public int SalesRepLoans
    {
      get { return _salesRepLoans; }
      set { SetPropertyValue("SalesRepLoans", ref _salesRepLoans, value); }
    }

    private decimal _chargesExclVAT;

    [Persistent]
    public decimal ChargesExclVAT
    {
      get { return _chargesExclVAT; }
      set { SetPropertyValue("ChargesExclVAT", ref _chargesExclVAT, value); }
    }

    private decimal _chargesVAT;

    [Persistent]
    public decimal ChargesVAT
    {
      get { return _chargesVAT; }
      set { SetPropertyValue("ChargesVAT", ref _chargesVAT, value); }
    }

    private decimal _totalCharges;

    [Persistent]
    public decimal TotalCharges
    {
      get { return _totalCharges; }
      set { SetPropertyValue("TotalCharges", ref _totalCharges, value); }
    }

    private decimal _creditLife;

    [Persistent]
    public decimal CreditLife
    {
      get { return _creditLife; }
      set { SetPropertyValue("CreditLife", ref _creditLife, value); }
    }

    private decimal _loanFeeExclVAT;

    [Persistent]
    public decimal LoanFeeExclVAT
    {
      get { return _loanFeeExclVAT; }
      set { SetPropertyValue("LoanFeeExclVAT", ref _loanFeeExclVAT, value); }
    }

    private decimal _loanFeeVAT;

    [Persistent]
    public decimal LoanFeeVAT
    {
      get { return _loanFeeVAT; }
      set { SetPropertyValue("LoanFeeVAT", ref _loanFeeVAT, value); }
    }

    private decimal _loanFeeInclVAT;

    [Persistent]
    public decimal LoanFeeInclVAT
    {
      get { return _loanFeeInclVAT; }
      set { SetPropertyValue("LoanFeeInclVAT", ref _loanFeeInclVAT, value); }
    }

    private decimal _FuneralAddOn;

    [Persistent]
    public decimal FuneralAddOn
    {
      get { return _FuneralAddOn; }
      set { SetPropertyValue("FuneralAddOn", ref _FuneralAddOn, value); }
    }

    private decimal _AgeAddOn;

    [Persistent]
    public decimal AgeAddOn
    {
      get { return _AgeAddOn; }
      set { SetPropertyValue("AgeAddOn", ref _AgeAddOn, value); }
    }

    private decimal _VAPExcl;

    [Persistent]
    public decimal VAPExcl
    {
      get { return _VAPExcl; }
      set { SetPropertyValue("VAPExcl", ref _VAPExcl, value); }
    }

    private decimal _VAPVAT;

    [Persistent]
    public decimal VAPVAT
    {
      get { return _VAPVAT; }
      set { SetPropertyValue("VAPVAT", ref _VAPVAT, value); }
    }

    private decimal _VAPIncl;

    [Persistent]
    public decimal VAPIncl
    {
      get { return _VAPIncl; }
      set { SetPropertyValue("VAPIncl", ref _VAPIncl, value); }
    }
      
    private decimal _TotalAddOn;

    [Persistent]
    public decimal TotalAddOn
    {
      get { return _TotalAddOn; }
      set { SetPropertyValue("TotalAddOn", ref _TotalAddOn, value); }
    }

    private decimal _TotFeeExcl;

    [Persistent]
    public decimal TotFeeExcl
    {
      get { return _TotFeeExcl; }
      set { SetPropertyValue("TotFeeExcl", ref _TotFeeExcl, value); }
    }

    private decimal _TotFeeVAT;

    [Persistent]
    public decimal TotFeeVAT
    {
      get { return _TotFeeVAT; }
      set { SetPropertyValue("TotFeeVAT", ref _TotFeeVAT, value); }
    }

    private decimal _TotFeeIncl;

    [Persistent]
    public decimal TotFeeIncl
    {
      get { return _TotFeeIncl; }
      set { SetPropertyValue("TotFeeIncl", ref _TotFeeIncl, value); }
    }

    private int _newClientNoOfLoans;

    [Persistent]
    public int NewClientNoOfLoans
    {
      get { return _newClientNoOfLoans; }
      set { SetPropertyValue("NewClientNoOfLoans", ref _newClientNoOfLoans, value); }
    }

    private decimal _newClientAmount;

    [Persistent]
    public decimal NewClientAmount
    {
      get { return _newClientAmount; }
      set { SetPropertyValue("NewClientAmount", ref _newClientAmount, value); }
    }

    private int _existingClientCount;

    [Persistent]
    public int ExistingClientCount
    {
      get { return _existingClientCount; }
      set { SetPropertyValue("ExistingClientCount", ref _existingClientCount, value); }
    }

    private int _revivedClientCount;

    [Persistent]
    public int RevivedClientCount
    {
      get { return _revivedClientCount; }
      set { SetPropertyValue("RevivedClientCount", ref _revivedClientCount, value); }
    }

    private decimal _revivedClientAmount;

    [Persistent]
    public decimal RevivedClientAmount
    {
      get { return _revivedClientAmount; }
      set { SetPropertyValue("RevivedClientAmount", ref _revivedClientAmount, value); }
    }

    private decimal _collections;

    [Persistent]
    public decimal Collections
    {
      get { return _collections; }
      set { SetPropertyValue("Collections", ref _collections, value); }
    }

    private decimal _refunds;

    [Persistent]
    public decimal Refunds
    {
      get { return _refunds; }
      set { SetPropertyValue("Refunds", ref _refunds, value); }
    }

    private int _reswipeBankChange;

    [Persistent]
    public int ReswipeBankChange
    {
      get { return _reswipeBankChange; }
      set { SetPropertyValue("ReswipeBankChange", ref _reswipeBankChange, value); }
    }

    private int _reswipeLoanTermChange;

    [Persistent]
    public int ReswipeLoanTermChange
    {
      get { return _reswipeLoanTermChange; }
      set { SetPropertyValue("ReswipeLoanTermChange", ref _reswipeLoanTermChange, value); }
    }

    private int _reswipeInstalmentChange;

    [Persistent]
    public int ReswipeInstalmentChange
    {
      get { return _reswipeInstalmentChange; }
      set { SetPropertyValue("ReswipeInstalmentChange", ref _reswipeInstalmentChange, value); }
    }

    private decimal _rollbackValue;

    [Persistent]
    public decimal RollbackValue
    {
      get { return _rollbackValue; }
      set { SetPropertyValue("RollbackValue", ref _rollbackValue, value); }
    }

    private decimal _vapLinkedLoansValue;

    [Persistent]
    public decimal VapLinkedLoansValue
    {
      get { return _vapLinkedLoansValue; }
      set { SetPropertyValue("VapLinkedLoansValue", ref _vapLinkedLoansValue, value); }
    }

    private int _vapLinkedLoans;

    [Persistent]
    public int VapLinkedLoans
    {
      get { return _vapLinkedLoans; }
      set { SetPropertyValue("VapLinkedLoans", ref _vapLinkedLoans, value); }
    }

    private int _vapDeniedByConWithAuth;

    [Persistent]
    public int VapDeniedByConWithAuth
    {
      get { return _vapDeniedByConWithAuth; }
      set { SetPropertyValue("VapDeniedByConWithAuth", ref _vapDeniedByConWithAuth, value); }
    }

    private int _vapDeniedByConWithOutAuth;

    [Persistent]
    public int VapDeniedByConWithOutAuth
    {
      get { return _vapDeniedByConWithOutAuth; }
      set { SetPropertyValue("VapDeniedByConWithOutAuth", ref _vapDeniedByConWithOutAuth, value); }
    }

    private int _vapExcludedLoans;

    [Persistent]
    public int VapExcludedLoans
    {
      get { return _vapExcludedLoans; }
      set { SetPropertyValue("VapExcludedLoans", ref _vapExcludedLoans, value); }
    }

    #region Constructors

    public ASS_CiReport()
    {
    }

    public ASS_CiReport(Session session) : base(session)
    {
    }

    #endregion
  }
}
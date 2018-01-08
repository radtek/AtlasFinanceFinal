using Atlas.Ass.Framework.Structures;

namespace Atlas.Ass.Structures
{
  public class BasicLoan : IBasicLoan
  {
    public string LegacyBranchNumber { get; set; }
    public int PayNo { get; set; }
    public int Quantity { get; set; }
    public decimal Cheque { get; set; }
    public decimal ChequeToday { get; set; }    
    public int BranchLoans { get; set; }
    public int SalesRepLoans { get; set; }
    public decimal ChargesExclVAT { get; set; }
    public decimal ChargesVAT { get; set; }
    public decimal TotalCharges { get; set; }
    public decimal CreditLife { get; set; }
    public decimal LoanFeeExclVAT { get; set; }
    public decimal LoanFeeVAT { get; set; }
    public decimal LoanFeeInclVAT { get; set; }
    public int ScoreAbove615Weekly { get; set; }
    public int ScoreAbove615BiWeekly { get; set; }
    public int ScoreAbove615Monthly { get; set; }
    public decimal FuneralAddOn { get; set; }
    public decimal AgeAddOn { get; set; }
    public decimal VAPExcl { get; set; }
    public decimal VAPVAT { get; set; }
    public decimal VAPIncl { get; set; }
    public decimal TotalAddOn { get; set; }
    public decimal TotFeeExcl { get; set; }
    public decimal TotFeeVAT { get; set; }
    public decimal TotFeeIncl { get; set; }
  }
}
namespace Atlas.Ass.Framework.Structures
{
  public interface IBasicLoan
  {
    string LegacyBranchNumber { get; set; }
    int PayNo { get; set; }
    int Quantity { get; set; }
    decimal Cheque { get; set; }
    decimal ChequeToday { get; set; }
    int BranchLoans { get; set; }
    int SalesRepLoans { get; set; }
    decimal ChargesExclVAT { get; set; }
    decimal ChargesVAT { get; set; }
    decimal TotalCharges { get; set; }
    decimal CreditLife { get; set; }
    decimal LoanFeeExclVAT { get; set; }
    decimal LoanFeeVAT { get; set; }
    decimal LoanFeeInclVAT { get; set; }
    int ScoreAbove615Weekly { get; set; }
    int ScoreAbove615BiWeekly { get; set; }
    int ScoreAbove615Monthly { get; set; }
    decimal FuneralAddOn { get; set; }
    decimal AgeAddOn { get; set; }
    decimal VAPExcl { get; set; }
    decimal VAPVAT { get; set; }
    decimal VAPIncl { get; set; }
    decimal TotalAddOn { get; set; }
    decimal TotFeeExcl { get; set; }
    decimal TotFeeVAT { get; set; }
    decimal TotFeeIncl { get; set; }
  }
}
using System.ComponentModel;
using Atlas.Common.Attributes;
using Falcon.Exporter.CiReport.Infrastructure.Attributes;

namespace Falcon.Exporter.CiReport.Infrastructure.Structures.Reports
{
  public class CiReport
  {
    [Description("Id")]
    public long Id { get; set; }

    [Description("LOANMETH")]
    [Order(1)]
    public string LoanMeth { get; set; }

    [Description("Name")]
    [Order(2)]
    public string Name { get; set; }

    [Description("PayNo")]
    [Order(2)]
    [DetailFormat("Black", "LightGray", "right")]
    public int PayNo { get; set; }

    [Description("QTY_SFEE")]
    [Order(2)]
    public string QtySfee { get; set; }

    [Description("Cheque")]
    [Order(3)]
    [Format("#,##0;-#,##0")]
    [DetailFormat("Black", "LightGray", "right")]
    public float Cheque { get; set; }

    [Description("Target")]
    [Order(4)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float Target { get; set; }

    [Description("Target %")]
    [Order(5)]
    [Format("0.00%")]
    [DetailFormat(alignment: "right")]
    public float TargetPercent { get; set; }

    [Description("Actual %")]
    [Order(6)]
    [Format("0.00%")]
    [DetailFormat("Black", "LightGray", "right")]
    public float ActualPercent { get; set; }

    [Description("Deviation %")]
    [Order(7)]
    [Format("0.00%")]
    [DetailFormat("Black", "BurlyWood", "right")]
    public float DeviationPercent { get; set; }

    [Description("CHARGES_EXCL")]
    [Order(8)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float ChargesExclVat { get; set; }

    [Description("CHARGES_VAT")]
    [Order(9)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float ChargesVat { get; set; }

    [Description("CHARGES_INCL")]
    [Order(10)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float TotalCharges { get; set; }

    [Description("CHARGES %")]
    [Order(11)]
    [Format("0.00%")]
    [DetailFormat(alignment: "right")]
    public float ChargesPercent { get; set; }

    [Description("CREDIT_LIFE")]
    [Order(12)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float CreditLife { get; set; }

    [Description("CREDIT_LIFE %")]
    [Order(13)]
    [Format("0.00%")]
    [DetailFormat(alignment: "right")]
    public float CreditLifePercent { get; set; }

    [Description("LOAN_FEE_EXCL")]
    [Order(14)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float LoanFeeExclVat { get; set; }

    [Description("LOAN_FEE_VAT")]
    [Order(15)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float LoanFeeVat { get; set; }

    [Description("LOAN_FEE_INCL")]
    [Order(16)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float LoanFeeInclVat { get; set; }

    [Description("LOAN_FEE %")]
    [Order(17)]
    [Format("0.00%")]
    [DetailFormat(alignment: "right")]
    public float LoanFeePercent { get; set; }

    [Description("FUNERAL_ADD_ON")]
    [Order(18)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float FuneralAddOn { get; set; }

    [Description("AGE_ADD_ON")]
    [Order(19)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float AgeAddOn { get; set; }

    [Description("VAP_EXCL")]
    [Order(20)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float VapExcl { get; set; }

    [Description("VAP_VAT")]
    [Order(21)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float VapVat { get; set; }

    [Description("VAP_INCL")]
    [Order(22)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float VapIncl { get; set; }

    [Description("TOT_ADD_ON")]
    [Order(23)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float TotalAddOn { get; set; }

    [Description("ADD_ON%")]
    [Order(24)]
    [Format("0.00%")]
    [DetailFormat(alignment: "right")]
    public float AddOnPercent { get; set; }

    [Description("TOT_FEE_EXCL")]
    [Order(25)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float TotFeeExcl { get; set; }

    [Description("TOT_FEE_VAT")]
    [Order(26)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float TotFeeVat { get; set; }

    [Description("TOT_FEE_INCL")]
    [Order(27)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float TotFeeIncl { get; set; }

    [Description("TOT_FEE%")]
    [Order(28)]
    [Format("0.00%")]
    [DetailFormat(alignment: "right")]
    public float TotFeePercent { get; set; }

    [Description("HOVRTOT")]
    [Order(29)]
    [Format("#,##0;-#,##0")]
    [DetailFormat("Black", "Aqua", "right")]
    public float HandedOverLoansAmount { get; set; }

    [Description("H/O %")]
    [Order(30)]
    [Format("0.00%")]
    [DetailFormat(alignment: "right")]
    public float HandedOverLoanPercent { get; set; }

    [Description("No. Of loans")]
    [Order(31)]
    [DetailFormat("Black", "LightGray", "right")]
    public int Loans { get; set; }

    [Description("Loan Mix")]
    [Order(32)]
    [Format("0.00%")]
    [DetailFormat("Black", "LightGray", "right")]
    public float LoanMix { get; set; }

    [Description("VAP Value")]
    [Order(33)]
    [DetailFormat(alignment: "right")]
    public int VapValueLinked { get; set; }

    [Description("VAP - Linked")]
    [Order(34)]
    [DetailFormat(alignment: "right")]
    public int VapLinked { get; set; }

    [Description("VAP - Consultant Denied With Auth")]
    [Order(35)]
    [DetailFormat(alignment: "right")]
    public int VapDeniedByConWithAuth { get; set; }

    [Description("VAP - Consultant Denied Without Auth")]
    [Order(36)]
    [DetailFormat(alignment: "right")]
    public int VapDeniedByConWithOutAuth { get; set; }

    [Description("VAP - Excluded")]
    [Order(37)]
    [DetailFormat(alignment: "right")]
    public int VapExcludedLoans { get; set; }

    [Description("COLLECT")]
    [Order(38)]
    [Format("#,##0;-#,##0")]
    [DetailFormat("Black", "LightGreen", "right")]
    public float Collections { get; set; }

    [Description("Rolled Value")]
    [Order(39)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float RolledValue { get; set; }

    [Description("Rolled %")]
    [Order(40)]
    [Format("0.00%")]
    [DetailFormat("Black", "LightGray", "right")]
    public float RolledPercent { get; set; }

    [Description("Refunds")]
    [Order(41)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float Refunds { get; set; }

    [Description("Clients New Amount")]
    [Order(42)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float QuantityClientNew { get; set; }

    [Description("H/O No of loans")]
    [Order(43)]
    [DetailFormat(alignment: "right")]
    public int HandedOverLoansQuantity { get; set; }

    [Description("H/O No of clients")]
    [Order(43)]
    [DetailFormat(alignment: "right")]
    public int HandedOverClientQuantity { get; set; }

    [Description("Clients Branch")]
    [Order(44)]
    [DetailFormat(alignment: "right")]
    public int ClientsBranch { get; set; }

    [Description("Clients Sales Reps")]
    [Order(45)]
    [DetailFormat(alignment: "right")]
    public int SalesRepLoans { get; set; }

    [Description("Client Total New")]
    [Order(46)]
    [DetailFormat("Black", "BurlyWood", "right")]
    public int NewClientNoOfLoans { get; set; }

    [Description("Current Client")]
    [Order(47)]
    [DetailFormat("Black", "BurlyWood", "right")]
    public int CurrentClient { get; set; }

    [Description("Revived Client")]
    [Order(48)]
    [DetailFormat("Black", "BurlyWood", "right")]
    public int RevivedClient { get; set; }

    [Description("New Client Mix")]
    [Order(49)]
    [Format("0.00%")]
    [DetailFormat(alignment: "right")]
    public float NewClientMix { get; set; }

    [Description("AVE LN")]
    [Order(50)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float AverageLoan { get; set; }

    [Description("New Client AVE LN")]
    [Order(51)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float NewClientAverageLoan { get; set; }

    [Description("Revived AVE LN")]
    [Order(52)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float AverageRevivedClientAmount { get; set; }

    [Description("Reswipe Bank Change")]
    [Order(53)]
    [DetailFormat(alignment: "right")]
    public int ReswipeBankChange { get; set; }

    [Description("Reswipe Loan Term Change")]
    [Order(54)]
    [DetailFormat(alignment: "right")]
    public int ReswipeLoanTermChange { get; set; }

    [Description("Reswipe Instalment Change")]
    [Order(55)]
    [DetailFormat(alignment: "right")]
    public int ReswipeInstalmentChange { get; set; }

    [Description("1 Month")]
    [Order(56)]
    [DetailFormat(alignment: "right")]
    public int OneMonth { get; set; }

    [Description("1M Thin")]
    [Order(57)]
    [DetailFormat(alignment: "right")]
    public int OneMonthThin { get; set; }

    [Description("1M Capped")]
    [Order(58)]
    [DetailFormat(alignment: "right")]
    public int OneMonthCapped { get; set; }

    [Description("2 to 4 Month")]
    [Order(59)]
    [DetailFormat(alignment: "right")]
    public int TwoToFourMonth { get; set; }

    [Description("5 to 6 Month")]
    [Order(60)]
    [DetailFormat(alignment: "right")]
    public int FiveToSixMonth { get; set; }

    [Description("12 Month")]
    [Order(61)]
    [DetailFormat(alignment: "right")]
    public int TwelveMonth { get; set; }

    [Description("Declined")]
    [Order(62)]
    [DetailFormat(alignment: "right")]
    public int Declined { get; set; }

    [Description("Total Compuscan Enquiries")]
    [Order(63)]
    [DetailFormat(alignment: "right")]
    public int TotalProducts { get; set; }
  }
}
using System.Collections.Generic;
using System.ComponentModel;

namespace Falcon.Common.Structures.Report.Ass
{
  public class CiFile
  {
    public string LegacyBranchNumber { get; set; } // not diplayed on excel file

    [Description("LOANMETH")]
    public string LoanMeth { get; set; }

    [Description("PAYNO")]
    public int PayNo { get; set; }

    [Description("QTY_SFEE")]
    public string QuantitySFee { get; set; }

    [Description("CHEQUE")]
    public decimal Cheque { get; set; }

    [Description("Target")]
    public decimal Target { get; set; }

    [Description("Target  %")]
    public float TargetPercent { get; set; }

    [Description("Actual  %")]
    public float Actual { get; set; }

    [Description("Deviation  %")]
    public float Deviation { get; set; }

    [Description("CHARGES_EXCL")]
    public decimal ChargesExclVAT { get; set; }

    [Description("CHARGES_VAT")]
    public decimal ChargesVAT { get; set; }

    [Description("CHARGES_INCL")]
    public decimal TotalCharges { get; set; }

    [Description("CHARGES %")]
    public float ChargesPercent { get; set; }

    [Description("CREDIT_LIFE")]
    public decimal CreditLife { get; set; }

    [Description("CREDIT_LIFE %")]
    public float CreditLifePercent { get; set; }

    [Description("LOAN_FEE_EXCL")]
    public decimal LoanFeeExclVAT { get; set; }

    [Description("LOAN_FEE_VAT")]
    public decimal LoanFeeVAT { get; set; }

    [Description("LOAN_FEE_INCL")]
    public decimal LoanFeeInclVAT { get; set; }

    [Description("LOAN_FEE %")]
    public float LoanFeePercent { get; set; }

    [Description("FUNERAL_ADD_ON")]
    public decimal FuneralAddOn { get; set; }

    [Description("AGE_ADD_ON")]
    public decimal AgeAddOn { get; set; }

    [Description("VAP_EXCL")]
    public decimal VAPExcl { get; set; }

    [Description("VAP_VAT")]
    public decimal VAPVAT { get; set; }

    [Description("VAP_INCL")]
    public decimal VAPIncl { get; set; }

    [Description("TOT_ADD_ON")]
    public decimal TotalAddOn { get; set; }

    [Description("ADD_ON%")]
    public float AddOnPercent { get; set; }

    [Description("TOT_FEE_EXCL")]
    public decimal TotFeeExcl { get; set; }

    [Description("TOT_FEE_VAT")]
    public decimal TotFeeVAT { get; set; }

    [Description("TOT_FEE_INCL")]
    public decimal TotFeeIncl { get; set; }

    [Description("TOT_FEE%")]
    public float TotFeePercent { get; set; }

    [Description("HOVRTOT")]
    public decimal HandoverTotal { get; set; }

    [Description("H/O %")]
    public float HandoverPercent { get; set; }

    [Description("LOANS")]
    public decimal Loans { get; set; }

    [Description("LN MIX")]
    public float LoanMix { get; set; }

    [Description("VAP - Linked")]
    public int VapLinkedLoans { get; set; }

    [Description("VAP - Consultant Denied With Authority")]
    public int VapDeniedByConWithAuth { get; set; }

    [Description("VAP - Consultant Denied Without Authority")]
    public int VapDeniedByConWithOutAuth { get; set; }

    [Description("VAP - Excluded")]
    public int VapExcludedLoans { get; set; }

    [Description("COLLECT")]
    public decimal Collections { get; set; }

    [Description("ROLLED VALUE")]
    public decimal RolledValue { get; set; }

    [Description("ROLLED %")]
    public float RolledPercentage { get; set; }

    [Description("REFUNDS")]
    public decimal Refunds { get; set; }

    [Description("Qty Cl New")]
    public decimal ChequeClientsNew { get; set; }

    [Description("H/O No of loans")]
    public decimal HandoverNoOfLoans { get; set; }

    [Description("H/O No of clients")]
    public decimal HandoverNoOfClients { get; set; }

    [Description("CL - BR")]
    public decimal ClientBranch { get; set; }

    [Description("CL - SR")]
    public decimal ClientSalesRep { get; set; }

    [Description("TOT - NC")]
    public long TotalNewClients { get; set; }

    [Description("CURR -CL")]
    public decimal ExistingClients { get; set; }

    [Description("CL - RIV")]
    public decimal RevivedClients { get; set; }
    
    [Description("AVE LN - RIV")]
    public decimal AverageRevivedClientsAmount { get; set; }

    [Description("NEW CL - MIX")]
    public float NewClientMix { get; set; }

    [Description("AVE LN")]
    public decimal AverageLoan { get; set; }

    [Description("NEW CL")]
    public decimal NewLoanAverage { get; set; }

    [Description("Reswipe - Bank")]
    public int ReswipeBankChange { get; set; }

    [Description("Reswipe - Term ")]
    public int ReswipeTermChange { get; set; }

    [Description("Reswipe - Instalment")]
    public int ReswipeInstalmentChange { get; set; }

    public Dictionary<string, int> CompuscanProducts { get; set; }
  }
}
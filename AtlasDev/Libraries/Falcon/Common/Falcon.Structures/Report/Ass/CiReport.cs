using System.Collections.Generic;
using System.ComponentModel;
using Falcon.Common.Interfaces.Structures;
using Falcon.Common.Interfaces.Structures.Reports.Ass;

namespace Falcon.Common.Structures.Report.Ass
{
  public class CiReport:ICiReport
  {
    public IBranch Branch { get; set; }

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

    [Description("INT_FEE")]
    public decimal InterestFee { get; set; }

    [Description("INT %")]
    public float InterestPercent { get; set; }

    [Description("INS_FEE")]
    public decimal InsuranceFee { get; set; }

    [Description("TOT_FEE %")]
    public float TotalFeePercent { get; set; }

    [Description("TOT_FEE")]
    public decimal TotalFee { get; set; }

    [Description("INS %")]
    public float InsurancePercent { get; set; }

    [Description("HOVRTOT")]
    public decimal HandoverTotal { get; set; }

    [Description("H/O %")]
    public float HandoverPercent { get; set; }

    [Description("LOANS")]
    public decimal Loans { get; set; }

    [Description("LN MIX")]
    public float LoanMix { get; set; }

    [Description("VAP - Value")]
    public decimal VapLinkedLoansValue { get; set; }

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

    [Description("Cl - H/O")]
    public decimal ClientsHandover { get; set; }

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
  }
}

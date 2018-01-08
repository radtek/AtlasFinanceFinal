using System;


namespace Atlas.Domain.DTO.ASS
{
  public class ASS_CiReportPossibleHandoverDTO
  {
    public long CiReportPossibleHandoverId { get; set; }
    public ASS_CiReportVersionDTO CiReportVersion { get; set; }
    public BRN_BranchDTO Branch { get; set; }
    public DateTime Date { get; set; }
    public decimal Arrears { get; set; }
    public DateTime OldestArrearsDate { get; set; }
    public decimal ReceivableThisMonth { get; set; }
    public decimal ReceivedThisMonth { get; set; }
    public decimal ReceivablePast { get; set; }
    public decimal ReceivedPast { get; set; }
    public decimal DebtorsBookValue { get; set; }
    public int HandedOverLoansQuantity { get; set; }
    public decimal HandedOverLoansAmount { get; set; }
    public decimal FlaggedOverdueValue { get; set; }
    public int FlaggedNoOfLoans { get; set; }
    public decimal PossibleHandOvers { get; set; }
    public decimal NextPossibleHandOvers { get; set; }
  }
}

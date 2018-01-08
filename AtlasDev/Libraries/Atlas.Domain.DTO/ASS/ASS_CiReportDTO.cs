using System;


namespace Atlas.Domain.DTO.ASS
{
  public class ASS_CiReportDTO
  {
    public long CiReportId { get; set; }
    public ASS_CiReportVersionDTO CiReportVersion { get; set; }
    public BRN_BranchDTO Branch { get; set; }
    public DateTime Date { get; set; }
    public int PayNo { get; set; }
    public int NoOfLoans { get; set; }
    public decimal Cheque { get; set; }
    public decimal ChequeToday { get; set; }
    public int BranchLoans { get; set; }
    public int SalesRepLoans { get; set; }
    public decimal Fees { get; set; }
    public decimal Insurance { get; set; }
    public int NewClientNoOfLoans { get; set; }
    public decimal NewClientAmount { get; set; }
    public int ExistingClientCount { get; set; }
    public int RevivedClientCount { get; set; }
    public decimal Collections { get; set; }
    public decimal Refunds { get; set; }
    public int ReswipeBankChange { get; set; }
    public int ReswipeLoanTermChange { get; set; }
    public int ReswipeInstalmentChange { get; set; }
    public decimal RollbackValue { get; set; }
    public decimal VapLinkedLoansValue { get; set; }
    public int VapLinkedLoans { get; set; }
    public int VapDeniedByConWithAuth { get; set; }
    public int VapDeniedByConWithOutAuth { get; set; }
    public int VapExcludedLoans { get; set; }
  }
}
using System;


namespace Atlas.Domain.DTO.ASS
{
  public class ASS_CiReportHandoverInfoDTO
  {
    public long CiReportHandoverInfoId { get; set; }
    public ASS_CiReportVersionDTO CiReportVersion { get; set; }
    public BRN_BranchDTO Branch { get; set; }
    public DateTime Date { get; set; }
    public int PayNo { get; set; }
    public int HandedOverLoansQuantity { get; set; }
    public decimal HandedOverLoansAmount { get; set; }
  }
}

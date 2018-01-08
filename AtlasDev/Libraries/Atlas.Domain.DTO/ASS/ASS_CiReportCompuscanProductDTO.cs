using System;


namespace Atlas.Domain.DTO.ASS
{
  public class ASS_CiReportCompuscanProductDTO
  {
    public long CiReportCompuscanProductId { get; set; }
    public ASS_CiReportVersionDTO CiReportVersion{ get; set; }
    public BRN_BranchDTO Branch{ get; set; }
    public DateTime Date{ get; set; }
    public int OneMonth{ get; set; }
    public int OneMonthThin{ get; set; }
    public int OneMonthCapped{ get; set; }
    public int TwoToFourMonth{ get; set; }
    public int FiveToSixMonth{ get; set; }
    public int TwelveMonth{ get; set; }
    public int Declined { get; set; }
  }
}

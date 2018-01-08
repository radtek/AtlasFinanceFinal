using System;


namespace Atlas.Domain.DTO.ASS
{
  public class ASS_CiReportScoreDTO
  {
    public long CiReportScoreId { get; set; }
    public ASS_CiReportVersionDTO CiReportVersion { get; set; }
    public BRN_BranchDTO Branch { get; set; }
    public DateTime Date { get; set; }
    public int PayNo { get; set; }
    public int ScoreAboveXWeekly { get; set; }
    public int ScoreAboveXBiWeekly { get; set; }
    public int ScoreAboveXMonthly { get; set; }
  }
}
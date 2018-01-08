namespace Atlas.Reporting.DTO
{
  public class ASS_CiReportLowMean
  {
    public long BranchId { get; set; }
    public string Name { get; set; }
    public float Target { get; set; }
    public float Achieved { get; set; }
    public float Variance { get; set; }
    public float AchievedPercent { get; set; }
    public int NoOfLoansRequired { get; set; }
  }
}
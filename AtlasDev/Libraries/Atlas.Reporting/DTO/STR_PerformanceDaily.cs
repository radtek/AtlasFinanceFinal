using System;
using System.ComponentModel;

namespace Atlas.Reporting.DTO
{
  public class STR_PerformanceDaily
  {
    public long AllocatedUserId { get; set; }
    [Description("Allocated User")]
    public string AllocatedUser { get; set; }
    [Description("Date")]
    public DateTime Date { get; set; }
    [Description("{0} Obtained")]
    public int PtpPtcObtained { get; set; }
    [Description("FollowUps")]
    public int FollowUps { get; set; }
    [Description("No Actions")]
    public int NoActionCount { get; set; }
    [Description("Escalations")]
    public int Escalations { get; set; }
  }
}

using System.ComponentModel;

namespace Falcon.Common.Structures.Report.Ass
{
  public class CiScoreBand
  {
    [Description("PAYNO")]
    public int PayNo { get; set; }
    [Description("Weekly (>= 615)")]
    public int Weekly615 { get; set; }
    [Description("Bi-Weekly (>= 615)")]
    public int BiWeekly615 { get; set; }
    [Description("Monthly (>= 615)")]
    public int Monthly615 { get; set; }
    [Description("Total (>= 615)")]
    public int Total615 { get; set; }
  }
}

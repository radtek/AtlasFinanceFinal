using Atlas.Common.Attributes;
using Atlas.Common.Utils.Ass;
using Falcon.Exporter.CiReport.Infrastructure.Attributes;

namespace Falcon.Exporter.CiReport.Infrastructure.Structures.Reports
{
  [NoHeader]
  [RowHeight]
  public class VarianceStatus
  {
    [Order(1)]
    public string VarianceTitle
    {
      get { return "Variance: "; }
    }

    [Format("#,##0;-#,##0")]
    [Order(2)]
    [ConditionalBackgroundColor("float.Parse(\"{{Variance}}\") < 0", "Red")]
    [ConditionalBackgroundColor("float.Parse(\"{{Variance}}\") >= 0", "Green")]
    public float Variance { get; set; } 

    [Format("0.00%")]
    [Order(3)]
    [ConditionalBackgroundColor("float.Parse(\"{{Variance}}\") < 0", "Red")]
    [ConditionalBackgroundColor("float.Parse(\"{{Variance}}\") >= 0", "Green")]
    public float AchievedPercent { get; set; }
  }
}
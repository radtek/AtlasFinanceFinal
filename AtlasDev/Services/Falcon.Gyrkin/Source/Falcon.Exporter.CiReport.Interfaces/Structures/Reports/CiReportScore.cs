using System.ComponentModel;
using Atlas.Common.Attributes;
using Falcon.Exporter.CiReport.Infrastructure.Attributes;

namespace Falcon.Exporter.CiReport.Infrastructure.Structures.Reports
{
  public class CiReportScore
  {
    [Description("Id")]
    public long Id { get; set; }
    [Description("Name")]
    public string Name { get; set; }
    [Description("PAYNO")]
    [Order(1)]
    public int PayNo { get; set; }
    [DetailFormat(alignment: "right")]
    [Description("Weekly (>= 615)")]
    [Order(1)]
    [DetailFormat(alignment: "right")]
    public int Weekly615 { get; set; }
    [Description("Bi-Weekly (>= 615)")]
    [Order(1)]
    [DetailFormat(alignment: "right")]
    public int BiWeekly615 { get; set; }
    [Description("Monthly (>= 615)")]
    [Order(1)]
    [DetailFormat(alignment: "right")]
    public int Monthly615 { get; set; }
    [Description("Total (>= 615)")]
    [Order(1)]
    [DetailFormat(alignment: "right")]
    public int Total615 { get; set; }
  }
}

using System.ComponentModel;
using Atlas.Common.Attributes;
using Falcon.Exporter.CiReport.Infrastructure.Attributes;

namespace Falcon.Exporter.CiReport.Infrastructure.Structures.Reports
{
  public class CiReportLowMean
  {
    public long BranchId { get; set; }

    [Description("Name")]
    [Order(1)]
    public string Name { get; set; }

    [Description("Target")]
    [Order(2)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float Target { get; set; }

    [Description("Achieved")]
    [Order(3)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float Achieved { get; set; }

    [Description("Achieved %")]
    [Order(4)]
    [Format("0.00%")]
    [DetailFormat(alignment: "right")]
    public float AchievedPercent { get; set; }

    [Description("No. of Loans Required")]
    [Order(5)]
    [DetailFormat(alignment: "right")]
    public float NoOfLoansRequired { get; set; }
  }
}
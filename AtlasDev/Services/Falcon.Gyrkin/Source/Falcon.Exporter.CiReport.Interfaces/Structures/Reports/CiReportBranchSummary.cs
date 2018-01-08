using System.ComponentModel;
using Atlas.Common.Attributes;
using Falcon.Exporter.CiReport.Infrastructure.Attributes;

namespace Falcon.Exporter.CiReport.Infrastructure.Structures.Reports
{
  public class CiReportBranchSummary
  {
    public long BranchId { get; set; }

    [Description("Branch Name")]
    [Order(1)]
    public string BranchName { get; set; }

    [Description("Cheque")]
    [Order(2)]
    [Format("#,##0;-#,##0")]
    [DetailFormat("Black", "LightGray", "right")]
    public float Cheque { get; set; }

    [Description("HOVRTOT")]
    [Order(3)]
    [Format("#,##0;-#,##0")]
    [DetailFormat("Black", "Aqua", "right")]
    public float HandedOverLoansAmount { get; set; }

    [Description("COLLECT")]
    [Order(4)]
    [Format("#,##0;-#,##0")]
    [DetailFormat("Black", "LightGreen", "right")]
    public float Collections { get; set; }
  }
}

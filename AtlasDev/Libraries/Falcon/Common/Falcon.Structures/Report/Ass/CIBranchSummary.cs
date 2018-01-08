using System.ComponentModel;

namespace Falcon.Common.Structures.Report.Ass
{
  public class CIBranchSummary
  {
    public long BranchId { get; set; }

    [Description("Branch")]
    public string BranchName { get; set; }

    [Description("CHEQUE")]
    public decimal Cheque { get; set; }

    [Description("HOVRTOT")]
    public decimal HandoverTotal { get; set; }

    [Description("COLLECT")]
    public decimal Collections { get; set; }

  }
}
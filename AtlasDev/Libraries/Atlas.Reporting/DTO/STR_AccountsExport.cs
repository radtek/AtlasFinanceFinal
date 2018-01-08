using System.ComponentModel;

namespace Atlas.Reporting.DTO
{
  public class STR_AccountsExport
  {
    [Description("Case No.")]
    public long CaseId { get; set; }
    [Description("Reference No.")]
    public string AccountReference { get; set; }
    [Description("Client Name")]
    public string ClientName { get; set; }
    [Description("ID Number")]
    public string IdNumber { get; set; }
    [Description("Priority")]
    public string Priority { get; set; }
    [Description("Status")]
    public string Status { get; set; }
    [Description("Allocated User")]
    public string AllocatedUser { get; set; }
  }
}

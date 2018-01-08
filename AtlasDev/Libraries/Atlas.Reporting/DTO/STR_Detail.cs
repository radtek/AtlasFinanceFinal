using System.ComponentModel;

namespace Atlas.Reporting.DTO
{
  public class STR_Detail
  {
    [Description("Case No.")]
    public string CaseNo { get; set; }
    [Description("Status")]
    public string CaseStatus { get; set; }
    [Description("Current Stream")]
    public string Stream { get; set; }
    [Description("Category")]
    public string Category { get; set; }
    [Description("Id Number")]
    public string IdNumber { get; set; }
    [Description("First Name")]
    public string FirstName { get; set; }
    [Description("Last Name")]
    public string LastName { get; set; }
  }
}

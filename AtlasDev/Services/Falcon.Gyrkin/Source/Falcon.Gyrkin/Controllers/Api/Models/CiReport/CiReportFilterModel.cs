using System;

namespace Falcon.Gyrkin.Controllers.Api.Models.CiReport
{
  public class CiReportFilterModel
  {
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public long[] BranchIds { get; set; }
  }
  public class EmailCiReportFilterModel
  {
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public long[] BranchIds { get; set; }
    public string Email { get; set; }
    public string UserId { get; set; }
  }
}

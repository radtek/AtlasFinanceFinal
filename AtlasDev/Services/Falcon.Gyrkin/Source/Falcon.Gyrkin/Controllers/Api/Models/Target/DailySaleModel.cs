using System;

namespace Falcon.Gyrkin.Controllers.Api.Models.Target
{
  public class DailySaleModel
  {
    public long BranchId { get; set; }
    public long HostId { get; set; }
    public DateTime TargetDate { get; set; }
    public decimal Amount { get; set; }
    public float Percent { get; set; }
    public long UserId { get; set; }
  }
}
using System;

namespace Falcon.Gyrkin.Controllers.Api.Models.Target
{
  public class HandoverTargetModel
  {
    public long HandoverTargetId { get; set; }
    public long BranchId { get; set; }
    public long HostId { get; set; }
    public DateTime TargetDate { get; set; }
    public decimal HandoverBudget { get; set; }
    public float ArrearTarget { get; set; }
    public long UserId { get; set; }
  }
}

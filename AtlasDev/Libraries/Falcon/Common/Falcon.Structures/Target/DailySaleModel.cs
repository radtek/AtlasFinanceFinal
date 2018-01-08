using System;
using Falcon.Common.Interfaces.Structures.Target;

namespace Falcon.Common.Structures.Target
{
  public class DailySaleModel : IDailySaleModel
  {
    public long DailySaleId { get; set; }
    public long BranchId { get; set; }
    public long HostId { get; set; }
    public DateTime TargetDate { get; set; }
    public decimal Amount { get; set; }
    public float Percent { get; set; }
    public long UserId { get; set; }
  }
}

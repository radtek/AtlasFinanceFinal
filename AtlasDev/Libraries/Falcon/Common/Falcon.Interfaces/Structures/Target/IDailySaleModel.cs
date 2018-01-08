using System;

namespace Falcon.Common.Interfaces.Structures.Target
{
  public interface IDailySaleModel
  {
    long DailySaleId { get; set; }
    long BranchId { get; set; }
    long HostId { get; set; }
    DateTime TargetDate { get; set; }
    decimal Amount { get; set; }
    float Percent { get; set; }
    long UserId { get; set; }
  }
}
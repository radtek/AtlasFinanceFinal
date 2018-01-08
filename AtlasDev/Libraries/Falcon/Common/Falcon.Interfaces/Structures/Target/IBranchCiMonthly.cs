using System;

namespace Falcon.Common.Interfaces.Structures.Target
{
  public interface IBranchCiMonthly
  {
    long BranchId { get; set; }
    string BranchDescription { get; set; }
    long HostId { get; set; }
    string HostDescription { get; set; }
    DateTime TargetDate { get; set; }
    decimal Amount { get; set; }
    float Percent { get; set; }
    string CreateUser { get; set; }
  }
}
using System;
using Falcon.Common.Interfaces.Structures.Target;

namespace Falcon.Common.Structures.Target
{
  public class BranchCiMonthly : IBranchCiMonthly
  {
    public long BranchId { get; set; }
    public string BranchDescription { get; set; }
    public long HostId { get; set; }
    public string HostDescription { get; set; }
    public DateTime TargetDate { get; set; }
    public decimal Amount { get; set; }
    public float Percent { get; set; }
    public string CreateUser { get; set; }
  }
}

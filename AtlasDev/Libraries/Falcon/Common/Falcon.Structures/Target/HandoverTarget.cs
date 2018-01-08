using System;
using Falcon.Common.Interfaces.Structures.Target;

namespace Falcon.Common.Structures.Target
{
  public class HandoverTarget : IHandoverTarget
  {
    public long BranchId { get; set; }
    public string BranchDescription { get; set; }
    public long HostId { get; set; }
    public string Host { get; set; }
    public DateTime TargetDate { get; set; }
    public decimal HandoverBudget { get; set; }
    public float ArrearTarget { get; set; }
  }
}
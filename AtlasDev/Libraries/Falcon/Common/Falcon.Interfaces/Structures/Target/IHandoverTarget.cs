using System;

namespace Falcon.Common.Interfaces.Structures.Target
{
  public interface IHandoverTarget
  {
    long BranchId { get; set; }
    string BranchDescription { get; set; }
    long HostId { get; set; }
    string Host { get; set; }
    DateTime TargetDate { get; set; }
    decimal HandoverBudget { get; set; }
    float ArrearTarget { get; set; }
  }
}
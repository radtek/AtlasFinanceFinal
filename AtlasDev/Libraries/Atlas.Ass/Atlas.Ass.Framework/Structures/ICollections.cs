using System;

namespace Atlas.Ass.Framework.Structures
{
  public interface ICollections
  {
    string LegacyBranchNumber { get; set; }
    DateTime? OldestArrearDate { get; set; }
    decimal ReceivableThisMonth { get; set; }
    decimal ReceivedThisMonth { get; set; }
    decimal ReceivablePast { get; set; }
    decimal ReceivedPast { get; set; }
  }
}
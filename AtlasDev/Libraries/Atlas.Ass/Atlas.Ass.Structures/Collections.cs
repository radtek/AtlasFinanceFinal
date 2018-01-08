using System;
using Atlas.Ass.Framework.Structures;

namespace Atlas.Ass.Structures
{
  public class Collections : ICollections
  {
    public string LegacyBranchNumber { get; set; }
    public DateTime? OldestArrearDate { get; set; }
    public decimal ReceivableThisMonth { get; set; }
    public decimal ReceivedThisMonth { get; set; }
    public decimal ReceivablePast { get; set; }
    public decimal ReceivedPast { get; set; }
  }
}
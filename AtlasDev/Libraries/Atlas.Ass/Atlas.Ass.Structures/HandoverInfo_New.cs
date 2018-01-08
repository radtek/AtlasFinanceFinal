using System;
using Atlas.Ass.Framework.Structures;

namespace Atlas.Ass.Structures
{
  public class HandoverInfo_New : IHandoverInfo_New
  {
    public string LegacyBranchNumber { get; set; }
    public DateTime HandoverDate { get; set; }
    public int PayNo { get; set; }
    public int Quantity { get; set; }
    public decimal Amount { get; set; }
    public int ClientQuantity { get; set; }
  }
}

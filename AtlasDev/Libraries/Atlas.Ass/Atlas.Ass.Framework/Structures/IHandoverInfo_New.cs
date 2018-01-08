using System;

namespace Atlas.Ass.Framework.Structures
{
  public interface IHandoverInfo_New
  {
    string LegacyBranchNumber { get; set; }
    DateTime HandoverDate { get; set; }
    int PayNo { get; set; }
    int Quantity { get; set; }
    decimal Amount { get; set; }
    int ClientQuantity { get; set; }
  }
}

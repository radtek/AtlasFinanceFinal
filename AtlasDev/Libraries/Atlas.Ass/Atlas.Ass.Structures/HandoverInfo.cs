using Atlas.Ass.Framework.Structures;

namespace Atlas.Ass.Structures
{
  public class HandoverInfo : IHandoverInfo
  {
    public string LegacyBranchNumber { get; set; }
    public int PayNo { get; set; }
    public int Quantity { get; set; }
    public decimal Amount { get; set; }
    public decimal ClientQuantity { get; set; }
  }
}

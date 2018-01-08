using Atlas.Ass.Framework.Structures;

namespace Atlas.Ass.Structures
{
  public class CollectionRefund : ICollectionRefund
  {
    public string LegacyBranchNumber { get; set; }
    public decimal PayNo { get; set; }
    public decimal Collections { get; set; }
    public decimal Refunds { get; set; }
  }
}

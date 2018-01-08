namespace Atlas.Ass.Framework.Structures
{
  public interface ICollectionRefund
  {
    string LegacyBranchNumber { get; set; }
    decimal PayNo { get; set; }
    decimal Collections { get; set; }
    decimal Refunds { get; set; }
  }
}

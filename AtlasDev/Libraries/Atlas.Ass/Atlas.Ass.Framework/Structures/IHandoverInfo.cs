namespace Atlas.Ass.Framework.Structures
{
  public interface IHandoverInfo
  {
    string LegacyBranchNumber { get; set; }
    int PayNo { get; set; }
    int Quantity { get; set; }
    decimal Amount { get; set; }
    decimal ClientQuantity { get; set; }
  }
}

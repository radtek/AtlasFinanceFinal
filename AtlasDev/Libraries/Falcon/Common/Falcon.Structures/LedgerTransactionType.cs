namespace Falcon.Common.Structures
{
  public class LedgerTransactionType
  {
    public int TransactionTypeId { get; set; }
    public string Description { get; set; }
    public int TransactionTypeGroupId { get; set; }
    public string TransactionTypeGroup { get; set; }
    public int SortKey { get; set; }
  }
}

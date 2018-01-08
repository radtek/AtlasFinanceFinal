namespace Stream.Framework.Structures
{
  public interface ITransactionType
  {
    int TransactionTypeId { get; set; }
    Enumerators.Stream.TransactionType Type { get; set; }
    string Description { get; set; }
  }
}
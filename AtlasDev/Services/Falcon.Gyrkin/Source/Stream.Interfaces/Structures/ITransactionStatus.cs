namespace Stream.Framework.Structures
{
  public interface ITransactionStatus
  {
    int TransactionStatusId { get; set; }
    Enumerators.Stream.TransactionStatus Status { get; set; }
    string Description { get; set; }
  }
}
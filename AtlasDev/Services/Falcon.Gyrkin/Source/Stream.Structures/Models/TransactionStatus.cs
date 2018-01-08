using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public class TransactionStatus : ITransactionStatus
  {
    public int TransactionStatusId { get; set; }

    public Framework.Enumerators.Stream.TransactionStatus Status { get; set; }

    public string Description { get; set; }
  }
}
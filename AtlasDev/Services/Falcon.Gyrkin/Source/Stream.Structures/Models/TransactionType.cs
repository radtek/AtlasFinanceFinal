using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public class TransactionType : ITransactionType
  {
    public int TransactionTypeId { get; set; }

    public Framework.Enumerators.Stream.TransactionType Type { get; set; }

    public string Description { get; set; }
  }
}
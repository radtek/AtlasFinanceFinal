using System;

namespace Stream.Framework.DataContracts.Requests
{
  public class AddOrUpdateAccountTransactionRequest
  {
    public long TransactionId { get; set; }
    public long AccountId { get; set; }
    public long Reference { get; set; }
    public DateTime TransactionDate { get; set; }
    public Enumerators.Stream.TransactionType TransactionType { get; set; }
    public Enumerators.Stream.TransactionStatus? TransactionStatus { get; set; }
    public decimal Amount { get; set; }
    public int InstalmentNumber { get; set; }
  }
}

using System;

namespace Stream.Framework.Structures
{
  public interface ITransaction
  {
    long TransactionId { get; set; }
    long AccountId { get; set; }
    string AccountReference { get; set; }
    long Reference { get; set; }
    DateTime TransactionDate { get; set; }
    Enumerators.Stream.TransactionType TransactionType { get; set; }
    Enumerators.Stream.TransactionStatus? TransactionStatus { get; set; }
    decimal Amount { get; set; }
    int InstalmentNumber { get; set; }
    DateTime CreateDate { get; set; }
  }
}
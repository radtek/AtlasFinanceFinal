using System;
using Atlas.Common.Extensions;
using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public class Transaction : ITransaction
  {
    public long TransactionId { get; set; }
    public long AccountId { get; set; }
    public string AccountReference { get; set; }
    public long Reference { get; set; }
    public DateTime TransactionDate { get; set; }
    public Framework.Enumerators.Stream.TransactionType TransactionType { get; set; }

    public string TransactionTypeDescription
    {
      get { return TransactionType.ToStringEnum(); }
    }

    public Framework.Enumerators.Stream.TransactionStatus? TransactionStatus { get; set; }

    public string TransactionStatusDescription
    {
      get { return TransactionStatus.HasValue ? TransactionStatus.ToStringEnum() : string.Empty; }
    }

    public decimal Amount { get; set; }
    public int InstalmentNumber { get; set; }
    public DateTime CreateDate { get; set; }
  }
}
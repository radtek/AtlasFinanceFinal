using System;

namespace Atlas.Ass.Framework.Structures.Stream
{
  public interface IAccountTransaction
  {
    long LoanReference { get; set; }
    long TransactionReference { get; set; }
    decimal Amount { get; set; }
    int Order { get; set; }
    int Seqno { get; set; }
    DateTime TransactionDate { get; set; }
    string TransactionType { get; set; }
    string TranasctionStatus { get; set; }
    DateTime? StatusDate { get; set; }
    DateTime BackupDate { get; set; }
  }
}
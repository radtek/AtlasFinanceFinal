using System;
using Atlas.Ass.Framework.Structures.Stream;

namespace Atlas.Ass.Structures.Stream
{
  public class AccountTransaction : IAccountTransaction
  {
    public long LoanReference { get; set; }
    public long TransactionReference { get; set; }
    public decimal Amount { get; set; }
    public int Order { get; set; }
    public int Seqno { get; set; }
    public DateTime TransactionDate { get; set; }
    public string TransactionType { get; set; }
    public string TranasctionStatus { get; set; }
    public DateTime? StatusDate { get; set; }
    public DateTime BackupDate { get; set; }
  }
}

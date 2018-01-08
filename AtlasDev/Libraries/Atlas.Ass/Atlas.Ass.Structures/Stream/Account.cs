using System;
using Atlas.Ass.Framework.Structures.Stream;

namespace Atlas.Ass.Structures.Stream
{
  public class Account : IAccount
  {
    public long LoanReference { get; set; }
    public string LegacyBranchNumber { get; set; }
    public string Client { get; set; }
    public string Loan { get; set; }
    public string Status { get; set; }
    public string StatusDescription { get; set; }
    public DateTime PaidDate { get; set; }
  }
}

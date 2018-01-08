using System;

namespace Atlas.Ass.Framework.Structures.Stream
{
  public interface IAccount
  {
    long LoanReference { get; set; }
    string LegacyBranchNumber { get; set; }
    string Client { get; set; }
    string Loan { get; set; }
    string Status { get; set; }
    string StatusDescription { get; set; }
    DateTime PaidDate { get; set; }
  }
}
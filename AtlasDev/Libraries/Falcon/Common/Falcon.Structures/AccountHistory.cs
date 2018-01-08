using System;

namespace Falcon.Common.Structures
{
  public sealed class AccountHistory
  {
    public long AccountId { get; set; }
    public long PersonId { get; set; }
    public string Host { get; set; }
    public string AccountType{ get; set; }
    public string AccountNo { get; set; }
    public string Status { get; set; }
    public decimal LoanAmount { get; set; }
    public decimal? Balance { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? OpenDate { get; set; }
    public DateTime LastStatusDate { get; set; }
  }
}
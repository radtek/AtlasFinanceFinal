using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Domain.Structures
{
  /// <summary>
  /// Account info for a specific account
  /// </summary>
  public class AccountInfo
  {
    public long AccountId { get; set; }
    public string AccountNo { get; set; }
    public Enumerators.Account.AccountStatus Status { get; set; }
    public Enumerators.Account.AccountStatusReason? StatusReason { get; set; }
    public Enumerators.Account.AccountStatusSubReason? StatusSubReason { get; set; }
    public int Period { get; set; }
    public float InterestRate { get; set; }
    public decimal LoanAmount { get; set; }
    public decimal RepaymentAmount { get; set; }
    public decimal Balance { get; set; }
    public DateTime? OpenDate { get; set; }
    public DateTime RepaymentDate { get; set; }
    public DateTime FirstInstalmentDate { get; set; }
    public decimal FirstRepaymentAmount { get; set; }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Domain.Structures
{
  public class BankDetail
  {
    public long BankDetailId { get; set; }
    public string BankAccountName { get; set; }
    public string BankAccountNumber { get; set; }
    public string BankBranchCode { get; set; }
    public string BankAccountType { get; set; }
    public long BankAccountTypeId { get; set; }
    public string BankName { get; set; }
    public long BankId { get; set; }
  }
}

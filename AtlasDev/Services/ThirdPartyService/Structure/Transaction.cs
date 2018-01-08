using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.ThirdParty.Service.Structure
{
  public sealed class Transaction : BaseStructure
  {
    public decimal Amount { get; set; }
    public Atlas.ThirdParty.Service.Structure.Enums.Bank Bank { get; set; }
    public string BankAccountName { get; set; }
    public string BankAccountNo { get; set; }
    public Atlas.ThirdParty.Service.Structure.Enums.BankAccountType BankAccountType { get; set; }
    public string BankBranchCode { get; set; }
    public string BankStatementReference { get; set; }
    public string IdNumber { get; set; }
    public DateTime InstalmentDate { get; set; }
    public string ThirdPartyReference { get; set; }
    public Atlas.ThirdParty.Service.Structure.Enums.TrackingDay TrackingDay { get; set; }
    public Atlas.ThirdParty.Service.Structure.Enums.PayRule? PayRule { get; set; }

  }
}

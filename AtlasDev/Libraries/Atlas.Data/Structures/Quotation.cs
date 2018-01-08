using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Domain.Structures
{
  public class Quotation
  {
    public long QuotationId { get; set; }
    public long AccountId { get; set; }
    public string AccountNo { get; set; }
    public string QuotationNo { get; set; }
    public DateTime QuoteDate { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string IdNumber { get; set; }
    public string ContactNumber { get; set; }
    public string ResidentialAddressLine1 { get; set; }
    public string ResidentialAddressLine2 { get; set; }
    public string ResidentialAddressLine3 { get; set; }
    public string ResidentialAddressLine4 { get; set; }
    public string ResidentialAddressCode { get; set; }
    public decimal Amount { get; set; }
    public decimal InitiationFee { get; set; }
    public decimal ServiceFee { get; set; }
    public decimal InterestRate { get; set; }
    public decimal RepaymentAmount { get; set; }
    public DateTime RepaymentDate { get; set; }
    public Enumerators.General.BankName Bank { get; set; }
    public string BankBranch { get; set; }
    public decimal DebitAmount { get; set; }
    public string BankAccountNo { get; set; }
    public string BankAccountName { get; set; }
    public Enumerators.General.BankAccountType BankAccountType { get; set; }
    public DateTime DateOfDebit { get; set; }
  }
}
using System;
using Falcon.Common.Interfaces.Structures.AVS;

namespace Falcon.Common.Structures.Avs
{
  public sealed class AvsTransactions : IAvsTransactions
  {
    public long TransactionId { get; set; }
    public long? BatchId { get; set; }
    public string Initials { get; set; }
    public string LastName { get; set; }
    public string IdNumber { get; set; }
    public long BankId { get; set; }
    public string Bank { get; set; }
    public string BranchCode { get; set; }
    public long AccountId { get; set; }
    public string AccountNo { get; set; }
    public string Service { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? ResponseDate { get; set; }
    public string ResponseAccountNumber { get; set; }
    public string ResponseIdNumber { get; set; }
    public string ResponseInitials { get; set; }
    public string ResponseLastName { get; set; }
    public string ResponseAccountOpen { get; set; }
    public string ResponseAcceptsDebit { get; set; }
    public string ResponseAcceptsCredit { get; set; }
    public string ResponseOpenThreeMonths { get; set; }
    public string ThirdPartyRef { get; set; }
    public string StatusColor { get; set; }
    public int StatusId { get; set; }
    public string Status { get; set; }
    public DateTime LastStatusDate { get; set; }
    public long CompanyId { get; set; }
    public string Company { get; set; }
    public string StatusIcon { get; set; }
    public string Result { get; set; }
    public string ResultColour { get; set; }
  }
}
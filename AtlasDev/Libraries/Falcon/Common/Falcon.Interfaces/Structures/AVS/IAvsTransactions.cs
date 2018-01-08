using System;

namespace Falcon.Common.Interfaces.Structures.AVS
{
  public interface IAvsTransactions
  {
    long TransactionId { get; set; }
    long? BatchId { get; set; }
    string Initials { get; set; }
    string LastName { get; set; }
    string IdNumber { get; set; }
    long BankId { get; set; }
    string Bank { get; set; }
    string BranchCode { get; set; }
    long AccountId { get; set; }
    string AccountNo { get; set; }
    string Service { get; set; }
    DateTime CreateDate { get; set; }
    DateTime? ResponseDate { get; set; }
    string ResponseAccountNumber { get; set; }
    string ResponseIdNumber { get; set; }
    string ResponseInitials { get; set; }
    string ResponseLastName { get; set; }
    string ResponseAccountOpen { get; set; }
    string ResponseAcceptsDebit { get; set; }
    string ResponseAcceptsCredit { get; set; }
    string ResponseOpenThreeMonths { get; set; }
    string ThirdPartyRef { get; set; }
    string StatusColor { get; set; }
    int StatusId { get; set; }
    string Status { get; set; }
    DateTime LastStatusDate { get; set; }
    long CompanyId { get; set; }
    string Company { get; set; }
    string StatusIcon { get; set; }
    string Result { get; set; }
    string ResultColour { get; set; }
  }
}
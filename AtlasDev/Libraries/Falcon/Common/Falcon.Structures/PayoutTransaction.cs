using System;

namespace Falcon.Common.Structures
{
  public sealed class PayoutTransaction
  {
    public long PayoutId { get; set; }
    public string ServiceType { get; set; }
    public decimal Amount { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime CreateDate { get; set; }
    public int PayoutStatusId { get; set; }
    public string PayoutStatus { get; set; }
    public string PayoutStatusColor { get; set; }
    public long? BatchId { get; set; }
    public int? BatchStatusId { get; set; }
    public string BatchStatus { get; set; }
    public long? BranchId { get; set; }
    public string BranchName { get; set; }
    public string IdNumber { get; set; }
    public string FirstName { get; set; }
    public string Surname { get; set; }
    public long BankId { get; set; }
    public string Bank { get; set; }
    public long BankAccountTypeId { get; set; }
    public string BankAccountType { get; set; }
    public string BankAccountNo { get; set; }
    public string BankAccountName { get; set; }
    public string BankBranchCode { get; set; }
    public string Result { get; set; }
    public bool IsValid { get; set; }
  }
}

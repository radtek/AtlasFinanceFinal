using System;
using Atlas.Enumerators;

namespace Falcon.Common.Interfaces.Structures
{
  public interface IDebitOrderTransaction
  {
    long TransactionNo { get; set; }
    long ControlId { get; set; }
    string IdNumber { get; set; }
    string Bank { get; set; }
    string BankAccountNo { get; set; }
    string BankAccountName { get; set; }
    string BankStatementReference { get; set; }
    Debit.Status? Status { get; set; }
    string StatusDescription { get; set; }
    int Repetition { get; set; }
    DateTime InstalmentDate { get; set; }
    DateTime ActionDate { get; set; }
    string ReplyCode { get; set; }
    string ReplyCodeDescription { get; set; }
    string ResponseCode { get; set; }
    string ResponseCodeDescription { get; set; }
    decimal Amount { get; set; }
    DateTime? CancelDate { get; set; }
    decimal? OverrideAmount { get; set; }
    DateTime? OverrideActionDate { get; set; }
    int? OverrideTrackingDays { get; set; }
  }
}

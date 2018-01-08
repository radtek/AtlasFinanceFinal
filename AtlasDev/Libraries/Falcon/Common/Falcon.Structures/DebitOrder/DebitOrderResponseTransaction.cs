using System;
using Atlas.Enumerators;
using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Structures.DebitOrder
{
  public class DebitOrderResponseTransaction : IDebitOrderTransaction
  {
    public long TransactionNo { get; set; }
    public long ControlId { get; set; }
    public string IdNumber { get; set; }
    public string Bank { get; set; }
    public string BankAccountNo { get; set; }
    public string BankAccountName { get; set; }
    public string BankStatementReference { get; set; }
    public Debit.Status? Status { get; set; }
    public string StatusDescription { get; set; }
    public int Repetition { get; set; }
    public DateTime InstalmentDate { get; set; }
    public DateTime ActionDate { get; set; }
    public string ReplyCode { get; set; }
    public string ReplyCodeDescription { get; set; }
    public string ResponseCode { get; set; }
    public string ResponseCodeDescription { get; set; }
    public decimal Amount { get; set; }
    public DateTime? CancelDate { get; set; }
    public decimal? OverrideAmount { get; set; }
    public DateTime? OverrideActionDate { get; set; }
    public int? OverrideTrackingDays { get; set; }
  }
}

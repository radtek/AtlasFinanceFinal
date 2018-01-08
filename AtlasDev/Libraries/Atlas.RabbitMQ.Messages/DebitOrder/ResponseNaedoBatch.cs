using System;
using System.Collections.Generic;


namespace Atlas.RabbitMQ.Messages.DebitOrder
{
  public class ResponseNaedoBatch : IMessage
  {
    public ResponseNaedoBatch(Guid correlationId, long? messageId = null)
    {
      CorrelationId = correlationId;
      MessageId = messageId;
      CreatedAt = DateTime.Now;
    }


    public long? MessageId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CorrelationId { get; set; }
    public bool HasError { get; set; }
    public string ErrorMessage { get; set; }
    public List<NaedoBatch> NaedoBatches { get; set; }
  }


  public class NaedoBatch
  {
    public long BatchId { get; set; }
    public Atlas.Enumerators.Debit.BatchStatus BatchStatus { get; set; }
    public string BatchStatusDescription { get; set; }
    public DateTime? LastStatusDate { get; set; }
    public DateTime? SubmitDate { get; set; }
    public DateTime CreateDate { get; set; }
    public int TransmissionNo { get; set; }
    public bool? TransmissionAccepted { get; set; }
    public List<NaedoBatchTransaction> NaedoBatchTransactions { get; set; }
  }


  public class NaedoBatchTransaction
  {
    public long TransactionNo { get; set; }
    public long ControlId { get; set; }
    public string IdNumber { get; set; }
    public string Bank { get; set; }
    public string BankAccountNo { get; set; }
    public string BankAccountName { get; set; }
    public string BankStatementReference { get; set; }
    public Atlas.Enumerators.Debit.Status Status { get; set; }
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
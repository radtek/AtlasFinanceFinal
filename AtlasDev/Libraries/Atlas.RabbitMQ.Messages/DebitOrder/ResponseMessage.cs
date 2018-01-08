using System;
using System.Collections.Generic;


namespace Atlas.RabbitMQ.Messages.DebitOrder
{
  public class ResponseMessage : IMessage
  {
    public ResponseMessage(Guid correlationId, long? messageId = null)
    {
      CorrelationId = correlationId;
      MessageId = messageId;
      CreatedAt = DateTime.Now;
    }


    public Type RequestMessageType { get; set; }
    //public string RequestMessage { get; set; } //Json object of request message
    public bool HasError { get; set; }
    public string ErrorMessage { get; set; }
    public long? MessageId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CorrelationId { get; set; }
    public ResponseControl Response { get; set; }

  }


  public class ResponseControl
  {
    public long ControlId { get; set; }
    public string IdNumber { get; set; }
    public string ThirdPartyReference { get; set; }
    public long BankId { get; set; }
    public string Bank { get; set; }
    public string BankBranchCode { get; set; }
    public string BankAccountNo { get; set; }
    public string BankAccountName { get; set; }
    public long BankAccountTypeId { get; set; }
    public string BankAccountType { get; set; }
    public string BankStatementReference { get; set; }
    public Atlas.Enumerators.Debit.ControlType ControlType { get; set; }
    public Atlas.Enumerators.Debit.ControlStatus ControlStatus { get; set; }
    public Atlas.Enumerators.Debit.FailureType FailureType { get; set; }
    public Atlas.Enumerators.Debit.TrackingDay TrackingDay { get; set; }
    public int TrackingDays { get; set; }
    public Atlas.Enumerators.Debit.AVSCheckType AVSCheckType { get; set; }
    public int Repetitions { get; set; }
    public int CurrentRepetition { get; set; }
    public decimal Instalment { get; set; }
    public DateTime LastInstalmentUpdate { get; set; }
    public Atlas.Enumerators.Account.PeriodFrequency Frequency { get; set; }
    public Atlas.Enumerators.Account.PayRule PayRule { get; set; }
    public Atlas.Enumerators.Account.PayDateType PayDateType { get; set; }// Either PayDateDayOfWeek or DateOfMonth MUST be populated. Both cannot be null
    public DayOfWeek? PayDateDayOfWeek { get; set; } // will not be null if paydatetype is DayOfWeek
    public int? PayDateDayOfMonth { get; set; } // will not be null if paydatetype is DayOfMonth
    public DateTime FirstInstalmentDate { get; set; }
    public List<Atlas.Enumerators.Debit.ValidationType> ValidationErrors { get; set; }
    public List<ResponseTransaction> ResponseTransactions { get; set; }
  }


  public class ResponseTransaction
  {
    public long TransactionNo { get; set; }
    public Atlas.Enumerators.Debit.Status Status { get; set; }
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
    public ResponseTransmissionBatch ResponseBatch { get; set; }
  }


  public class ResponseTransmissionBatch
  {
    public long BatchId { get; set; }
    public Atlas.Enumerators.Debit.BatchStatus BatchStatus { get; set; }
    public DateTime? LastStatusDate { get; set; }
    public DateTime? SubmitDate { get; set; }
    public DateTime CreateDate { get; set; }
    public int TransmissionNo { get; set; }
    public bool? TransmissionAccepted { get; set; }
  }

}
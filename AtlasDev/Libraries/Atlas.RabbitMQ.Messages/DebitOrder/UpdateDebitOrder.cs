using System;


namespace Atlas.RabbitMQ.Messages.DebitOrder
{
  public class UpdateDebitOrder : IMessage
  {
    public UpdateDebitOrder(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.Now;
    }


    public long? MessageId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CorrelationId { get; set; }
    public long ControlId { get; set; }
    public Atlas.Enumerators.General.BankName? Bank { get; set; }
    public string BankBranchCode { get; set; }
    public string BankAccountNo { get; set; }
    public Atlas.Enumerators.General.BankAccountType? BankAccountType { get; set; }
    public string BankAccountName { get; set; }
    public Atlas.Enumerators.Debit.FailureType? FailureType { get; set; }
    public Atlas.Enumerators.Debit.TrackingDay? TrackingDays { get; set; }
    public Atlas.Enumerators.Debit.AVSCheckType? AVSCheckType { get; set; }
    public decimal? Instalment { get; set; }
    public Atlas.Enumerators.Account.PeriodFrequency? Frequency { get; set; }
    public Atlas.Enumerators.Account.PayRule? PayRule { get; set; }
    public Atlas.Enumerators.Account.PayDateType? PayDateType { get; set; } // Either PayDateDayOfWeek or DateOfMonth MUST be populated. Both cannot be null
    public DayOfWeek? PayDateDayOfWeek { get; set; } // will not be null if paydatetype is DayOfWeek
    public int? PayDateDayOfMonth { get; set; } // will not be null if paydatetype is DayOfMonth
    public DateTime? OverrideNextActionDate { get; set; } // will not be null if paydatetype is DayOfMonth
  }

}
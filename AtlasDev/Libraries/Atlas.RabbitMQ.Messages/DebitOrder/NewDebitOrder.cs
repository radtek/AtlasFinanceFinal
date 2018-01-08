using System;
using System.Collections.Generic;

using Atlas.Enumerators;


namespace Atlas.RabbitMQ.Messages.DebitOrder
{
  public class NewDebitOrder : IMessage
  {
    public NewDebitOrder(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.Now;
    }


    public long? MessageId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CorrelationId { get; set; }
    public General.Host Host { get; set; }
    public long? BranchId { get; set; } // CPY_company
    public string ThirdPartyReference { get; set; }
    public string BankStatementReference { get; set; }
    public General.BankName Bank { get; set; }
    public string IdNumber { get; set; }
    public string BankBranchCode { get; set; }
    public string BankAccountNo { get; set; }
    public General.BankAccountType BankAccountType { get; set; }
    public string BankAccountName { get; set; }
    public Debit.FailureType FailureType { get; set; }
    public Debit.ControlType ControlType { get; set; }
    public Debit.TrackingDay TrackingDays { get; set; }
    public Debit.AVSCheckType AVSCheckType { get; set; }
    public int Repetitions { get; set; }
    public decimal Instalment { get; set; }
    public Account.PeriodFrequency Frequency { get; set; }
    public Account.PayRule PayRule { get; set; }
    public Account.PayDateType PayDateType { get; set; } // Either PayDateDayOfWeek or DateOfMonth MUST be populated. Both cannot be null
    public DayOfWeek? PayDateDayOfWeek { get; set; } // will not be null if paydatetype is DayOfWeek
    public int? PayDateDayOfMonth { get; set; } // will not be null if paydatetype is DayOfMonth
    public DateTime FirstInstalmentDate { get; set; }
    public long? CreatePersonId { get; set; }
    public List<Transcation> Transactions { get; set; }
  }

  public class Transcation
  {
    public int Repetition { get; set; }
    public decimal Instalment { get; set; }
    public DateTime InstalmentDate { get; set; }
    public DateTime ActionDate { get; set; }
  }
}
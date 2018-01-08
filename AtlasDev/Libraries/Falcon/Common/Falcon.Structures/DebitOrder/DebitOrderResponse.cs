using System;
using System.Collections.Generic;
using Atlas.Enumerators;
using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Structures.DebitOrder
{
  public class DebitOrderResponse 
  {
    public long ControlId { get; set; }
    public string IdNumber { get; set; }
    public bool HasError { get; set; }
    public string ErrorMessage { get; set; }
    public string ThirdPartyReference { get; set; }
    public long BankId { get; set; }
    public string Bank { get; set; }
    public string BankBranchCode { get; set; }
    public string BankAccountNo { get; set; }
    public string BankAccountName { get; set; }
    public long BankAccountTypeId { get; set; }
    public string BankAccountType { get; set; }
    public string BankStatementReference { get; set; }
    public Debit.ControlType? ControlType { get; set; }
    public string ControlTypeDescription { get; set; }
    public Debit.ControlStatus? ControlStatus { get; set; }
    public string ControlStatusDescription { get; set; }
    public Debit.FailureType? FailureType { get; set; }
    public string FailureTypeDescription { get; set; }
    public Debit.TrackingDay? TrackingDays { get; set; }
    public string TrackingDaysDescription { get; set; }
    public Debit.AVSCheckType? AvsCheckType { get; set; }
    public string AvsCheckTypeDescription { get; set; }
    public int Repetitions { get; set; }
    public int CurrentRepetition { get; set; }
    public decimal Instalment { get; set; }
    public DateTime LastInstalmentUpdate { get; set; }
    public Atlas.Enumerators.Account.PeriodFrequency? Frequency { get; set; }
    public string FrequencyDescription { get; set; }
    public Atlas.Enumerators.Account.PayRule? PayRule { get; set; }
    public string PayRuleDescription { get; set; }
    public Atlas.Enumerators.Account.PayDateType? PayDateType { get; set; }// Either PayDateDayOfWeek or DateOfMonth MUST be populated. Both cannot be null
    public string PayDateTypeDescription { get; set; }// Either PayDateDayOfWeek or DateOfMonth MUST be populated. Both cannot be null
    public DayOfWeek? PayDateDayOfWeek { get; set; } // will not be null if paydatetype is DayOfWeek
    public int? PayDateDayOfMonth { get; set; } // will not be null if paydatetype is DayOfMonth
    public DateTime FirstInstalmentDate { get; set; }
    public List<Debit.ValidationType> ValidationErrors { get; set; }
    public List<IDebitOrderTransaction> ResponseTransactions { get; set; }
  }
}

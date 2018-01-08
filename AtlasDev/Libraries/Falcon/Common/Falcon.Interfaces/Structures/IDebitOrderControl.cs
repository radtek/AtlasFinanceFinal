using System;
using System.Collections.Generic;
using Atlas.Enumerators;

namespace Falcon.Common.Interfaces.Structures
{
  public interface IDebitOrderControl
  {
    long ControlId { get; set; }
    string IdNumber { get; set; }
    bool HasError { get; set; }
    string ErrorMessage { get; set; }
    string ThirdPartyReference { get; set; }
    long BankId { get; set; }
    string Bank { get; set; }
    string BankBranchCode { get; set; }
    string BankAccountNo { get; set; }
    string BankAccountName { get; set; }
    long BankAccountTypeId { get; set; }
    string BankAccountType { get; set; }
    string BankStatementReference { get; set; }
    Debit.ControlType? ControlType { get; set; }
    string ControlTypeDescription { get; set; }
    Debit.ControlStatus? ControlStatus { get; set; }
    string ControlStatusDescription { get; set; }
    Debit.FailureType? FailureType { get; set; }
    string FailureTypeDescription { get; set; }
    Debit.TrackingDay? TrackingDays { get; set; }
    string TrackingDaysDescription { get; set; }
    Debit.AVSCheckType? AvsCheckType { get; set; }
    string AvsCheckTypeDescription { get; set; }
    int Repetitions { get; set; }
    int CurrentRepetition { get; set; }
    decimal Instalment { get; set; }
    DateTime LastInstalmentUpdate { get; set; }
    Account.PeriodFrequency? Frequency { get; set; }
    string FrequencyDescription { get; set; }
    Account.PayRule? PayRule { get; set; }
    string PayRuleDescription { get; set; }
    Account.PayDateType? PayDateType { get; set; }// Either PayDateDayOfWeek or DateOfMonth MUST be populated. Both cannot be null
    string PayDateTypeDescription { get; set; }// Either PayDateDayOfWeek or DateOfMonth MUST be populated. Both cannot be null
    DayOfWeek? PayDateDayOfWeek { get; set; } // will not be null if paydatetype is DayOfWeek
    int? PayDateDayOfMonth { get; set; } // will not be null if paydatetype is DayOfMonth
    DateTime FirstInstalmentDate { get; set; }
    List<Debit.ValidationType> ValidationErrors { get; set; }
    List<IDebitOrderTransaction> ResponseTransactions { get; set; }
  }
}

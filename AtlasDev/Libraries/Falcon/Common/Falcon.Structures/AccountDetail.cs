using System;
using System.Collections.Generic;
using Atlas.Enumerators;
using Falcon.Common.Structures.Avs;

namespace Falcon.Common.Structures
{
  public sealed class AccountDetail
  {
    public long AccountId { get; set; }
    public long PersonId { get; set; }
    public int HostId { get; set; }
    public string Host { get; set; }
    public string PayRule { get; set; }
    public string PayDate { get; set; }
    public long BranchId { get; set; }
    public string Branch { get; set; }
    public long? AccountTypeId { get; set; }
    public string AccountType { get; set; }
    public string IdNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string AccountNo { get; set; }
    public string Firstname { get; set; }
    public string Middlename { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public string Gender { get; set; }
    public int StatusId { get; set; }
    public string Status { get; set; }
    public float InterestRate { get; set; }
    public int Period { get; set; }
    public int PeriodFrequnencyId { get; set; }
    public string PeriodFrequnency { get; set; }
    public decimal LoanAmount { get; set; }
    public decimal CapitalAmount { get; set; }
    public decimal AccountBalance { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime LastStatusDate { get; set; }
    public decimal PayoutAmount { get; set; }
    public decimal InstalmentAmount { get; set; }
    public DateTime? FirstInstalmentDate { get; set; }
    public long EmployerId { get; set; }
    public int Delinquency { get; set; }
    public double DelinquencyPercentage { get; set; }
    public decimal ArrearsAmount { get; set; }
    public DateTime CacheDate { get; set; }
    public List<AccountAffordabilityOption> AffordabilityOptions { get; set; }
    public List<AccountAffordabilityItem> AffordabilityItems { get; set; }
    public List<AccountAddress> Addresses { get; set; }
    public List<AccountContact> Contacts { get; set; }
    public List<AccountNote> Notes { get; set; }
    public List<AccountPayout> Payouts { get; set; }
    public List<AccountWorklow> Workflow { get; set; }
    public List<AccountBureauEnquiry> CreditEnquiries { get; set; }
    public List<FraudScore> FraudEnquiries { get; set; }
    public List<BureauEnquiries> BureauEnquiries { get; set; }
    public List<BankDetail> BankDetails { get; set; }
    public List<AccountStatement> Statement { get; set; }
    public List<AccountDebitControl> DebitControls { get; set; }
    public List<AccountStatushistory> StatusHistory { get; set; }
    public List<AvsTransactions> AvsTransactions { get; set; }
    public List<Authentication> Authentication { get; set; }
    public List<AccountHistory> AccountHistory { get; set; }
    public List<Relation> Relations { get; set; }
    public List<Employer> Employers { get; set; }
    public List<Quotation> Quotations { get; set; }
  }

  public class BureauEnquiries
  {
    public Int64 EnquiryId { get; set; }
    public Risk.RiskEnquiryType EnquiryType { get; set; }
    public Risk.RiskTransactionType TransactionType { get; set; }
    public string IdentityNum { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsSucess { get; set; }
    public DateTime EnquiryDate { get; set; }
    public DateTime? CreateDate { get; set; }
  }

  public class AccountAffordabilityOption
  {
    public long AffordabilityOptionId { get; set; }
    public decimal Amount { get; set; }
    public decimal TotalFees { get; set; }
    public decimal CapitalAmount { get; set; }
    public decimal? TotalPayBack { get; set; }
    public decimal? Instalment { get; set; }
    public int NumOfInstalments { get; set; }
    public int Period { get; set; }
    public string PeriodFrequency { get; set; }
    public int AffordabilityOptionStatusId { get; set; }
    public string AffordabilityOptionStatus { get; set; }
    public string AffordabilityOptionStatusColor { get; set; }
    public string AffordabilityOptionType { get; set; }
    public float? InterestRate { get; set; }
    public bool CanClientAfford { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? LastStatusDate { get; set; }
  }
  
  public class AccountAffordabilityItem
  {
    public long AffordabilityId { get; set; }
    public string Category { get; set; }
    public string Type { get; set; } // Income/Expense
    public string TypeColor { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreateDate { get; set; }
    public string CreateUser { get; set; }
    public DateTime? DeleteDate { get; set; }
    public string DeleteUser { get; set; }
  }

  public class AccountPayout
  {
    public long PayoutId { get; set; }
    public string PayoutStatus { get; set; }
    public int PayoutStatusId { get; set; }
    public string PayoutStatusColor { get;set;}
    public decimal Amount { get; set; }
    public DateTime ActionDate { get; set; }
    public long BatchId { get; set; }
    public string BatchStatus { get; set; }
    public int BatchStatusId { get; set; }
    public DateTime LastBatchStatusDate { get; set; }
    public DateTime? BatchAuthoriseDate { get; set; }
    public DateTime? BatchSubmitDate { get; set; }
    public DateTime BatchCreateDate { get; set; }
    public string Bank { get; set; }
    public string BankAccountNo { get; set; }
    public string BankAccountName { get; set; }
    public bool IsValid { get; set; }
    public int ResultCodeId { get; set; }
    public string ResultCode { get; set; }
    public string ResultCodeDescription { get; set; }
    public DateTime? ResultDate { get; set; }
    public int ResultQualifierId { get; set; }
    public string ResultQualifier { get; set; }
    public string ResultMessage { get; set; }
    public bool? Paid { get; set; }
    public DateTime? PaidDate { get; set; }
    public DateTime CreateDate { get; set; }
  }

  public class AccountAddress
  {
    public long AddressId { get; set; }
    public long AddressTypeId { get; set; }
    public string AddressType { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string AddressLine3 { get; set; }
    public string AddressLine4 { get; set; }
    public string Province { get; set; }
    public long ProvinceId { get; set; }
    public string PostalCode { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreateDate { get; set; }
  }

  public class AccountContact
  {
    public long ContactId { get; set; }
    public long ContactTypeId { get; set; }
    public string ContactType { get; set; }
    public string Value { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreateDate { get; set; }
  }

  public class AccountNote
  {
    public long NoteId { get; set; }
    public long? ParentNoteId { get; set; }
    public string Note { get; set; }
    public string CreateUser { get; set; }
    public long CreateUserId { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? LastEditDate { get; set; }
    public string DeleteUser { get; set; }
    public DateTime? DeleteDate { get; set; }
  }

  public class AccountWorklow
  {
    public long ProcessStepJobAccountId { get; set; }
    public long ProcessJobId { get; set; }
    public long ProcessId { get; set; }
    public string Process { get; set; }
    public long ProcessStepJobId { get; set; }
    public long ProcessStepId { get; set; }
    public string ProcessStep { get; set; }
    public int ProcessJobStateId { get; set; }
    public string ProcessJobState { get; set; }
    public int ProcessStepJobStateId { get; set; }
    public string ProcessStepJobState { get; set; }
    public DateTime ProcessLastStateDate { get; set; }
    public DateTime? ProcessCompleteDate { get; set; }
    public DateTime? ProcessStepLastStateDate { get; set; }
    public DateTime? ProcessStepCompleteDate { get; set; }
  }

  public class AccountBureauEnquiry
  {
    public long EnquiryId { get; set; }
    public long? PreviousEnquiryId { get; set; }
    public long ServiceId { get; set; }
    public string Service { get; set; }
    public int EnquiryTypeId { get; set; }
    public string EnquiryType { get; set; }
    public int TransactionTypeId { get; set; }
    public string TransactionType { get; set; }
    public string IdentityNum { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsSucess { get; set; }
    public string Score { get; set; }
    public string Colour { get; set; }
    public DateTime EnquiryDate { get; set; }
    public DateTime? CreateDate { get; set; }
    public List<AccountBureauAccount> BureauAccounts { get; set; }
    public List<FraudScore> FraudScore { get; set; }
  }

  public class FraudScore
  {
    public long FraudScoreId { get; set; }
    public string Rating { get; set; }
    public string RatingDescription { get; set; }
    public string IDNumber { get; set; }
    public string BankAccountNo { get; set; }
    public string CellNo { get; set; }
    public bool Passed { get; set; }
    public DateTime? OverrideDate { get; set; }
    public string OverrideUser { get; set; }
    public DateTime? CreatedDate { get; set; }

    // Sectionals //
    public List<FraudScoreReason> Reasons { get; set; }

  }

  public class FraudScoreReason
  {
    public long ReasonId { get; set; }
    public string Description { get; set; }
    public string ReasonCode { get; set; }
  }

  public class AccountBureauAccount
  {
    public Int64 BureauAccountId { get; set; }
    public int BureauAccountTypeId { get; set; }
    public string BureauAccountType { get; set; }
    public int AccountSourceId { get; set; }
    public string AccountSource { get; set; }
    public long AccountTypeId { get; set; }
    public string AccountType { get; set; }
    public string Subscriber { get; set; }
    public string AccountNo { get; set; }
    public string SubAccountNo { get; set; }
    public long AccountStatusCodeId { get; set; }
    public string AccountStatusCode { get; set; }
    public string AccountStatusCodeSortCode { get; set; }
    public string Status { get; set; }
    public int? JointParticipants { get; set; }
    public string Period { get; set; }
    public string PeriodType { get; set; }
    public bool Enabled { get; set; }
    public DateTime? OpenDate { get; set; }
    public Decimal? Instalment { get; set; }
    public Decimal? OpenBalance { get; set; }
    public Decimal? CurrentBalance { get; set; }
    public Decimal? OverdueAmount { get; set; }
    public DateTime? BalanceDate { get; set; }
    public DateTime? LastPayDate { get; set; }
    public DateTime? StatusDate { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string CreateUser { get; set; }
    public DateTime? OverrideDate { get; set; }
    public string OverrideUser { get; set; }
  }

  public class AccountStatement
  {
    public long TransactionId { get; set; }
    public string TransactionType { get; set; }
    public int TransactionTypeId { get; set; }
    public decimal Amount { get; set; }
    public decimal RunningBalance { get; set; }
    public DateTime TransactionDate { get; set; }
    public DateTime CreateDate { get; set; }
  }

  public class AccountDebitControl
  {
    public long ControlId { get; set; }
    public int ServiceId { get; set; }
    public string Service { get; set; }
    public string ThirdPartyReference { get; set; }
    public string BankStatementReference { get; set; }
    public string IdNumber { get; set; }
    public string BankBranchCode { get; set; }
    public string BankAccountNo { get; set; }
    public long BankAccountTypeId { get; set; }
    public string BankAccountType { get; set; }
    public string BankAccountName { get; set; }
    public Bank Bank { get; set; }
    public int ControlTypeId { get; set; }
    public string ControlType { get; set; }
    public int ControlStatusId { get; set; }
    public string ControlStatus { get; set; }
    public string ControlStatusColor { get; set; }
    public int FailureTypeId { get; set; }
    public string FailureType { get; set; }
    public DateTime? LastStatusDate { get; set; }
    public int TrackingDays { get; set; }
    public int CurrentRepetition { get; set; }
    public int Repetitions { get; set; }
    public bool IsValid { get; set; }
    public DateTime? CreateDate { get; set; }
    public int PeriodFrequencyId { get; set; }
    public string PeriodFrequency { get; set; }
    public int PayRuleId { get; set; }
    public string PayRule { get; set; }
    public int PayDateId { get; set; }
    public int PayDate { get; set; }
    public int AVSCheckTypeId { get; set; }
    public string AVSCheckType { get; set; }
    public long AVSTransactionId { get; set; }
    public decimal Instalment { get; set; }
    public DateTime LastInstalmentUpdate { get; set; }
    public DateTime FirstInstalmentDate { get; set; }
    public List<AccountDebitControlTransaction> Transactions { get; set; }
  }

  public class AccountDebitControlTransaction
  {
    public Int64 TransactionId { get; set; }
    public int DebitTypeId { get; set; }
    public string DebitType { get; set; }
    public long BatchId { get; set; }
    public int BatchStatusId { get; set; }
    public string BatchStatus { get; set; }
    public DateTime? BatchLastStatusDate { get; set; }
    public DateTime? BatchSubmitDate { get; set; }
    public string BatchSubmitUser { get; set; }
    public DateTime BatchCreateDate { get; set; }
    public int StatusId { get; set; }
    public string Status { get; set; }
    public string StatusColor { get; set; }
    public DateTime? LastStatusDate { get; set; }
    public decimal Amount { get; set; }
    public DateTime InstalmentDate { get; set; }
    public DateTime ActionDate { get; set; }
    public int Repetition { get; set; }
    public int ResponseCodeId { get; set; }
    public string ResponseCode { get; set; }
    public string ResponseCodeDescription { get; set; }
    public DateTime? ResponseDate { get; set; }
    public DateTime? CancelDate { get; set; }
    public string CancelUser { get; set; }
    public DateTime? OverrideDate { get; set; }
    public string OverrideUser { get; set; }
    public decimal? OverrideAmount { get; set; }
    public DateTime? OverrideActionDate { get; set; }
    public int? OverrideTrackingDays { get; set; }
    public DateTime? CreateDate { get; set; }
  }

  public class AccountStatushistory
  {
    public int StatusId { get; set; }
    public string Status { get; set; }
    public DateTime CreateDate { get; set; }
  }

  public class Authentication
  {
    public Int64 AuthenticationId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string IdNo { get; set; }
    public string BankAccountNo { get; set; }
    public bool Authenticated { get; set; }
    public bool Completed { get; set; }
    public string QuestionCount { get; set; }
    public decimal? AuthenticatedPercentage { get; set; }
    public decimal? RequiredPercentage { get; set; }
    public string Reference { get; set; }
    public bool Enabled { get; set; }
    public string OverrideUser { get; set; }
    public DateTime? OverrideDate { get; set; }
    public DateTime? CreateDate { get; set; }
  }
}
using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class DBT_ControlDTO
  {
    [DataMember]
    public Int64 ControlId { get; set; }
    [DataMember]
    public DBT_ServiceDTO Service { get; set; }
    [DataMember]
    public HostDTO Host { get; set; }
    [DataMember]
    public CPY_CompanyDTO CompanyBranch { get; set; }
    [DataMember]
    public string ThirdPartyReference { get; set; }
    [DataMember]
    public string BankStatementReference { get; set; }
    [DataMember]
    public string IdNumber { get; set; }
    [DataMember]
    public string BankBranchCode { get; set; }
    [DataMember]
    public string BankAccountNo { get; set; }
    [DataMember]
    public BankAccountTypeDTO BankAccountType { get; set; }
    [DataMember]
    public string BankAccountName { get; set; }
    [DataMember]
    public BankDTO Bank { get; set; }
    [DataMember]
    public DBT_ControlTypeDTO ControlType { get; set; }
    [DataMember]
    public DBT_ControlStatusDTO ControlStatus { get; set; }
    [DataMember]
    public DBT_FailureTypeDTO FailureType { get; set; }
    [DataMember]
    public DateTime? LastStatusDate { get; set; }
    [DataMember]
    public int TrackingDays { get; set; }
    [DataMember]
    public int CurrentRepetition { get; set; }
    [DataMember]
    public int Repetitions { get; set; }
    [DataMember]
    public bool IsValid { get; set; }
    [DataMember]
    public DateTime? CreateDate { get; set; }
    [DataMember]
    public PER_PersonDTO CreateUser { get; set; }
    [DataMember]
    public ACC_PeriodFrequencyDTO PeriodFrequency { get; set; }
    [DataMember]
    public ACC_PayRuleDTO PayRule { get; set; }
    [DataMember]
    public ACC_PayDateDTO PayDate { get; set; }
    [DataMember]
    public DBT_AVSCheckTypeDTO AVSCheckType { get; set; }
    [DataMember]
    public long AVSTransactionId { get; set; }
    [DataMember]
    public decimal Instalment { get; set; }
    [DataMember]
    public DateTime LastInstalmentUpdate { get; set; }
    [DataMember]
    public DateTime FirstInstalmentDate { get; set; }
  }
}
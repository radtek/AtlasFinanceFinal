using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class AVS_TransactionDTO
  {
    [DataMember]
    public Int64 TransactionId { get; set; }
    [DataMember]
    public Int64? TransactionRef { get; set; }
    [DataMember]
    public HostDTO Host { get; set; }
    [DataMember]
    public AVS_ServiceDTO Service { get; set; }
    [DataMember]
    public AVS_StatusDTO Status { get; set; }
    [DataMember]
    public DateTime LastStatusDate { get; set; }
    [DataMember]
    public CPY_CompanyDTO Company { get; set; }
    [DataMember]
    public AVS_BankAccountPeriodDTO BankAccountPeriod { get; set; }
    [DataMember]
    public ACC_AccountDTO Account { get; set; }
    [DataMember]
    public PER_PersonDTO Person { get; set; }
    [DataMember]
    public AVS_BatchDTO Batch { get; set; }
    [DataMember]
    public string Initials { get; set; }
    [DataMember]
    public string LastName { get; set; }
    [DataMember]
    public string IdNumber { get; set; }
    [DataMember]
    public BankDTO Bank { get; set; }
    [DataMember]
    public string BranchCode { get; set; }
    [DataMember]
    public string AccountNo { get; set; }
    [DataMember]
    public string ResponseAccountNumber { get; set; }
    [DataMember]
    public string ResponseIdNumber { get; set; }
    [DataMember]
    public string ResponseInitials { get; set; }
    [DataMember]
    public string ResponseLastName { get; set; }
    [DataMember]
    public string ResponseAccountOpen { get; set; }
    [DataMember]
    public string ResponseAcceptsDebit { get; set; }
    [DataMember]
    public string ResponseAcceptsCredit { get; set; }
    [DataMember]
    public string ResponseOpenThreeMonths { get; set; }
    [DataMember]
    public string ThirdPartyRef { get; set; }
    [DataMember]
    public bool Enabled { get; set; }
    [DataMember]
    public AVS_ResultDTO Result { get; set; }
    [DataMember]
    public PER_PersonDTO CreateUser { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }
    [DataMember]
    public DateTime? ResponseDate { get; set; }
    [DataMember]
    public string ErrorMessage { get; set; }
  }
}
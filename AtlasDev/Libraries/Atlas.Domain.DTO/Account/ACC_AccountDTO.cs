using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class ACC_AccountDTO
  {
    [DataMember]
    public Int64 AccountId { get; set; }
    [DataMember]
    public string AccountNo { get; set; }
    [DataMember]
    public ACC_AccountTypeDTO AccountType { get; set; }
    [DataMember]
    public PER_PersonDTO Person { get; set; }
    [DataMember]
    public HostDTO Host { get; set; }
    [DataMember]
    public ACC_StatusDTO Status { get; set; }
    [DataMember]
    public Decimal LoanAmount { get; set; }
    [DataMember]
    public Decimal TotalTopUpAmount { get; set; }
    [DataMember]
    public Decimal TotalFees { get; set; }
    [DataMember]
    public Decimal CapitalAmount { get; set; }
    [DataMember]
    public Decimal PayoutAmount { get; set; }
    [DataMember]
    public Decimal? ThirdPartyPayoutAmount { get; set; }
    [DataMember]
    public Decimal AccountBalance { get; set; }
    [DataMember]
    public float InterestRate { get; set; }
    [DataMember]
    public int Period { get; set; }
    [DataMember]
    public ACC_PeriodFrequencyDTO PeriodFrequency { get; set; }
    [DataMember]
    public Decimal InstalmentAmount { get; set; }
    [DataMember]
    public int NumOfInstalments { get; set; }
    [DataMember]
    public ACC_StatusReasonDTO StatusReason { get; set; }
    [DataMember]
    public ACC_StatusSubReasonDTO StatusSubReason { get; set; }
    [DataMember]
    public string NLREnquiryReferenceNo { get; set; }
    [DataMember]
    public string NLRRegistrationNo { get; set; }
    [DataMember]
    public bool IsNLRRegistered { get; set; }
    [DataMember]
    public DateTime StatusChangeDate { get; set; }
    [DataMember]
    public DateTime? FirstInstalmentDate { get; set; }
    [DataMember]
    public DateTime? OpenDate { get; set; }
    [DataMember]
    public DateTime? CloseDate { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }
    [DataMember]
    public PER_PersonDTO CreatedBy { get; set; }

  }
}

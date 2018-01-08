using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class PYT_PayoutDTO
  {
    [DataMember]
    public long PayoutId { get; set; }
    [DataMember]
    public PYT_ServiceDTO Service { get; set; }
    [DataMember]
    public ACC_AccountDTO Account { get; set; }
    [DataMember]
    public PER_PersonDTO Person { get; set; }
    [DataMember]
    public CPY_CompanyDTO Company { get; set; }
    [DataMember]
    public decimal Amount { get; set; }
    [DataMember]
    public DateTime ActionDate { get; set; }
    [DataMember]
    public PYT_BatchDTO Batch { get; set; }
    [DataMember]

    public PYT_PayoutStatusDTO PayoutStatus { get; set; }
    [DataMember]
    public BankDetailDTO BankDetail { get; set; }
    [DataMember]
    public DateTime LastStatusDate { get; set; }

    public PER_PersonDTO StatusUser { get; set; }
    [DataMember]
    public bool IsValid { get; set; }
    [DataMember]
    public PYT_ResultCodeDTO ResultCode { get; set; }
    [DataMember]
    public DateTime? ResultDate { get; set; }
    [DataMember]
    public PYT_ResultQualifierDTO ResultQualifier { get; set; }
    [DataMember]
    public string ResultMessage { get; set; }
    [DataMember]
    public bool? Paid { get; set; }
    [DataMember]
    public DateTime? PaidDate { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }

  }
}
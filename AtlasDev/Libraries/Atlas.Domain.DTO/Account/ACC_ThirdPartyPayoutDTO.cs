using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class ACC_ThirdPartyPayoutDTO
  {
    [DataMember]
    public Int64 ThirdPartyPayoutId { get; set; }
    [DataMember]
    public ACC_AccountDTO Account { get; set; }
    [DataMember]
    public CPY_CompanyDTO Company { get; set; }
    [DataMember]
    public BankDetailDTO BankDetail { get; set; }
    [DataMember]
    public string ReferenceNo { get; set; }
    [DataMember]
    public decimal Amount { get; set; }
    [DataMember]
    public bool Enabled { get; set; }
    [DataMember]
    public DateTime? PaidDate { get; set; }
    [DataMember]
    public DateTime? UpdateDate { get; set; }
    [DataMember]
    public PER_PersonDTO UpdatedBy { get; set; }
    [DataMember]
    public DateTime CreatedDate { get; set; }
    [DataMember]
    public PER_PersonDTO CreatedBy { get; set; }
  }
}
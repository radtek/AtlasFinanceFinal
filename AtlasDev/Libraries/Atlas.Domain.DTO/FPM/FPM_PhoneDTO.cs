using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  using System;

    [Serializable]
  [DataContract(IsReference = true)]
  public sealed class FPM_PhoneDTO
  {
    [DataMember]
    public Int64 PhoneId { get;set;}
    [DataMember]
    public FPM_FraudScoreDTO FraudScore { get;set;}
    [DataMember]
    public string PhoneNo { get;set;}
    [DataMember]
    public string PhoneTypeId { get;set;}
    [DataMember]
    public string OtherDescription { get;set;}
    [DataMember]
    public DateTime? InformationDate { get; set; }
  }
}

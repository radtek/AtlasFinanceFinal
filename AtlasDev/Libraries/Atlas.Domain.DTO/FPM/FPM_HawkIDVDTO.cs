using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  using System;

    [Serializable]
  [DataContract(IsReference = true)]
  public sealed class FPM_HawkIDVDTO
  {
    [DataMember]
    public Int64 HawkIdvId { get;set;}
    [DataMember]
    public FPM_FraudScoreDTO FraudScore { get;set;}
    [DataMember]
    public string IDVerifiedCode { get;set;}
    [DataMember]
    public string IDVerifiedDescription { get;set;}
    [DataMember]
    public string VerifiedSurname { get;set;}
    [DataMember]
    public string VerifiedForeName1 { get;set;}
    [DataMember]
    public string VerifiedForeName2 { get;set;}
    [DataMember]
    public DateTime? DeceasedDate { get; set; }
  }
}


using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  using System;

    [Serializable]
  [DataContract(IsReference = true)]
  public sealed class FPM_FraudScore_ReasonDTO
  {
    [DataMember]
    public Int64 FraudScoreReasonId { get;set;}
    [DataMember]
    public FPM_FraudScoreDTO FraudScore { get;set;}
    [DataMember]
    public string Description { get;set;}
    [DataMember]
    public string ReasonCode { get; set; }
  }
}

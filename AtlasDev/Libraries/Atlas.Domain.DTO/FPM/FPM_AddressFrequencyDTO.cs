using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  using System;

    [Serializable]
  [DataContract(IsReference = true)]
  public sealed class FPM_AddressFrequencyDTO
  {
    [DataMember]
    public Int64 AddressFrequencyId { get;set;}
    [DataMember]
    public FPM_FraudScoreDTO FraudScore { get;set;}
    [DataMember]
    public int Last24Hours { get;set;}
    [DataMember]
    public int Last48Hours { get;set;}
    [DataMember]
    public int Last96Hours { get;set;}
    [DataMember]
    public int Last30Days { get;set;}
    [DataMember]
    public string AddressMessage { get; set; }
  }
}

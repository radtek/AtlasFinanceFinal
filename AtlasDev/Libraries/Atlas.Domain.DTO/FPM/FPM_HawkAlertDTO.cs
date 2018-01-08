using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  using System;

    [Serializable]
  [DataContract(IsReference = true)]
  public sealed class FPM_HawkAlertDTO
  {
    [DataMember]
    public Int64 HawkAlertId { get;set;}
    [DataMember]
    public FPM_FraudScoreDTO FraudScore { get;set;}
    [DataMember]
    public string HawkNo { get;set;}
    [DataMember]
    public string HawkCode { get;set;}
    [DataMember]
    public string HawkDescription { get;set;}
    [DataMember]
    public string HawkFoundFor { get;set;}
    [DataMember]
    public DateTime? DeceasedDate { get;set;}
    [DataMember]
    public string SubscriberName { get;set;}
    [DataMember]

    public string SubscriberReference { get;set;}
    [DataMember]

    public string ContactName { get;set;}
    [DataMember]

    public string ContactTelCode { get;set;}
    [DataMember]

    public string ContactTelNo { get;set;}
    [DataMember]

    public string VictimReference { get;set;}
    [DataMember]

    public string VictimTelCode { get;set;}
    [DataMember]

    public string VictimTelNo { get; set; }
  }
}

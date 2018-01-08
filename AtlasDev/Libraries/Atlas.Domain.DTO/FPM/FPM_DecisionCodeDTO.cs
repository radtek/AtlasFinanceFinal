using System.Runtime.Serialization;
using Atlas.Enumerators;

namespace Atlas.Domain.DTO
{
  using System;

    [Serializable]
  [DataContract]
  public sealed class FPM_DecisionCodeDTO
  {
    [DataMember]
    public int DecisionCodeId { get;set;}
    [DataMember]
    public string ReasonCode { get;set;}
    [DataMember]
    public FPM.DecisionOutCome DecisionOutCome { get; set; }
  }
}

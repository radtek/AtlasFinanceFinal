using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference=true)]
  public class BUR_BandRangeDTO
  {
    [DataMember]
    public int BandRangeId { get; set; }
    [DataMember]
    public BUR_BandDTO Band { get; set; }
    [DataMember]
    public int Start { get; set; }
    [DataMember]
    public int End { get; set; }
  }
}

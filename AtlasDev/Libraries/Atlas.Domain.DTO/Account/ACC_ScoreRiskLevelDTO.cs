using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class ACC_ScoreRiskLevelDTO
  {
    [DataMember]
    public Int64 ScoreRiskLevelId { get; set; }
    [DataMember]
    public BUR_ServiceDTO Service { get; set; }
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public DateTime? ExpiryDate { get; set; }
  }
}
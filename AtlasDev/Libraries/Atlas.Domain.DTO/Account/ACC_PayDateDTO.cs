using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class ACC_PayDateDTO
  {
    [DataMember]
    public int PayDateId { get; set; }
    [DataMember]
    public ACC_PayDateTypeDTO PayDateType { get; set; }
    [DataMember]
    public int DayNo { get; set; }
  }
}
using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class PYT_ServiceScheduleDTO
  {
    [DataMember]
    public int ServiceScheduleId{get;set;}
    [DataMember]
    public PYT_ServiceDTO Service{get;set;}
    [DataMember]
    public DateTime? OpenTime{get;set;}
    [DataMember]
    public DateTime? CloseTime{get;set;}
    [DataMember]
    public bool UseSaturday{get;set;}
    [DataMember]
    public bool UseSunday { get; set; }
  }
}

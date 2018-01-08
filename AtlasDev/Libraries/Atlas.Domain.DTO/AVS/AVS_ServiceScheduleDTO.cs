using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class AVS_ServiceScheduleDTO
  {
    [DataMember]
    public Int32 ServiceScheduleId { get; set; }
    [DataMember]
    public HostDTO Host { get; set; }
    [DataMember]
    public AVS_ServiceDTO Service { get; set; }
    [DataMember]
    public AVS_ServiceTypeDTO ServiceType { get; set; }
    [DataMember]
    public DateTime? OpenTime { get; set; }
    [DataMember]
    public DateTime? CloseTime { get; set; }
    [DataMember]
    public bool UseSaturday { get; set; }
    [DataMember]
    public bool UseSunday { get; set; }
  }
}
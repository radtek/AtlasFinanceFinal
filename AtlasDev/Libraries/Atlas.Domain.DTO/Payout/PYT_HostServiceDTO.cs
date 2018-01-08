using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class PYT_HostServiceDTO
  {
    [DataMember]
    public int HostServiceId { get; set; }
    [DataMember]
    public HostDTO Host { get; set; }
    [DataMember]

    public PYT_ServiceDTO Service { get; set; }
    [DataMember]
    public bool Enabled { get; set; }
    [DataMember]
    public DateTime? DisabledDate { get; set; }
  }
}

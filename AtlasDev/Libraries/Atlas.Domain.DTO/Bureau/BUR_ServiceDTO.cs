using System.Runtime.Serialization;
using System;
using Atlas.Enumerators;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class BUR_ServiceDTO
  {
    [DataMember]
    public Int64 ServiceId { get; set; }
    [DataMember]
    public Risk.ServiceType ServiceType { get; set; }
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public string Username { get; set; }
    [DataMember]
    public string Password { get; set; }
    [DataMember]
    public bool Enabled { get; set; }
  }
}

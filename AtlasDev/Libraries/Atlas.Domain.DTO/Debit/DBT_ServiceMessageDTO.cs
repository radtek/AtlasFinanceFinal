using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class DBT_ServiceMessageDTO
  {
    [DataMember]
    public int ServiceMessageId { get; set; }
    [DataMember]
    public string Code { get; set; }
    [DataMember]
    public string Description { get; set; }
  }
}

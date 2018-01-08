using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class ACC_TopUpStatusDTO
  {
    [DataMember]
    public Int64 TopUpStatusId { get; set; }
    [DataMember]
    public string Description { get; set; }
  }
}

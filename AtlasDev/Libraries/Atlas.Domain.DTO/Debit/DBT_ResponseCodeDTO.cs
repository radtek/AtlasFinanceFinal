using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class DBT_ResponseCodeDTO
  {
    [DataMember]
    public int ResponseCodeId { get; set; }
    [DataMember]
    public string Code { get; set; }
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public bool? IsFailed { get; set; }
  }
}
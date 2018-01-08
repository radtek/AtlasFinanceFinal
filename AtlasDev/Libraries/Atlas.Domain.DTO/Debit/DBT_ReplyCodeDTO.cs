using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class DBT_ReplyCodeDTO
  {
    [DataMember]
    public int ReplyCodeId { get; set; }
    [DataMember]
    public string Description { get; set; }
  }
}
using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class BUR_AccountTypeCodeDTO
  {
    [DataMember]
    public Int64 TypeCodeId { get; set; }
    [DataMember]
    public string ShortCode { get; set; }
    [DataMember]
    public string Description { get; set; }
  }
}
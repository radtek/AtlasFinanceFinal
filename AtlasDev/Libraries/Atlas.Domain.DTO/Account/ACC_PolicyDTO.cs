using System.Runtime.Serialization;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public sealed class ACC_PolicyDTO
  {
    [DataMember]
    public Int64 PolicyId { get; set; }
    [DataMember]
    public Enumerators.Account.Policy Type { get; set; }
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public bool Enabled { get; set; }
  }
}

using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class ACC_AffordabilityCategoryDTO
  {
    [DataMember]
    public Int64 AffordabilityCategoryId { get; set; }
    [DataMember]

    public HostDTO Host { get; set; }
    [DataMember]

    public string Description { get; set; }
    [DataMember]

    public Enumerators.Account.AffordabilityCategoryType AffordabilityCategoryType { get; set; }
    [DataMember]

    public int Ordinal { get; set; }
    [DataMember]

    public bool Enabled { get; set; }
  }
}

using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class ACC_AffordabilityCategoryTypeDTO
  {
    [DataMember]
    public Int64 AffordabilityCategoryTypeId { get; set; }
    [DataMember]
    public string Description { get; set; }
  }
}

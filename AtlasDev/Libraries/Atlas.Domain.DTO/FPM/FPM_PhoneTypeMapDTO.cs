using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  using System;

    [Serializable]
  [DataContract(IsReference = true)]
  public sealed class FPM_PhoneTypeMapDTO
  {
    [DataMember]
    public Int64 PhoneTypeMapId { get; set; }
    [DataMember]
    public ContactTypeDTO PhoneType { get; set; }
    [DataMember]
    public string Description { get; set; }
  }
}

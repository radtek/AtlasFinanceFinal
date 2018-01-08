using System;
using System.Runtime.Serialization;
using Atlas.Common.Extensions;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class AddressTypeDTO
  {
    [DataMember]
    public Int64 AddressTypeId { get; set; }
    [DataMember]
    public Enumerators.General.AddressType Type 
      {
        get { return Description.FromStringToEnum<Enumerators.General.AddressType>(); }
        set { value = Description.FromStringToEnum<Enumerators.General.AddressType>(); }
    }
    [DataMember]
    public string Description { get; set; }
  }
}

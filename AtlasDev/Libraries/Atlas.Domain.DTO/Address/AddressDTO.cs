using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class AddressDTO
  {
    [DataMember]
    public Int64 AddressId { get; set; }
    [DataMember]
    public AddressTypeDTO AddressType { get; set; }
    [DataMember]
    public string Line1 { get; set; }
    [DataMember]
    public string Line2 { get; set; }
    [DataMember]
    public string Line3 { get; set; }
    [DataMember]
    public string Line4 { get; set; }
    [DataMember]
    public ProvinceDTO Province { get; set; }
    [DataMember]
    public string PostalCode { get; set; }
    [DataMember]
    public bool IsActive { get; set; }
    [DataMember]
    public PER_PersonDTO CreatedBy { get; set; }
    [DataMember]
    public DateTime CreatedDT { get; set; }
  }
}

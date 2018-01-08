using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class ContactDTO
  {
    [DataMember]
    public Int64 ContactId { get; set; }
    [DataMember]
    public ContactTypeDTO ContactType { get; set; }
    [DataMember]
    public string Value { get; set; }
    [DataMember]
    public bool IsActive { get; set; }
    [DataMember]
    public PER_PersonDTO CreatedBy { get; set; }
    [DataMember]
    public DateTime CreatedDT { get; set; }
  }
}

using System;
using System.Runtime.Serialization;
using Atlas.Common.Extensions;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class ContactTypeDTO
  {
    [DataMember]
    public Int64 ContactTypeId { get; set; }
    [DataMember]
    public Enumerators.General.ContactType Type
    {
      get { return Description.FromStringToEnum<Enumerators.General.ContactType>(); }
      set { value = Description.FromStringToEnum<Enumerators.General.ContactType>(); }
    }
    [DataMember]
    public string Description { get; set; }
  }
}

using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class ACC_AffordabilityOptionTypeDTO
  {
    [DataMember]
    public Int32 AffordabilityOptionTypeId { get; set; }
    [DataMember]
    public Enumerators.Account.AffordabilityOptionType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Account.AffordabilityOptionType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Account.AffordabilityOptionType>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}

using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class ACC_AffordabilityOptionStatusDTO
  {
    [DataMember]
    public Int32 AffordabilityOptionStatusId { get; set; }
    [DataMember]
    public Enumerators.Account.AffordabilityOptionStatus Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Account.AffordabilityOptionStatus>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Account.AffordabilityOptionStatus>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}

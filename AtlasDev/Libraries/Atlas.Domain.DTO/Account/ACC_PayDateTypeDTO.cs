using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class ACC_PayDateTypeDTO
  {
    [DataMember]
    public int PayDateTypeId { get; set; }
    [DataMember]
    public Enumerators.Account.PayDateType Type
    {
      get { return Description.FromStringToEnum<Enumerators.Account.PayDateType>(); }
      set { value = Description.FromStringToEnum<Enumerators.Account.PayDateType>(); }
    }
    [DataMember]
    public string Description { get; set; }
  }
}

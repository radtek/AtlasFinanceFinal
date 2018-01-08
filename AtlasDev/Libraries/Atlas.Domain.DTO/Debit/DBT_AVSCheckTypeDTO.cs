using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class DBT_AVSCheckTypeDTO
  {
    [DataMember]
    public int AVSCheckTypeId { get; set; }
    [DataMember]
    public Enumerators.Debit.AVSCheckType Type
    {
      get { return Description.FromStringToEnum<Enumerators.Debit.AVSCheckType>(); }
      set { value = Description.FromStringToEnum<Enumerators.Debit.AVSCheckType>(); }
    }
    [DataMember]
    public string Description { get; set; }
  }
}
using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class DBT_ControlTypeDTO
  {
    [DataMember]
    public int ControlTypeId{get;set;}
    [DataMember]
    public Enumerators.Debit.ControlType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Debit.ControlType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Debit.ControlType>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}

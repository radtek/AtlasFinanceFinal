using System;
using System.Runtime.Serialization;
using Atlas.Common.Extensions;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class DBT_DebitTypeDTO
  {
    [DataMember]
    public int DebitTypeId { get; set; }
    [DataMember]
    public Enumerators.Debit.DebitType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Debit.DebitType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Debit.DebitType>();
      }
    }

    [DataMember]
    public string Description { get; set; }
  }
}
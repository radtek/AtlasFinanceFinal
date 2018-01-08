using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class DBT_FailureTypeDTO
  {
    [DataMember]
    public int FailureTypeId { get; set; }
    [DataMember]
    public Enumerators.Debit.FailureType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Debit.FailureType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Debit.FailureType>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}

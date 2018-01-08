using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class DBT_ValidationDTO
  {
    [DataMember]
    public int ValidationId { get; set; }
    [DataMember]
    public Enumerators.Debit.ValidationType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Debit.ValidationType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Debit.ValidationType>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}
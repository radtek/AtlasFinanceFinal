using System;
using System.Runtime.Serialization;
using Atlas.Common.Extensions;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class DBT_StatusDTO
  {
    [DataMember]
    public int StatusId { get; set; }
    [DataMember]
    public Enumerators.Debit.Status Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Debit.Status>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Debit.Status>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}
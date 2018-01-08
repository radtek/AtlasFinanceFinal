using System.Runtime.Serialization;
using System;
using Atlas.Common.Extensions;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class DBT_BatchStatusDTO
  {
    [DataMember]

    public int BatchStatusId { get;set;}
    [DataMember]
    public Enumerators.Debit.BatchStatus Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Debit.BatchStatus>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Debit.BatchStatus>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}
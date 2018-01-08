using System.Runtime.Serialization;
using System;
using Atlas.Common.Extensions;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class DBT_ControlStatusDTO 
  {
    [DataMember]
    public int ControlStatusId { get;set;}
    [DataMember]
    public Enumerators.Debit.ControlStatus Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Debit.ControlStatus>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Debit.ControlStatus>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}
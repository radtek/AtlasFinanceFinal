using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class ACC_StatusDTO
  {
    [DataMember]
    public Int32 StatusId { get; set; }
    [DataMember]
    public Enumerators.Account.AccountStatus Type
    {
      get { return Description.FromStringToEnum<Enumerators.Account.AccountStatus>(); }
      set { value = Description.FromStringToEnum<Enumerators.Account.AccountStatus>(); }
    }
    [DataMember]
    public string Description { get; set; }
  }
}
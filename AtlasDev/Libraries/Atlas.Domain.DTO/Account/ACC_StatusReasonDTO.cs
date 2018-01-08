using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class ACC_StatusReasonDTO
  {
    [DataMember]
    public Int64 StatusReasonId { get; set; }
    [DataMember]
    public Enumerators.Account.AccountStatusReason Type
    {
      get { return Description.FromStringToEnum<Enumerators.Account.AccountStatusReason>(); }
      set { value = Description.FromStringToEnum<Enumerators.Account.AccountStatusReason>(); }
    }
    [DataMember]
    public ACC_StatusDTO Status { get; set; }
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public bool Enabled { get; set; }
    [DataMember]
    public bool MultipleSubReasons { get; set; }
  }
}

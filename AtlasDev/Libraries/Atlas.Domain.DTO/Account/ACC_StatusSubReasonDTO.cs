using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class ACC_StatusSubReasonDTO
  {
    [DataMember]
    public Int64 StatusSubReasonId { get; set; }
    [DataMember]
    public Enumerators.Account.AccountStatusSubReason Type
    {
      get { return Description.FromStringToEnum<Enumerators.Account.AccountStatusSubReason>(); }
      set { value = Description.FromStringToEnum<Enumerators.Account.AccountStatusSubReason>(); }
    }
    [DataMember]
    public ACC_StatusReasonDTO StatusReason { get; set; }
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public bool Enabled { get; set; }
  }
}

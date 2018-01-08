using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class ACC_SettlementStatusDTO
  {
    [DataMember]
    public Int32 SettlementStatusId { get; set; }
    [DataMember]
    public Enumerators.Account.SettlementStatus Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Account.SettlementStatus>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Account.SettlementStatus>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}
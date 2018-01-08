using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class ACC_SettlementTypeDTO
  {
    [DataMember]
    public long SettlementTypeId { get; set; }
    [DataMember]
    public Enumerators.Account.SettlementType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Account.SettlementType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Account.SettlementType>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}
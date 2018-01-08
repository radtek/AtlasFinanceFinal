using System;
using System.Runtime.Serialization;
using Atlas.Common.Extensions;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class ACC_PayRuleDTO
  {
    [DataMember]
    public int PayRuleId { get; set; }
    [DataMember]
    public Enumerators.Account.PayRule Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Account.PayRule>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Account.PayRule>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}

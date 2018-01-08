using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class PYT_ReplyCodeTypeDTO
  {
    [DataMember]
    public int ReplyCodeTypeId { get; set; }
    [DataMember]
    public Enumerators.Payout.PayoutReplyCodeType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Payout.PayoutReplyCodeType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Payout.PayoutReplyCodeType>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}
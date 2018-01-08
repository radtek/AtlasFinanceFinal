using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class PYT_ReplyCodeDTO
  {
    [DataMember]
    public int ReplyCodeId { get; set; }
    [DataMember]
    public PYT_ServiceTypeDTO ServiceType { get; set; }
    [DataMember]
    public Enumerators.Payout.ReplyCode Type
    {
      get
      {
        return Code.FromStringToEnum<Enumerators.Payout.ReplyCode>();
      }
      set
      {
        value = Code.FromStringToEnum<Enumerators.Payout.ReplyCode>();
      }
    }
    [DataMember]
    public string Code { get; set; }
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public PYT_ReplyCodeTypeDTO ReplyCodeType { get; set; }
  }
}

using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class PYT_ResultCodeDTO
  {
    [DataMember]
    public int ResultCodeId { get; set; }

    [DataMember]
    public Enumerators.Payout.ResultCode Type
    {
      get
      {
        return ResultCode.FromStringToEnum<Enumerators.Payout.ResultCode>();
      }
      set
      {
        value = ResultCode.FromStringToEnum<Enumerators.Payout.ResultCode>();
      }
    }
    [DataMember]
    public string ResultCode { get; set; }
    [DataMember]
    public string Description { get; set; }
  }
}

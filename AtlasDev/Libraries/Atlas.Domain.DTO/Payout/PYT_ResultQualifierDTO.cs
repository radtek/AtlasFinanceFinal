using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class PYT_ResultQualifierDTO
  {
    [DataMember]
    public int ResultQualifierId { get; set; }
    [DataMember]
    public Enumerators.Payout.ResultQualifierCode Type
    {
      get
      {
        return ResultQualifierCode.FromStringToEnum<Enumerators.Payout.ResultQualifierCode>();
      }
      set
      {
        value = ResultQualifierCode.FromStringToEnum<Enumerators.Payout.ResultQualifierCode>();
      }
    }
    [DataMember]
    public string ResultQualifierCode { get; set; }
    [DataMember]
    public string Description { get; set; }
  }
}

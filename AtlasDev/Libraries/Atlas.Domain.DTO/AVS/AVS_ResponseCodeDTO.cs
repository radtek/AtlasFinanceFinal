using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference=true)]
  public class AVS_ResponseCodeDTO
  {
    [DataMember]
    public Int32 ResponseCodeId { get; set; }
    [DataMember]
    public Enumerators.AVS.ResponseCode Type { get; set; }
    [DataMember]
    public string ResponseCode { get; set; }
    [DataMember]
    public AVS_ResponseGroupDTO ResponseGroup { get; set; }
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public AVS_ResponseResultDTO ResponseResult { get; set; }
  }
}

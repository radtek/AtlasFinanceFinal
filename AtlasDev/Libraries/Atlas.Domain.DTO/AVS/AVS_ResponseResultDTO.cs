using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class AVS_ResponseResultDTO
  {
    [DataMember]
    public Int32 ResponseResultId { get; set; }
    [DataMember]
    public Enumerators.AVS.ResponseResult Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.AVS.ResponseResult>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.AVS.ResponseResult>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}

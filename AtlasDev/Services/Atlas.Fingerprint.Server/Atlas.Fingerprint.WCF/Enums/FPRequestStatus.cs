using System;
using System.Runtime.Serialization;

  
namespace Atlas.WCF.FPServer.Interface
{
  [DataContract]
  public enum FPRequestStatus
  {
    [EnumMember]
    NotSet = 0,

    [EnumMember]
    Successful = 1,

    [EnumMember]
    Failed = 2,

    [EnumMember]
    Cancelled = 3
  };
}

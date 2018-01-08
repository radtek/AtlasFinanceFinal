using System;
using System.Runtime.Serialization;
using System.Collections.Generic;


namespace Atlas.WCF.FPServer.Interface
{
  [DataContract]
  public enum FPRequestType
  {
    [EnumMember]
    NotSet = 0,

    [EnumMember]
    Verify = 1,

    [EnumMember]
    Enroll = 2,

    [EnumMember]
    Identification = 3
  };
}

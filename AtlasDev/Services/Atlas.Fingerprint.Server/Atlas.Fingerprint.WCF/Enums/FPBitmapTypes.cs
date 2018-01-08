using System;
using System.Runtime.Serialization;


namespace Atlas.WCF.FPServer.Interface
{
  [DataContract]
  public enum FPBitmapTypes
  {
    [EnumMember]
    NotSet = 0,

    [EnumMember]
    WSQ = 1,

    [EnumMember]
    RawByte = 2
  }
}

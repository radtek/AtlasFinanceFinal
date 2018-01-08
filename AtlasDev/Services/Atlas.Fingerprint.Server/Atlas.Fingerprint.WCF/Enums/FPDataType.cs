using System;
using System.Runtime.Serialization;


namespace Atlas.WCF.FPServer.Interface
{
  [DataContract]
  public enum FPDataType
  {
    [EnumMember]
    NotSet = 0,

    [EnumMember]
    Template = 1,

    [EnumMember]
    Bitmap = 2
  }
}

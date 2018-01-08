using System;
using System.Runtime.Serialization;


namespace Atlas.WCF.FPServer.Interface
{
  [DataContract]
  public enum BitmapColoursType
  {
    [EnumMember]
    NotSet = 0,

    [EnumMember]
    BlackFingeprintOnWhiteBackground = 1,

    [EnumMember]
    WhiteFingerprintOnBlackBackground = 2
  }
}

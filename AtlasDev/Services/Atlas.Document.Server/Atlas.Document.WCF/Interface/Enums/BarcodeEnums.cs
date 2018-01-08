using System;
using System.Runtime.Serialization;


namespace Atlas.DocServer.WCF.Interface
{
  public class BarcodeEnums
  {
    [DataContract(Name = "BarcodeTypes", Namespace = "urn:Atlas/ASS/DocServer/Enums/2014/05")]
    public enum BarcodeTypes
    {
      [EnumMember]
      NotSet,

      [EnumMember]
      CodaBar,

      [EnumMember]
      Code128,

      [EnumMember]
      DataMatrix,

      [EnumMember]
      Code3Of9,

      [EnumMember]
      Code11,

      [EnumMember]
      Code32,

      [EnumMember]
      Code9Of3,

      [EnumMember]
      Ean13,

      [EnumMember]
      Ean5,

      [EnumMember]
      Ean8,

      [EnumMember]
      Gs1DataBar,

      [EnumMember]
      I2Of5,

      [EnumMember]
      IntelliMail,

      [EnumMember]
      MSI,

      [EnumMember]
      PatchCode,

      [EnumMember]
      Pdf417,

      [EnumMember]
      Plus2,

      [EnumMember]
      Plus5,

      [EnumMember]
      PharmaCode,

      [EnumMember]
      Postnet,

      [EnumMember]
      QrCode,

      [EnumMember]
      UpcA,

      [EnumMember]
      UpcB,

      [EnumMember]
      UpcE,

      [EnumMember]
      All1D,

      [EnumMember]
      All2D,

      [EnumMember]
      All
    }

    [DataContract(Name = "BarcodeDirections")]
    public enum BarcodeDirections
    {
      [EnumMember]
      NotSet,

      [EnumMember]
      Angle0,

      [EnumMember]
      Angle90,

      [EnumMember]
      Angle180,

      [EnumMember]
      Angle270,
      [EnumMember]
      All
    }
  }
}

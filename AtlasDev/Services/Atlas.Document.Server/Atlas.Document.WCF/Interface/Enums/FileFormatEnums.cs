using System;
using System.Runtime.Serialization;


namespace Atlas.DocServer.WCF.Interface
{
  public class FileFormatEnums
  {
    [DataContract(Name = "FileFormat", Namespace = "urn:Atlas/ASS/DocServer/Enums/2014/05")]
    public enum FormatType
    {
      [EnumMember]
      NotSet = 0,

      [EnumMember]
      ZIP = 1,

      [EnumMember]
      TXT = 2,

      [EnumMember]
      CSV = 3,

      [EnumMember]
      PDF = 4,

      [EnumMember]
      HTML = 5,

      [EnumMember]
      MHT = 6,

      [EnumMember]
      RTF = 11,

      [EnumMember]
      TIFF = 12,

      [EnumMember]
      BMP = 13,

      [EnumMember]
      PNG = 14,

      [EnumMember]
      JPEG = 15,

      [EnumMember]
      JPEG2K = 16,

      [EnumMember]
      EMF = 17,

      [EnumMember]
      XLS = 20,

      [EnumMember]
      XLSX = 21,

      [EnumMember]
      DOC = 22,

      [EnumMember]
      DOCX = 23,

      [EnumMember]
      ODS = 30,

      [EnumMember]
      ODT = 31,

      [EnumMember]
      REPX = 40,

      [EnumMember]
      PRNX = 41,

      [EnumMember]
      SNX = 42
    };

  }
}

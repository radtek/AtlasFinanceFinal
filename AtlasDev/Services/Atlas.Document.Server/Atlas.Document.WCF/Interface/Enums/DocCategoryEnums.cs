using System;
using System.Runtime.Serialization;


namespace Atlas.DocServer.WCF.Interface
{
  [DataContract(Namespace = "urn:Atlas/ASS/DocServer/Admin/2014/05")]  
  public class DocCategoryEnums
  {
    /// <summary>
    /// The document category- i.e. quote, application form, etc.
    /// </summary>
    [DataContract(Name = "Categories", Namespace = "urn:Atlas/ASS/DocServer/Enums/2014/05")]
    public enum Categories
    {
      [EnumMember]
      NotSet = 0,

      [EnumMember]
      Quote = 1,

      [EnumMember]
      Application = 2,

      [EnumMember]
      Contract = 3,

      [EnumMember]
      Receipt = 4,

      [EnumMember]
      Insurance = 5,

      [EnumMember]
      Afford = 6,

      [EnumMember]
      NuCard = 7,

      [EnumMember]
      Employment = 8,

      [EnumMember]
      Bank = 9,

      [EnumMember]
      Identification = 10,

      [EnumMember]
      Statement = 11,

      [EnumMember]
      Scorecard = 12,

      [EnumMember]
      AVS = 13,

      [EnumMember]
      IDVerification = 14,

      [EnumMember]
      Voucher = 15
    };

  }
}

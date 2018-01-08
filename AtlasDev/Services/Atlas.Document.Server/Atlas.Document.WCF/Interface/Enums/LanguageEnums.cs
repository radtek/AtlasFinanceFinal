using System;
using System.Runtime.Serialization;


namespace Atlas.DocServer.WCF.Interface
{
  public class LanguageEnums
  {
    [DataContract(Name = "LanguageEnums", Namespace = "urn:Atlas/ASS/DocServer/Enums/2014/05")]
    public enum Language
    {
      [EnumMember]
      NotSet = 0,

      [EnumMember]
      English = 1,

      [EnumMember]
      Afrikaans = 2,

      [EnumMember]
      Zulu = 3,

      [EnumMember]
      Sotho = 4,

      [EnumMember]
      Xosa = 5
    }

  }
}

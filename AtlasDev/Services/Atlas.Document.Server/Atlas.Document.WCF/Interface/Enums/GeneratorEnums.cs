using System;
using System.Runtime.Serialization;


namespace Atlas.DocServer.WCF.Interface
{
  public class GeneratorEnums
  {
    [DataContract(Name = "Generators", Namespace = "urn:Atlas/ASS/DocServer/Enums/2014/05")]
    public enum Generators
    {
      [EnumMember]
      NotSet = 0,

      [EnumMember]
      Scanner = 1,

      [EnumMember]
      ThirdParty = 2,

      [EnumMember]
      AtlasDocGen = 3,

      [EnumMember]
      email = 4,

      [EnumMember]
      Fax = 5
    };

  }
}

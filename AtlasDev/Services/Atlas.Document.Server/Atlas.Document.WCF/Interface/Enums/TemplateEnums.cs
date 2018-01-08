using System;
using System.Runtime.Serialization;


namespace Atlas.DocServer.WCF.Interface
{
  public class TemplateEnums
  {
    // Needs to match value and name: ** Atlas.Enumerators.Document.DocumentTemplate **
    [DataContract(Name = "TemplateTypes", Namespace = "urn:Atlas/ASS/DocServer/Enums/2014/05")]
    public enum TemplateTypes
    {  
      [EnumMember]
      NotSet = 0,

      [EnumMember]
      Quote_AssQuick = 1,

      [EnumMember]
      Quote_ASS = 2,

      [EnumMember]
      Application_ASSCredit = 3,
      
      [EnumMember]
      Contract_ASS = 4,

      [EnumMember]
      Insurance_ASSPolicy = 5,

      [EnumMember]
      Insurance_ASSWording = 6,  

      [EnumMember]
      Afford_ASS = 7,

      [EnumMember]
      Receipt_ASSNuCard = 8,

      [EnumMember]
      Insurance_VAPBeneficiary = 9,

      [EnumMember]
      Contract_VAP = 10,

      [EnumMember]
      Quote_VAP = 11,

      [EnumMember]
      Insurance_VAPWording = 12
    }
  }
}

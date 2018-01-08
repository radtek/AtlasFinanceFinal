using System;
using System.Runtime.Serialization;


namespace Atlas.DocServer.WCF.Interface
{
  [Serializable]
  [DataContract(Namespace = "urn:Atlas/ASS/DocServer/Recognition/2014/05")]  
  public sealed class DocTemplate
  {
    [DataMember]
    public Int64 TemplateId { get; set; }

    [DataMember]
    public DocCategoryEnums.Categories Category { get; set; }

    [DataMember]
    public TemplateEnums.TemplateTypes TemplateType { get; set; }

    [DataMember]
    public byte[] FileBytes { get; set; }
    
    [DataMember]
    public string Comments { get; set; }
    
    [DataMember]
    public FileFormatEnums.FormatType FileFormatType { get; set; }
   
    [DataMember]
    public int Revision { get; set; }
    
    [DataMember]
    public LanguageEnums.Language Language { get; set; }
  
  }
  
}

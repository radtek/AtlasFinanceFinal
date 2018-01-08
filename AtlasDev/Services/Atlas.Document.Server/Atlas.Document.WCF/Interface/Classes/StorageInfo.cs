using System;
using System.Runtime.Serialization;


namespace Atlas.DocServer.WCF.Interface
{
  [Serializable]
  [DataContract(Namespace = "urn:Atlas/ASS/DocServer/Admin/2014/05")]  
  public sealed class StorageInfo
  {    
    [DataMember]
    public Int64 StorageId { get; set; }

    [DataMember]
    public Int64 ClientPersonId { get; set; }

    [DataMember]
    public byte[] StorageSystemRef { get; set; }

    [DataMember]
    public string Reference { get; set; }

    [DataMember]
    public DocCategoryEnums.Categories Category { get; set; }

    [DataMember]
    public FileFormatEnums.FormatType FileFormatType { get; set; }

    [DataMember]
    public int Revision { get; set; }

    [DataMember]
    public string Comment { get; set; }

    [DataMember]
    public int Size { get; set; }

    [DataMember]
    public string Hash { get; set; } // Provide storage for up to SHA-512 HEX (84 bytes base64 encoded)       

    [DataMember]
    public Int64 SourceTemplateId { get; set; }

    [DataMember]
    public Int64 SourceDocumentId { get; set; } // For scans- the original document

    [DataMember]
    public GeneratorEnums.Generators Generator { get; set; }

    [DataMember]
    public byte[] SourceData { get; set; }

    [DataMember]
    public Int64 CreatedByPersonId { get; set; }

    [DataMember]
    public DateTime CreateDate { get; set; }

    [DataMember]
    public string Keywords { get; set; }

  }
}

using System;
using System.Runtime.Serialization;


namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public sealed class DOC_FileStoreDTO
  {     
    [DataMember]
    public Int64 StorageId { get; set; }
    [DataMember]
    public PER_PersonDTO Client { get; set; }
    [DataMember]
    public byte[] StorageSystemRef { get; set;  }
    [DataMember]
    public string Reference { get; set; }
    [DataMember]
    public DOC_CategoryDTO Category { get; set; }
    [DataMember]
    public DOC_FileFormatTypeDTO FileFormatType { get; set; }
    [DataMember]
    public int Revision { get; set; }
    [DataMember]
    public string Comments { get; set; }
    [DataMember]
    public int Size { get; set; }
    [DataMember]
    public string Hash { get; set; } // Provide storage for up to SHA-512 HEX (84 bytes base64 encoded)   
    [DataMember]
    public DOC_TemplateStoreDTO SourceTemplate { get; set; }
    [DataMember]
    public Enumerators.Document.Generator Generator { get; set; }
    [DataMember]
    public string SourceData { get; set; }
    [DataMember]
    public PER_PersonDTO CreatedBy { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }   
  }
}

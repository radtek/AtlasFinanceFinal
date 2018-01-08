using System;
using System.Runtime.Serialization;


namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public sealed class DOC_TemplateStoreDTO
  {
    [DataMember]
    public Int64 TemplateId { get; set; }

    [DataMember]
    public DOC_TemplateTypeDTO TemplateType { get; set; }

    [DataMember]
    public byte[] FileBytes { get; set; }
    
    [DataMember]
    public string Comment { get; set; }

    [DataMember]
    public DOC_FileFormatTypeDTO TemplateFileFormat { get; set; }

    [DataMember]
    public int Revision { get; set; }
    [DataMember]
    public LNG_LanguageDTO Language { get; set; }

    public PER_PersonDTO CreatedBy { get; set; }

    [DataMember]
    public DateTime CreateDate { get; set; }
  }
}

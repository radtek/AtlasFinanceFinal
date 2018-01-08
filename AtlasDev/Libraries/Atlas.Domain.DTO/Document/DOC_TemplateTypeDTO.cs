using System;
using System.Runtime.Serialization;


  namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class DOC_TemplateTypeDTO
  {
    [DataMember]
    public int TemplateTypeId { get; set; }

    [DataMember]
    public Enumerators.Document.DocumentTemplate Type { get; set; }

    [DataMember]
    public string Description { get; set;}

    [DataMember]
    public DOC_CategoryDTO Category { get; set; }

    [DataMember]
    public DateTime CreateDate { get; set; }

  }
}

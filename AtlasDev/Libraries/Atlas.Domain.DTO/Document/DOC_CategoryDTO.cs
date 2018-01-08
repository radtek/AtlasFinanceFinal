using System;
using System.Runtime.Serialization;


namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public sealed class DOC_CategoryDTO
  {
    [DataMember]
    public Int64 CategoryId { get; set; }
    [DataMember]
    public DOC_CategoryDTO ParentCategory { get; set; }
    [DataMember]
    public Enumerators.Document.Category Type { get; set; }
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public bool Enabled { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }

  }
}

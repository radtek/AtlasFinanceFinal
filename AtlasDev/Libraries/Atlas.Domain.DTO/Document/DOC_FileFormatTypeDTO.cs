using System;
using System.Runtime.Serialization;


namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public sealed class DOC_FileFormatTypeDTO
  {
    [DataMember]
    public int FileFormatTypeId { get; set; }
    [DataMember]
    public Enumerators.Document.FileFormat Type { get; set; }
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }
  }
}

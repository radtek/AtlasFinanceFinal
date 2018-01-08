using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class NTF_TemplateDTO
  {
    [DataMember]
    public int TemplateId { get; set; }
    [DataMember]
    public NTF_TemplateTypeDTO TemplateType { get; set; }
    [DataMember]
    public string Template { get; set; }
    [DataMember]
    public int Version { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }
  }
}
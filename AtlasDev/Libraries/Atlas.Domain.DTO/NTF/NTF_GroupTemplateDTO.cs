using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class NTF_GroupTemplateDTO
  {
    [DataMember]
    public Int64 GroupTemplateId { get; set; }
    [DataMember]
    public NTF_GroupDTO Group { get; set; }
    [DataMember]
    public NTF_TemplateDTO Template { get; set; }
    [DataMember]
    public DateTime? DisableDate { get; set; }
  }
}
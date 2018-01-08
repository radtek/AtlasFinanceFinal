using System;
using System.Runtime.Serialization;


namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class LNG_LanguageDTO
  {
    [DataMember]
    public Int64 LanguageId { get; set; }

    [DataMember]
    public Enumerators.General.Language Type { get; set; }

    [DataMember]
    public string Description { get; set; }
  }
}

using System;
using System.Runtime.Serialization;


namespace Atlas.DocServer.WCF.Interface
{
  [Serializable]
  [DataContract(Namespace = "urn:Atlas/ASS/DocServer/Recognition/2014/05")]  
  public sealed class DocumentFound
  {
    [DataMember]
    public DocCategoryEnums.Categories Category { get; set; }

    [DataMember]
    public int ConfidenceLevel { get; set; }
  }
}

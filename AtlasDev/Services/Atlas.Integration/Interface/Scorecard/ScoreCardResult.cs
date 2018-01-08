using System;
using System.Runtime.Serialization;


namespace Atlas.Integration.Interface
{
  [DataContract(Namespace = "Atlas.Services.2015.Integration.ScoreCard")]
  public class ScoreCardResult
  {
    [DataMember]
    public Int64 EnquiryId { get; set; }

    //[DataMember]
    //public string Reference { get; set; }

    [DataMember]
    public int CodexScore { get; set; }

    [DataMember]
    public AtlasProductResult[] AtlasProducts { get; set; }

    [DataMember]
    public string Comments { get; set; }
    
    [DataMember]
    public string Error { get; set; }
  }
}

using System;
using System.Runtime.Serialization;

namespace Atlas.Integration
{
  [DataContract]
  public class ScoreCardResult
  {
    [DataMember]
    AtlasProduct[] AtlasProducts { get; set; }
  }
}

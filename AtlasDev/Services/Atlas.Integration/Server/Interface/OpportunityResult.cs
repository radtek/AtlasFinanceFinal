using System;
using System.Runtime.Serialization;

namespace Atlas.Integration
{
  [DataContract]
  public class OpportunityResult
  {
    [DataMember]
    public long TrackingId { get; set; }
    [DataMember]
    public string Error { get; set; }
  }
}

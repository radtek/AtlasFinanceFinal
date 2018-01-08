using System;
using System.Runtime.Serialization;

namespace Atlas.Integration
{
  [DataContract]
  public class SendOTPResult
  {
    [DataMember]
    public long TrackingId { get; set; }
    [DataMember]
    public string Error { get; set; }
    [DataMember]
    public string CorrelationId { get; set; }
  }
}

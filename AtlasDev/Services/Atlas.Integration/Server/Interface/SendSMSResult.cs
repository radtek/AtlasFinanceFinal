using System;
using System.Runtime.Serialization;

namespace Atlas.Integration
{
  [DataContract]
  public class SendSMSResult
  {
    [DataMember]
    public long TrackingId { get; set; }
    [DataMember]
    public string Error { get; set; }
  }
}

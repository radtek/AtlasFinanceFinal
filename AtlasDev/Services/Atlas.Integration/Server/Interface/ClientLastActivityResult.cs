using System;
using System.Runtime.Serialization;

namespace Atlas.Integration
{
  [DataContract]
  public class ClientLastActivityResult
  {
    [DataMember]
    public ActivityType Activity { get; set; }
    [DataMember]
    public DateTime Date { get; set; }
    [DataMember]
    public string Notes { get; set; }
  }
}

using System;
using System.Runtime.Serialization;


namespace Atlas.Integration.Interface
{
  [DataContract(Namespace = "Atlas.Services.2015.Integration.SMS")]
  public class SendSMSResult
  {
    [DataMember]
    public long ResultId { get; set; }
    [DataMember]
    public string Error { get; set; }
  }
}

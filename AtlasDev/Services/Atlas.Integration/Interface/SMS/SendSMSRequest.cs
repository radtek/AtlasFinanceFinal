using System;
using System.Runtime.Serialization;


namespace Atlas.Integration.Interface
{
  [DataContract(Namespace = "Atlas.Services.2015.Integration.SMS")]
  public class SendSMSRequest
  {
    [DataMember]
    public string CellularNumber { get; set; }
    [DataMember]
    public string Message { get; set; }    
  }
}

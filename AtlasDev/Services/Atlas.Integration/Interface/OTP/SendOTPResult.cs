using System;
using System.Runtime.Serialization;


namespace Atlas.Integration.Interface
{
  [DataContract(Namespace = "Atlas.Services.2015.Integration.OTP")]
  public class SendOTPResult
  {
    [DataMember]
    public long ResultId { get; set; }   
    [DataMember]
    public string OTP { get; set; }
    [DataMember]
    public string Error { get; set; }
  }
}

using System;
using System.Runtime.Serialization;


namespace Atlas.Integration.Interface
{
  [DataContract(Namespace = "Atlas.Services.2015.Integration.OTP")]
  public class SendOTPRequest
  {
    [DataMember]
    public string CellularNumber { get; set; }
    [DataMember]
    public string MessageTemplate { get; set; }
    [DataMember]
    public string OtpTemplateId { get; set; }
  }
}

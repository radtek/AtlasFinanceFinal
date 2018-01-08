using System;
using System.Runtime.Serialization;

namespace Atlas.Integration
{
  [DataContract]
  public class VerifyOTPResult
  {
    [DataMember]
    public bool OTPMatched { get; set; }

    [DataMember]
    public string Error { get; set; }
  }
}

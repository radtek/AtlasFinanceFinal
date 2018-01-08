using System;
using System.Runtime.Serialization;

namespace Atlas.Integration
{
  [DataContract]
  public class LoginResult
  {
    [DataMember]
    public bool LoginSuccessful { get; set; }
    [DataMember]
    public string LoginToken { get; set; }
    [DataMember]
    public string Error { get; set; }
  }
}

using System;
using System.Runtime.Serialization;


namespace Atlas.Integration.Interface
{
  [DataContract(Namespace = "Atlas.Services.2015.Integration.Login")]
  public class LoginResult
  {
    [DataMember]
    public bool Successful { get; set; }
    [DataMember]
    public string LoginToken { get; set; }
    [DataMember]
    public string Error { get; set; }
  }
}

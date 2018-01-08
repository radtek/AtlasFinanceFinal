using System;
using System.Runtime.Serialization;


namespace Atlas.Integration.Interface
{
  [DataContract(Namespace = "Atlas.Services.2015.Integration.Login")]
  public class UserLoginRequest
  {
    [DataMember]
    public string UserIdNum { get; set; }
    [DataMember]
    public string UserBranch { get; set; }
  }
}

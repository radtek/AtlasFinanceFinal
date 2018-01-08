using System;
using System.Runtime.Serialization;


namespace Atlas.Integration.Interface
{
  [DataContract(Namespace = "Atlas.Services.2015.Integration.Login")]
  public class SystemLoginRequest
  {
    [DataMember]
    public string UserName { get; set; }
    [DataMember]
    public string Password { get; set; }
    [DataMember]
    public string SystemId { get; set; }
  }
}

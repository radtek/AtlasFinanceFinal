using System;
using System.Runtime.Serialization;


namespace Atlas.Integration.Interface
{
  [DataContract(Namespace = "Atlas.Services.2015.Integration.User")]
  public class UsersResult
  {
    [DataMember]
    public UserDetail[] UserList { get; set; }

    [DataMember]
    public string Error { get; set; }

  }
}

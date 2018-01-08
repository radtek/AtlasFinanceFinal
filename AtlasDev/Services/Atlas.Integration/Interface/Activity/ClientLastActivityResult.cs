using System;
using System.Runtime.Serialization;


namespace Atlas.Integration.Interface
{
  [DataContract(Namespace = "Atlas.Services.2015.Integration.Activity")]
  public class ClientLastActivityResult
  {
    [DataMember]
    public LastActivity[] Activities { get; set; }
    [DataMember]
    public string Error { get; set; }

  }
}

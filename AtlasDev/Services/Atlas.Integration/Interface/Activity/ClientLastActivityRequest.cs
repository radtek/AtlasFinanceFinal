using System;
using System.Runtime.Serialization;


namespace Atlas.Integration.Interface
{
  [DataContract(Namespace = "Atlas.Services.2015.Integration.Activity")]
  public class ClientLastActivityRequest
  { 
    [DataMember]
    public string IdNumberOrPassport { get; set;}
    [DataMember]
    public DateTime StartDate { get; set; }
  }
}

using System;
using System.Runtime.Serialization;


namespace Atlas.Integration.Interface
{
  [DataContract(Namespace = "Atlas.Services.2015.Integration.AtlasProduct")]
  public class AtlasProductResult
  {    
    [DataMember]
    public string Product { get; set; }
    [DataMember]
    public bool Passed { get; set; }
    [DataMember]
    public string Reason { get; set; }
  }
}

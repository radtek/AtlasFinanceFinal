using System;
using System.Runtime.Serialization;


namespace Atlas.Integration.Interface
{
  [DataContract(Namespace = "Atlas.Services.2015.Integration.Vetting")]
  public class VettingParameter
  {
    [DataMember]
    public string Parameter { get; set; }
    [DataMember]
    public string Value { get; set; }
    [DataMember]
    public bool? PositiveOutcome { get; set; }
    [DataMember]
    public string Comment { get; set; }
  }
}

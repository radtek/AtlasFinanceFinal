using System;
using System.Runtime.Serialization;


namespace Atlas.Integration.Interface
{
  [DataContract]
  public class BranchDetails
  {
    [DataMember]
    public string Code { get; set; }
    [DataMember]
    public string Description { get; set; }
  }
}

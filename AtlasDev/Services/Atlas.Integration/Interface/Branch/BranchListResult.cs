using System;
using System.Runtime.Serialization;


namespace Atlas.Integration.Interface
{
  [DataContract(Namespace = "Atlas.Services.2015.Integration.Branch")]
  public class BranchListResult
  {
    [DataMember]
    public BranchDetail[] BranchList { get; set; }
    [DataMember]
    public string Error { get; set; }
  }
}

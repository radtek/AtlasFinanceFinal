using System;
using System.Runtime.Serialization;


namespace Atlas.Integration.Interface
{
  [DataContract(Namespace = "Atlas.Services.2015.Integration.Branch")]
  public class BranchDetail
  {
    [DataMember]
    public string Code { get; set; }
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public string EMailAddress { get; set; }
    [DataMember]
    public string Region { get; set; }
    [DataMember]
    public string ManagerName { get; set; }
  }

}

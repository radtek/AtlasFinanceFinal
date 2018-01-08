using System;
using System.Runtime.Serialization;


namespace Atlas.Integration.Interface
{
  [DataContract(Namespace = "Atlas.Services.2015.Integration.Opportunity")]
  public class CheckOpportunityStatusRequest
  {
    [DataMember]
    public int[] AddOpportunityResultIds { get; set; }
  }
}

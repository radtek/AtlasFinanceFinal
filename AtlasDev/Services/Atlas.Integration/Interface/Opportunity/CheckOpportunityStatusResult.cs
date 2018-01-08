using System;
using System.Runtime.Serialization;


namespace Atlas.Integration.Interface
{
  [DataContract(Namespace = "Atlas.Services.2015.Integration.Opportunity")]
  public class CheckOpportunityStatusResult
  {    
    [DataMember]
    public OpportunityStatus[] Status { get; set; }

    [DataMember]
    public string Error { get; set; }

  }
}

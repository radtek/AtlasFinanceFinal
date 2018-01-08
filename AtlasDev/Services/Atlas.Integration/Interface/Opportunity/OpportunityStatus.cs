using System;
using System.Runtime.Serialization;


namespace Atlas.Integration.Interface
{
  [DataContract(Namespace = "Atlas.Services.2015.Integration.Opportunity")]
  public class OpportunityStatus
  {
    [DataMember]
    public long AddOpportunityResultId { get; set; }

    [DataMember]
    public OpportunityStates Status { get; set; }

    [DataMember]
    public string Comment { get; set; }
        
    [DataMember]
    public string GrantedBranch { get; set; }

    [DataMember]
    public DateTime GrantedDate { get; set; }

    [DataMember]
    public decimal GrantedLoanAmount { get; set; }

    [DataMember]
    public int GrantedPeriodMonths { get; set; }

    [DataMember]
    public string Error { get; set; }

  }


  [DataContract(Namespace = "Atlas.Services.2015.Integration.Opportunity")]
  public enum OpportunityStates
  {
    [EnumMember]
    NotFound = 0,
    [EnumMember]
    Pending = 1,
    [EnumMember]
    Failed = 2,
    [EnumMember]
    Successful = 3
  }
}

using System;
using System.Runtime.Serialization;


namespace Atlas.Integration.Interface
{
  [DataContract(Namespace = "Atlas.Services.2015.Integration.Opportunity")]
  public class AddOpportunityRequest
  {
    [DataMember]
    public string CallerReferenceId { get; set; }
    [DataMember]
    public DateTime Started { get; set; }
    [DataMember]
    public DateTime Completed { get; set; }
    [DataMember]
    public DateTime FollowUpStart { get; set; }
    [DataMember]
    public string UserID { get; set; }
    [DataMember]
    public string BranchCode { get; set; }

    [DataMember]
    public string GPSLocation { get; set; }
    [DataMember]
    public string CellularNumber { get; set; }
    [DataMember]
    public string IdNumber { get; set; }
    [DataMember]
    public string FirstName { get; set; }
    [DataMember]
    public string Surname { get; set; }
    [DataMember]
    public DateTime DateOfBirth { get; set; }

    [DataMember]
    public long ScoreCardEnquiryId { get; set; }

    [DataMember]
    public bool Successful { get; set; }

    [DataMember]
    public VettingParameter[] VettingParameters { get; set; }

  }
}

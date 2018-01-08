using System;
using System.Runtime.Serialization;

namespace Atlas.Integration
{
  [DataContract]
  public class OpportunityRequest
  {
    public DateTime Started { get; set; }
    public DateTime Completed { get; set; }
    public DateTime FollowUp { get; set; }
    public string UserID { get; set; }
    public string Branch { get; set; }

    public string CellNumber { get; set; }
    public string IdNumber { get; set; }
    public string FirstName { get; set; }
    public string Surname { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Parameters[] VettingParameters { get; set; }

  }
}

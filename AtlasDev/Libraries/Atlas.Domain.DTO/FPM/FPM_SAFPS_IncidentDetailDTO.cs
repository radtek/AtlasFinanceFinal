using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  using System;

    [Serializable]
  [DataContract(IsReference = true)]
  public sealed class FPM_SAFPS_IncidentDetailDTO
  {
    [DataMember]
    public Int64 IncidentDetailId { get;set;}
    [DataMember]
    public bool Victim { get;set;}
    [DataMember]
    public string MembersReference { get;set;}
    [DataMember]
    public string Category { get; set; }
    [DataMember]
    public string SubCategory { get;set;}
    [DataMember]
    public string IncidentDate { get;set;}
    [DataMember]
    public string SubRole {get;set;}
    [DataMember]
    public string City { get;set;}
    [DataMember]
    public string Detail { get;set;}
    [DataMember]
    public string Forensic { get;set;}

  }
}

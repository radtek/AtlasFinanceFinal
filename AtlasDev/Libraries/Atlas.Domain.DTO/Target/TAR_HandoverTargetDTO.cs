using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class TAR_HandoverTargetDTO
  {
    [DataMember]
    public int TargetId { get; set; }
    [DataMember]
    public BRN_BranchDTO Branch { get; set; }
    [DataMember]
    public HostDTO Host { get; set; }
    [DataMember]
    public DateTime StartRange { get; set; }
    [DataMember]
    public DateTime EndRange { get; set; }
    [DataMember]
    public DateTime? DisableDate { get; set; }
    [DataMember]
    public DateTime ActiveDate { get; set; }
    [DataMember]
    public decimal HandoverBudget { get; set; }
    [DataMember]
    public float ArrearTarget { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }
    [DataMember]
    public PER_PersonDTO CreateUser { get; set; }
  }
}
using System;
using System.Runtime.Serialization;


namespace Atlas.Domain.DTO.Nucard
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class NUC_NuCardDTO
  {
    [DataMember]
    public Int64 NuCardId { get; set; }
    [DataMember]
    public PER_PersonDTO AllocatedPerson { get; set; }
    [DataMember]
    public BRN_BranchDTO IssuedByBranch { get; set; }
    [DataMember]
    public NUC_NuCardProfileDTO NuCardProfile { get; set; }
    [DataMember]
    public string SequenceNum { get; set; }
    [DataMember]
    public string TrackingNum { get; set; }
    [DataMember]
    public string CardNum { get; set; }
    [DataMember]
    public DateTime IssueDT { get; set; }
    [DataMember]
    public DateTime ExpiryDT { get; set; }
    public NUC_NuCardStatusDTO Status { get; set; }
    [DataMember]
    public PER_PersonDTO CreatedBy { get; set; }
    [DataMember]
    public PER_PersonDTO DeletedBy { get; set; }
    [DataMember]
    public PER_PersonDTO LastEditedBy { get; set; }
    [DataMember]
    public DateTime? CreatedDT { get; set; }
    [DataMember]
    public DateTime? DeletedDT { get; set; }
    [DataMember]
    public DateTime? LastEditedDT { get; set; }
    [DataMember]

    /* Non Model Related */
    public string ClientNum { get; set; }
    [DataMember]
    public string LegacyBranchNum { get; set; }
    [DataMember]
    public DateTime? UserDT { get; set; }
  }
}

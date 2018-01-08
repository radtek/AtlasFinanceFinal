using System;
using System.Runtime.Serialization;


namespace Atlas.Domain.DTO.Nucard
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class NUC_NuCardBatchDTO
  {
    [DataMember]
    public Int64 NuCardBatchId { get; set; }
    [DataMember]
    public BRN_BranchDTO DeliverToBranch { get; set; }
    [DataMember]
    public BRN_BranchDTO ReceivedByBranch { get; set; }
    [DataMember]
    public string BundleNum { get; set; }
    [DataMember]
    public string SequenceStart { get; set; }
    [DataMember]
    public string SequenceEnd { get; set; }
    [DataMember]
    public string OutSequence { get; set; }
    [DataMember]
    public DateTime? DeliveryDT { get; set; }
    [DataMember]
    public DateTime? ReceivedDT { get; set; }
    [DataMember]
    public DateTime? SentDT { get; set; }
    [DataMember]
    public PER_PersonDTO SentBy { get; set; }
    [DataMember]
    public PER_PersonDTO ReceivedBy { get; set; }
    [DataMember]
    public CPY_CompanyDTO Courier { get; set; }
    [DataMember]
    public int QuantitySent { get; set; }
    [DataMember]
    public int QuantityReceived { get; set; }
    [DataMember]
    public Enumerators.General.NucardBatchStatus Status { get; set; }
    [DataMember]
    public string TrackingNum { get; set; }
    [DataMember]
    public string Comment { get; set; }
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
  }
}


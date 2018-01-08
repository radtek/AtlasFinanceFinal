using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class AVS_BatchDTO
  {
    [DataMember]
    public Int64 BatchId { get; set; }
    [DataMember]
    public AVS_ServiceDTO Service { get; set; }
    [DataMember]
    public int TotalRecords { get; set; }
    [DataMember]
    public int TransmissionNo { get; set; }
    [DataMember]
    public int GenerationNo { get; set; }
    [DataMember]
    public int FirstSequenceNo { get; set; }
    [DataMember]
    public int LastSequenceNo { get; set; }
    [DataMember]
    public bool? TransmissionAccepted { get; set; }
    [DataMember]
    public bool? GenerationAccepted { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }
    [DataMember]
    public DateTime? SubmitDate { get; set; }
    [DataMember]
    public DateTime? ReplyDate { get; set; }
    [DataMember]
    public string ErrorMessage { get; set; }
  }
}

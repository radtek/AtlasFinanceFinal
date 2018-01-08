using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class PYT_TransmissionDTO
  {
    [DataMember]
    public long TransmissionId { get; set; }
    [DataMember]
    public PYT_BatchDTO Batch { get; set; }
    [DataMember]
    public int TransmissionNo { get; set; }
    [DataMember]
    public bool? Accepted { get; set; }
    [DataMember]
    public PYT_ReplyCodeDTO ReplyCode { get; set; }
    [DataMember]
    public DateTime? ReplyDate { get; set; }
    [DataMember]
    public string FilePath { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }
  }
}
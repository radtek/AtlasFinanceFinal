using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class PYT_TransmissionSetDTO
  {
    [DataMember]
    public long TransmissionSetId { get; set; }
    [DataMember]
    public PYT_TransmissionDTO Transmission { get; set; }
    [DataMember]
    public int GenerationNo { get; set; }
    [DataMember]
    public bool? Accepted { get; set; }
    [DataMember]
    public PYT_ReplyCodeDTO ReplyCode { get; set; }
    [DataMember]
    public DateTime? ReplyDate { get; set; }
  }
}

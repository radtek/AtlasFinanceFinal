using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class PYT_TransmissionTransactionDTO
  {

    [DataMember]
    public long TransmissionTransactionId { get; set; }
    [DataMember]
    public PYT_TransmissionSetDTO TransmissionSet { get; set; }
    [DataMember]
    public PYT_BatchDTO Batch { get; set; }
    [DataMember]
    public PYT_PayoutDTO Payout { get; set; }
    [DataMember]
    public int SequenceNo { get; set; }
    [DataMember]
    public bool? Accepted { get; set; }
    [DataMember]
    public PYT_ReplyCodeDTO ReplyCode { get; set; }
    [DataMember]
    public DateTime? ReplyDate { get; set; }
  }
}

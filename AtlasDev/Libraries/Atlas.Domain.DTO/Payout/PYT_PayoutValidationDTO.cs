using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class PYT_PayoutValidationDTO
  {
    [DataMember]
    public int PayoutValidationId { get; set; }
    [DataMember]
    public PYT_PayoutDTO Payout { get; set; }
    [DataMember]
    public PYT_ValidationDTO Validation { get; set; }
  }
}

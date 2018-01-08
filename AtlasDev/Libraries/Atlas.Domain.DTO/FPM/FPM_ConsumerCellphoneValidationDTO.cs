using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  using System;

    [Serializable]
  [DataContract(IsReference = true)]
  public sealed class FPM_ConsumerCellphoneValidationDTO
  {
    [DataMember]
    public Int64 ConsumerCellPhoneValidationId { get; set; }
    [DataMember]
    public FPM_FraudScoreDTO FraudScore { get; set; }
    [DataMember]
    public string CellularNumber { get; set; }
    [DataMember]
    public string CellularVerification { get; set; }
    [DataMember]
    public DateTime? CellularFirstUsed { get; set; }
  }
}

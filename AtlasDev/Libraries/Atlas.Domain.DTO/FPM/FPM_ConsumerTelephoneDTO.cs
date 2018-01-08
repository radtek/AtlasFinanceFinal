using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  using System;

    [Serializable]
  [DataContract(IsReference = true)]
  public sealed class FPM_ConsumerTelephoneDTO
  {
    [DataMember]
    public Int64 ConsumerTelephoneId { get; set; }
    [DataMember]
    public FPM_FraudScoreDTO FraudScore { get; set; }
    [DataMember]
    public string TelephoneNumberDial { get; set; }
    [DataMember]
    public string TelephoneNumber { get; set; }
    [DataMember]
    public int TelephoneTotal24Hours { get; set; }
    [DataMember]
    public int TelephoneTotal48Hours { get; set; }
    [DataMember]
    public int TelephoneTotal96Hours { get; set; }
    [DataMember]
    public int TelephoneTotal30Days { get; set; }
    [DataMember]
    public string CellPhoneNumber { get; set; }
    [DataMember]
    public int CellPhoneTotal24Hours { get; set; }
    [DataMember]
    public int CellPhoneTotal48Hours { get; set; }
    [DataMember]
    public int CellPhoneTotal96Hours { get; set; }
    [DataMember]
    public int CellPhoneTotal30Days { get; set; }
  }
}

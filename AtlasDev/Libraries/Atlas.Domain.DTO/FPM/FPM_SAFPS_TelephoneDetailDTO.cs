using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  using System;


    [Serializable]
  [DataContract(IsReference = true)]
  public sealed class FPM_SAFPS_TelephoneDetailDTO
  {
    [DataMember]
    public Int64 TelephoneDetailId { get; set; }
    [DataMember]
    public FPM_SAFPSDTO SAFPS { get; set; }
    [DataMember]
    public string TelephoneType { get; set; }
    [DataMember]
    public string TelephoneNo { get; set; }
    [DataMember]
    public string TelephoneCity { get; set; }
    [DataMember]
    public string Country { get; set; }
  }
}

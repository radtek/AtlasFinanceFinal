using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  using System;

    [Serializable]
  [DataContract(IsReference = true)]
  public sealed class FPM_SAFPS_BankDetailDTO
  {
    [DataMember]
    public Int64 BankDetailId { get; set; }
    [DataMember]
    public FPM_SAFPSDTO SAFPS { get; set; }
    [DataMember]
    public string AccountNo { get; set; }
    [DataMember]
    public string AccountType { get; set; }
    [DataMember]
    public string Bank { get; set; }
    [DataMember]
    public string BankFrom { get; set; }
    [DataMember]
    public string BankTo { get; set; }
  }
}

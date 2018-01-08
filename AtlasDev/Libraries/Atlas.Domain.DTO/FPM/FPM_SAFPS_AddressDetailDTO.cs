using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  using System;

    [Serializable]
  [DataContract(IsReference = true)]
  public sealed class FPM_SAFPS_AddressDetailDTO
  {
    [DataMember]
    public Int64 AddressDetail { get;set;}
    [DataMember]
    public FPM_SAFPSDTO SAFPS { get;set;}
    [DataMember]
    public string Type { get;set;}
    [DataMember]
    public string Street { get; set; }
    [DataMember]
    public string Address { get; set; }
    [DataMember]
    public string City { get; set; }
    [DataMember]
    public string PostalCode { get; set; }
    [DataMember]
    public string From { get; set; }
    [DataMember]
    public string To { get;set;}
  }
}

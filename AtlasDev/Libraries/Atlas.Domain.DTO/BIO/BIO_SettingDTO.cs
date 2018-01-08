using System.Runtime.Serialization;
using System;
using Atlas.Enumerators;


namespace Atlas.Domain.DTO
{ 
  [Serializable]
  [DataContract]
  public class BIO_SettingDTO
  {
    [DataMember]
    public Int64 FPSettingId { get; set; }
    [DataMember]
    public Biometric.AppliesTo AppliesTo { get; set; }
    [DataMember]
    public Biometric.SettingType SettingType { get; set; }
    [DataMember]
    public Int64 Entity { get; set; }
    [DataMember]
    public string Value { get; set; }
  }
}

using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  using System;

    [Serializable]
  [DataContract]
  public sealed class FPM_SAFPS_OtherIdDetailDTO
  {
    [DataMember]
    public Int64 OtherIdDetailId { get;set;}
    [DataMember]
    public string IDNo { get;set;}
    [DataMember]
    public string Type { get;set;}
    [DataMember]
    public string IssueDate { get;set;}
    [DataMember]
    public string Country { get;set;}
  }
}

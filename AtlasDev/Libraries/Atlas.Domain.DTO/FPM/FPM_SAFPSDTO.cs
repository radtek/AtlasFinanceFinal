using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  using System;

    [Serializable]
  [DataContract]
  public sealed class FPM_SAFPSDTO
  {
    [DataMember]
    public Int64 SafpsId { get; set; }
    [DataMember]
    public string Reference { get; set; }
    [DataMember]
    public string Category { get; set; }
    [DataMember]
    public string SubCategory { get; set; }
    [DataMember]
    public string Subject { get; set; }
    [DataMember]
    public string Passport { get; set; }
    [DataMember]
    public DateTime IncidentDate { get; set; }
    [DataMember]
    public bool Victim { get; set; }

  }
}

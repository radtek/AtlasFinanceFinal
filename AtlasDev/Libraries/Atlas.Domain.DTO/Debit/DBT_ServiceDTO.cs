using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class DBT_ServiceDTO
  {
    [DataMember]
    public int ServiceId { get; set; }
    [DataMember]
    public string ReferenceName { get; set; }
    [DataMember]
    public string UserCode { get; set; }
    [DataMember]
    public string UserName { get; set; }
    [DataMember]
    public string UserReference { get; set; }
    [DataMember]
    public char Environment { get; set; }
    [DataMember]
    public string AccountNo { get; set; }
    [DataMember]
    public string BranchCode { get; set; }
    [DataMember]
    public int NextTransmissionNo { get; set; }
    [DataMember]
    public int NextGenerationNo { get; set; }
    [DataMember]
    public int NextSequenceNo { get; set; }
    [DataMember]
    public DateTime LastSequenceUpdate { get; set; }
    [DataMember]
    public string OutgoingFilePath { get; set; }
    [DataMember]
    public string IncomingFilePath { get; set; }
    [DataMember]
    public string ArchiveFilePath { get; set; }
    [DataMember]
    public bool Enabled { get; set; }
  }
}
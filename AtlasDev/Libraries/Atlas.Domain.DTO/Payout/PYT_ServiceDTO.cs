using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class PYT_ServiceDTO
  {
    [DataMember]

    public int ServiceId { get; set; }
    public PYT_ServiceTypeDTO ServiceType { get; set; }
    [DataMember]
    public string UserCode { get; set; }
    [DataMember]
    public string Username { get; set; }
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public string ReferenceName { get; set; }
    [DataMember]
    public string BranchCode { get; set; }
    [DataMember]
    public string AccountNo { get; set; }
    [DataMember]
    public string SwiftCode { get; set; }
    [DataMember]
    public string BankServeCode { get; set; }
    [DataMember]
    public int NextTransmissionNo { get; set; }
    [DataMember]
    public int NextGenerationNo { get; set; }
    [DataMember]
    public int NextSequenceNo { get; set; }
    [DataMember]
    public string Address1 { get; set; }
    [DataMember]
    public string Address2 { get; set; }
    [DataMember]
    public string Address3 { get; set; }
    [DataMember]
    public string Address4 { get; set; }
    [DataMember]
    public string OutgoingPath { get; set; }
    [DataMember]
    public string IncomingPath { get; set; }
    [DataMember]
    public string ArchivePath { get; set; }
    [DataMember]
    public bool Enabled { get; set; }
  }
}

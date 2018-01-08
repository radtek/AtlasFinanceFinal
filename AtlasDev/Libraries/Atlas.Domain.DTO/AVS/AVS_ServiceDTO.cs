using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class AVS_ServiceDTO
  {
    [DataMember]
    public Int32 ServiceId { get; set; }
    [DataMember]
    public Enumerators.AVS.Service Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.AVS.Service>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.AVS.Service>();
      }
    }
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public string UserCode { get; set; }
    [DataMember]
    public string Username { get; set; }
    [DataMember]
    public string BankServCode { get; set; }
    [DataMember]
    public bool AwaitReply { get; set; }
    [DataMember]
    public bool Enabled { get; set; }
    [DataMember]
    public int SendInterval { get; set; }
    [DataMember]
    public int NextGenerationNo { get; set; }
    [DataMember]
    public int NextTransmissionNo { get; set; }
    [DataMember]
    public string OutgoingPath { get; set; }
    [DataMember]
    public string IncomingPath { get; set; }
    [DataMember]
    public string ArchivePath { get; set; }
    [DataMember]
    public char EnvironmentFlag { get; set; }
  }
}
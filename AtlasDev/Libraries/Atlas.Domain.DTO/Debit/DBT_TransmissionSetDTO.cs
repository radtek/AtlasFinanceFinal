using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class DBT_TransmissionSetDTO
  {
    [DataMember]
    public Int64 TransmissionSetId { get;set;}
    [DataMember]
    public DBT_TransmissionDTO Transmission { get;set;}
    [DataMember]
    public int GenerationNo { get;set;}
    [DataMember]
    public bool Accepted { get; set; }
    [DataMember]
    public DBT_ReplyCodeDTO ReplyCode { get;set;}
    [DataMember]
    public DateTime? CreateDate { get; set; }
  }
}
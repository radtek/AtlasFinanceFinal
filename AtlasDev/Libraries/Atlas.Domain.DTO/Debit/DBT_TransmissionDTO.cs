using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class DBT_TransmissionDTO
  {
    [DataMember]
    public Int64 TransmissionId { get;set;}
    [DataMember]
    public DBT_BatchDTO Batch { get;set;}
    [DataMember]
    public int TransmissionNo { get;set;}
    [DataMember]
    public bool? Accepted { get;set;}
    [DataMember]
    public DBT_ReplyCodeDTO ReplyCode { get;set;}
    [DataMember]
    public DateTime? ReplyDate { get;set;}
    [DataMember]
    public DateTime? CreateDate { get; set; }
  }
}
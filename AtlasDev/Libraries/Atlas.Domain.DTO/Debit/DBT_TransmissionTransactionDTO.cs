using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class DBT_TransmissionTransactionDTO
  {
    [DataMember]
    public Int64 TransmissionTransactionId { get;set;}
    [DataMember]
    public DBT_TransmissionSetDTO TransmissionSet { get;set;}
    [DataMember]
    public DBT_TransactionDTO Transaction { get;set;}
    [DataMember]
    public DBT_BatchDTO Batch { get;set;}
    [DataMember]
    public int? SequenceNo { get;set;}
    [DataMember]
    public bool Accepted { get;set;}
    [DataMember]
    public DBT_ReplyCodeDTO ReplyCode { get;set;}
    [DataMember]
    public DateTime CreateDate { get; set; }
  }
}
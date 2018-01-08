using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO.Nucard
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class NUC_TransactionDTO
  {
    [DataMember]
    public Int64 NucardTransactionId {get;set;}
    [DataMember]
    public NUC_NuCardDTO NuCard { get; set; }
    [DataMember]
    public string Description {get;set;}
    [DataMember]
    public string ReferenceNum {get;set;}
    [DataMember]
    public Decimal? Amount {get;set;}
    [DataMember]
    public DateTime? LoadDT {get;set;}
    [DataMember]
    public bool IsPending {get;set;}
    [DataMember]
    public TransactionSourceDTO Source {get;set;}
    [DataMember]
    public string ServerTransactionId {get;set;}
    [DataMember]
    public Enumerators.General.ApplicationIdentifiers SourceApplication{get;set;}
    [DataMember]
    public PER_PersonDTO CreatedBy {get;set;}
    [DataMember]

    public PER_PersonDTO DeletedBy {get;set;}
    [DataMember]
    public PER_PersonDTO LastEditedBy {get;set;}
    [DataMember]
    public DateTime? CreatedDT {get;set;}
    [DataMember]
    public DateTime? DeletedDT {get;set;}
    [DataMember]
    public DateTime? LastEditedDT { get; set; }
  }
}
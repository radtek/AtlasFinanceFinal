using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class ACC_QuotationDTO
  { 
    [DataMember]
    public Int64 QuotationId{get;set;}
    [DataMember]
    public string QuotationNo{get;set;}
    [DataMember]
    public ACC_AccountDTO Account{get;set;}
    [DataMember]
    public ACC_AffordabilityOptionDTO AffordabilityOption{get;set;}
    [DataMember]
    public ACC_QuotationStatusDTO QuotationStatus{get;set;}
    [DataMember]
    public DateTime LastStatusDate{get;set;}
    [DataMember]
    public DateTime ExpiryDate{get;set;}
    [DataMember]
    public DateTime CreateDate{get;set;}
    [DataMember]
    public PER_PersonDTO CreateUser { get; set; }
  }
}

using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class ACC_SettlementDTO
  {
    [DataMember]
    public long SettlementId { get; set; }
    [DataMember]
    public ACC_AccountDTO Account { get; set; }
    [DataMember]
    public ACC_SettlementStatusDTO SettlementStatus { get; set; }
    [DataMember]
    public ACC_SettlementTypeDTO SettlementType { get; set; }
    [DataMember]
    public DateTime LastStatusChange { get; set; }
    [DataMember]
    public decimal Amount { get; set; }
    [DataMember]
    public decimal Fees { get; set; }
    [DataMember]
    public decimal Interest { get; set; }
    [DataMember]
    public decimal TotalAmount { get; set; }
    [DataMember]
    public DateTime SettlementDate { get; set; }
    [DataMember]
    public DateTime ExpirationDate { get; set; }
    [DataMember]
    public PER_PersonDTO CreateUser { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }
  }
}
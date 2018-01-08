using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class ACC_ArrearageDTO
  {
    [DataMember]
    public Int64 ArrearageId { get; set; }
    [DataMember]
    public ACC_AccountDTO Account { get; set; }
    [DataMember]
    public DateTime PeriodStart { get; set; }
    [DataMember]
    public DateTime PeriodEnd { get; set; }
    [DataMember]
    public int ArrearageCycle { get; set; }
    [DataMember]
    public int DelinquencyRank { get; set; }
    [DataMember]
    public decimal InstalmentDue { get; set; }
    [DataMember]
    public decimal AmountPaid { get; set; }
    [DataMember]
    public decimal TotalPaid { get; set; }
    [DataMember]
    public decimal ArrearsAmount { get; set; }
    [DataMember]
    public decimal TotalArrearsAmount { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }

  }
}

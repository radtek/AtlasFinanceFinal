using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class ACC_TopUpDTO
  {
    [DataMember]
    public Int64 TopUpId { get; set; }
    [DataMember]
    public ACC_AccountDTO Account { get; set; }
    [DataMember]
    public ACC_TopUpStatusDTO TopUpStatus { get; set; }
    [DataMember]
    public Decimal TopUpAmount { get; set; }
    [DataMember]
    public Decimal TotalFees { get; set; }
    [DataMember]
    public Decimal CapitalAmount { get; set; }
    [DataMember]
    public DateTime CreatedDate { get; set; }
    [DataMember]
    public PER_PersonDTO CreatedBy { get; set; }
    [DataMember]
    public DateTime? PayoutDate { get; set; }
  }
}
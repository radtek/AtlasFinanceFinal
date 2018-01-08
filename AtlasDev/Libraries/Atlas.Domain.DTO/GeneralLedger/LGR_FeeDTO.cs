using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class LGR_FeeDTO
  {
    [DataMember]
    public Int64 FeeId { get; set; }
    [DataMember]
    public LGR_TransactionTypeDTO TransactionType { get; set; }
    [DataMember]
    public decimal? Amount { get; set; }
    [DataMember]
    public float? Percentage { get; set; }
    [DataMember]
    public float? VAT { get; set; }
    [DataMember]
    public bool? CalculateOnAccountBalance { get; set; }
    [DataMember]
    public Enumerators.GeneralLedger.FeeRangeType FeeRangeType { get; set; }
    [DataMember]
    public decimal? RangeStart { get; set; }
    [DataMember]
    public decimal? RangeEnd { get; set; }
    [DataMember]
    public bool IsInitial { get; set; }
    [DataMember]
    public bool IsRecurring { get; set; }
    [DataMember]
    public bool OnSettlement { get; set; }
    [DataMember]
    public bool IsArrearageFee { get; set; }
    [DataMember]
    public bool AddWithNewTopUp { get; set; }
    [DataMember]
    public decimal? LessAmount { get; set; }
    [DataMember]
    public decimal? MaxAmount { get; set; }
  }
}
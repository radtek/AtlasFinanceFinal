using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class ACC_AffordabilityOptionFeeDTO
  {
    [DataMember]
    public Int64 AffordabilityOptionFeeId { get; set; }
    [DataMember]
    public ACC_AffordabilityOptionDTO AffordabilityOption { get; set; }
    [DataMember]
    public ACC_AccountTypeFeeDTO AccountTypeFee { get; set; }
    [DataMember]
    public Decimal Amount { get; set; }
  }
}

using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class ACC_AccountFeeDTO
  {
    [DataMember]
    public Int64 AccountFeeId { get; set; }
    [DataMember]
    public ACC_AccountDTO Account { get; set; }
    [DataMember]
    public ACC_TopUpDTO TopUp { get; set; }
    [DataMember]
    public ACC_AccountTypeFeeDTO AccountTypeFee { get; set; }
    [DataMember]
    public Decimal Amount { get; set; }
  }
}

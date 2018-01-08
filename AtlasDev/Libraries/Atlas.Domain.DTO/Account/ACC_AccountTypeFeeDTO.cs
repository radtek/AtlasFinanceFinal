using System.Runtime.Serialization;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class ACC_AccountTypeFeeDTO
  {
    [DataMember]
    public Int64 AccountTypeFeeId { get; set; }
    [DataMember]
    public ACC_AccountTypeDTO AccountType { get; set; }
    [DataMember]
    public LGR_FeeDTO Fee { get; set; }
    [DataMember]
    public DateTime? EffectiveDate { get; set; }
    [DataMember]
    public bool Enabled { get; set; }
    [DataMember]
    public DateTime? DisabledDate { get; set; }
    [DataMember]
    public int Ordinal { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }
    [DataMember]
    public PER_PersonDTO CreateUser { get; set; }
  }
}
using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class ACC_AccountPayRuleDTO
  {
    [DataMember]
    public int AccountPayRuleId { get; set; }
    [DataMember]
    public ACC_AccountDTO Account { get; set; }
    [DataMember]
    public ACC_PayRuleDTO PayRule { get; set; }
    [DataMember]
    public ACC_PayDateDTO PayDate { get; set; }
    [DataMember]
    public bool Enabled { get; set; }
    [DataMember]
    public DateTime? DisabledDate { get; set; }
    [DataMember]
    public DateTime? CreateDate { get; set; }
    [DataMember]
    public PER_PersonDTO CreateUser { get; set; }
  }
}

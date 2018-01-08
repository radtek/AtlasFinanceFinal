using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference=true)]
  public class ACC_AffordabilityDTO
  {
    [DataMember]
    public Int64 AffordabilityId { get; set; }
    [DataMember]
    public ACC_AccountDTO Account { get; set; }
    [DataMember]
    public ACC_AffordabilityCategoryDTO AffordabilityCategory { get; set; }
    [DataMember]
    public Decimal Amount { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }
    [DataMember]
    public PER_PersonDTO CreateUser { get; set; }
    [DataMember]
    public DateTime? DeleteDate { get; set; }
    [DataMember]
    public PER_PersonDTO DeleteUser { get; set; }
  }
}
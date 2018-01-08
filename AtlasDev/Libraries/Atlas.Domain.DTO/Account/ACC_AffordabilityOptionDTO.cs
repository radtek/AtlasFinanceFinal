using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class ACC_AffordabilityOptionDTO
  {
    [DataMember]
    public Int64 AffordabilityOptionId { get; set; }
    [DataMember]
    public ACC_AccountDTO Account { get; set; }
    [DataMember]
    public ACC_TopUpDTO TopUp { get; set; }
    [DataMember]
    public ACC_AccountTypeDTO AccountType { get; set; }
    [DataMember]
    public Decimal Amount { get; set; }
    [DataMember]
    public Decimal TotalFees { get; set; }
    [DataMember]
    public Decimal CapitalAmount { get; set; }
    [DataMember]
    public Decimal? TotalPayBack { get; set; }
    [DataMember]
    public Decimal? Instalment { get; set; }
    [DataMember]
    public int NumOfInstalment { get; set; }
    [DataMember]
    public int Period { get; set; }
    [DataMember]
    public ACC_PeriodFrequencyDTO PeriodFrequency { get; set; }
    [DataMember]
    public DateTime? LastStatusDate { get; set; }
    [DataMember]
    public ACC_AffordabilityOptionStatusDTO AffordabilityOptionStatus { get; set; }
    [DataMember]
    public ACC_AffordabilityOptionTypeDTO AffordabilityOptionType { get; set; }
    [DataMember]
    public float? InterestRate { get; set; }
    [DataMember]
    public bool? CanClientAfford { get; set; }
    [DataMember]
    public DateTime CreatedDate { get; set; }
    [DataMember]
    public PER_PersonDTO CreatedBy { get; set; }
  }
}
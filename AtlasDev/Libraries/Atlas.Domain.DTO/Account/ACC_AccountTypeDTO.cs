using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class ACC_AccountTypeDTO
  {
    [DataMember]
    public Int64 AccountTypeId { get; set; }
    [DataMember]
    public HostDTO Host { get; set; }
    [DataMember]
    public ACC_PeriodFrequencyDTO PeriodFrequency { get; set; }
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public decimal CloseBalance { get; set; }
    [DataMember]
    public int MinPeriod { get; set; }
    [DataMember]
    public int? MaxPeriod { get; set; }
    [DataMember]
    public decimal MinAmount { get; set; }
    [DataMember]
    public decimal? MaxAmount { get; set; }
    [DataMember]
    public float InterestRate { get; set; }
    [DataMember]
    public float? RepoRate { get; set; }
    [DataMember]
    public float? RepoFactor { get; set; }
    [DataMember]
    public int? BufferDaysFirstInstalmentDate { get; set; }
    [DataMember]
    public int? InterestFreePeriods { get; set; }
    [DataMember]
    public int QuotationExpiryPeriod { get; set; }
    [DataMember]
    public bool? AllowAffordabilityOptions { get; set; }
    [DataMember]
    public float? AffordabilityPercentBuffer { get; set; }
    [DataMember]
    public int DefaultTrackingDays { get; set; }
    [DataMember]
    public int ArrearageBufferPeriod { get; set; }
    [DataMember]
    public int SettlementExpirationBuffer { get; set; }
    [DataMember]
    public int Ordinal { get; set; }
    [DataMember]
    public bool Enabled { get; set; }
    [DataMember]
    public DateTime? DisabledDate { get; set; }
  }
}
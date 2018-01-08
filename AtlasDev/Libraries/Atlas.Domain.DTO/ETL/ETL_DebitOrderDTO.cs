using System;

namespace Atlas.Domain.DTO
{
  public class ETL_DebitOrderDTO
  {
    public long DebitOrderId { get; set; }
    public ETL_StageDTO Stage { get; set; }
    public DateTime LastStageDate { get; set; }
    public string ErrorMessage { get; set; }
    public ETL_DebitOrderBatchDTO DebitOrderBatch { get; set; }
    public DBT_ControlDTO DebitOrderControl { get; set; }
    public string ThirdPartyReference { get; set; }
    public string BankStatementReference { get; set; }
    public string IdNumber { get; set; }
    public string AccountNumber { get; set; }
    public string AccountName { get; set; }
    public BankDTO Bank { get; set; }
    public string BankBranchCode { get; set; }
    public DateTime FirstActionDate { get; set; }
    public int Repititions { get; set; }
    public BankAccountTypeDTO BankAccountType { get; set; }
    public decimal InstalmentAmount { get; set; }
    public ACC_PeriodFrequencyDTO PeriodFrequency { get; set; }
    public int TrackingDays { get; set; }
    public ACC_PayRuleDTO PayRule { get; set; }
    public ACC_PayDateTypeDTO PayDateType { get; set; }
    public int PayDateNo { get; set; }
  }
}
using System;

namespace Atlas.Reporting.DTO
{
  public class ASS_CiReportPossibleHandovers
  {
    public long ParentId { get; set; }

    public long Id { get; set; }

    public string Name { get; set; }

    public float PossibleHandovers { get; set; }

    public float HandoverBudget { get; set; }

    public float Variance { get; set; }

    public float ActualVsBudgetPercent { get; set; }

    public float NextPossibleHandovers { get; set; }

    public float NextMonthBreakEven { get; set; }

    public float DebtorsBookValue { get; set; }

    public float ActualArrears { get; set; }

    public float ArrearTargetPercent { get; set; }

    public float ArrearTarget { get; set; }

    public float ArrearsToDebtorsBookPercent { get; set; }

    public float CollectionsThisMonthPercent { get; set; }

    public float CollectionsPastPercent { get; set; }

    public float FlaggedNoOfLoans { get; set; }

    public float FlaggedOverdueValue { get; set; }

    public DateTime OldestArrearsDate { get; set; }
  }
}
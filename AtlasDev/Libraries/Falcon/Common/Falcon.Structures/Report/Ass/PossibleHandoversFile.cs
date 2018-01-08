using System;
using System.ComponentModel;

namespace Falcon.Common.Structures.Report.Ass
{
  public class PossibleHandoversFile
  {
    [Description("Branch/Region")]
    public string Branch { get; set; }

    [Description("Hand Over (Computer)")]
    public decimal HandOverComputer { get; set; }

    [Description("Handover Budget")]
    public decimal HandoverBudget { get; set; }

    [Description("Variance Minus = Short")]
    public decimal ShortFall { get; set; }

    [Description("Actual vs. Budget %")]
    public float ActualVsBudget { get; set; }

    [Description("Handover Value Next Month")]
    public decimal HandoverValueNextMonth { get; set; }

    [Description("Next month H/O for break even")]
    public decimal NextMonthHandoverForBreakEven { get; set; }

    [Description("D/Book")]
    public decimal DebtorBook { get; set; }

    [Description("Act arr")]
    public decimal ActualArrears { get; set; }

    [Description("Arr Target (7%)")]
    public decimal ArrearsTarget { get; set; }

    [Description("% To D/Book")]
    public float PercentToDebtorsBook { get; set; }

    [Description("This month")]
    public decimal CollectionsThisMonth { get; set; }

    [Description("Past ({0})")]
    public decimal CollectionsPrevMonth { get; set; }

    [Description("Oldest Arrear Date")]
    public DateTime? OldestArrearDate { get; set; }

    [Description("Value")]
    public decimal FlaggedLoansOverdue { get; set; }

    [Description("Quantity")]
    public int FlaggedLoans { get; set; }
  }
}
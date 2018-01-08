using System;
using System.ComponentModel;
using Atlas.Common.Attributes;
using Falcon.Exporter.CiReport.Infrastructure.Attributes;

namespace Falcon.Exporter.CiReport.Infrastructure.Structures.Reports
{
  public class PossibleHandover
  {
    [Description("ParentId")]
    public long ParentId { get; set; }

    [Description("Id")]
    public long Id { get; set; }

    [Description("Name")]
    [Order(1)]
    [HeaderFormat]
    public string Name { get; set; }

    [Description("Possible Handover")]
    [Order(2)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float PossibleHandovers { get; set; }

    [Description("Handover Budget")]
    [Order(3)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float HandoverBudget { get; set; }

    [Description("Variance (minus = short)")]
    [Order(4)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float Variance { get; set; }

    [Description("Actual Vs Budget")]
    [Order(5)]
    [Format("0.00%")]
    [DetailFormat("Black", "LightCoral", "right")]
    public float ActualVsBudgetPercent { get; set; }

    [Description("Next Month Possible Handover")]
    [Order(6)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float NextPossibleHandovers { get; set; }

    [Description("Next Month H/O to Break-even")]
    [Order(7)]
    [Format("#,##0;-#,##0")]
    [DetailFormat("Black", "Yellow", "right")]
    public float NextMonthBreakEven { get; set; }

    [Description("Debtors Book")]
    [Order(8)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float DebtorsBookValue { get; set; }

    [Description("Actual Arrears")]
    [Order(9)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float ActualArrears { get; set; }

    [Format("0.00%")]
    public float ArrearTargetPercent { get; set; }

    [Description("Arrear Target {{ArrearTargetPercent}}")]
    [Order(10)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float ArrearTarget { get; set; }

    [Description("% to D/Book")]
    [Order(11)]
    [Format("0.00%")]
    [DetailFormat(alignment: "right")]
    public float ArrearsToDebtorsBookPercent { get; set; }

    [Description("This Month")]
    [Order(12)]
    [Format("0.00%")]
    [DetailFormat(alignment: "right")]
    public float CollectionsThisMonthPercent { get; set; }

    [Description("Past ({{OldestArrearsDate}})")]
    [Order(13)]
    [Format("0.00%")]
    [DetailFormat(alignment: "right")]
    public float CollectionsPastPercent { get; set; }

    [Description("Quantity")]
    [Order(14)]
    [DetailFormat(alignment: "right")]
    public float FlaggedNoOfLoans { get; set; }

    [Description("Value")]
    [Order(15)]
    [Format("#,##0;-#,##0")]
    [DetailFormat(alignment: "right")]
    public float FlaggedOverdueValue { get; set; }

    public DateTime OldestArrearsDate { get; set; }
  }
}
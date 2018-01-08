using System;


namespace Atlas.Accounts.Reports.QuartzTasks
{
  class Amounts
  {
    public string BranchName;
    public string Branch;

    public Periods Period;
    public long Sched_Qty;
    public decimal Sched_Val;

    public long Paid_Qty;
    public decimal Paid_Val;

    public enum Periods
    {
      Today,
      TillYesterday,
      TillMonthEnd
    }

  }
}

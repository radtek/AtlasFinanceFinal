using System;
using System.ComponentModel;

using Atlas.Enumerators.EnumAttributes;


namespace Atlas.Enumerators
{
  public class Evolution
  {
    /// <summary>
    /// 
    /// </summary>
    public enum BatchTypes
    {
      [Description("Not set")]
      NotSet = 0,

      [Description("Daily extract")]
      Daily = 1,

      [Description("Monthly extract")]
      Monthly = 2,

      [Description("Ad-hoc/QE")]
      AdHoc = 3
    };


    /// <summary>
    /// 
    /// </summary>
    public enum AssLoanStates
    {
      [Description("Not set")]
      NotSet = 0,

      [Description("Adjustment")]
      [ShortCode("A")]
      Adjustment = 1,

      [Description("Cancelled")]
      [ShortCode("C")]
      Cancelled = 2,

      [Description("Discounted")]
      [ShortCode("D")]
      Discounted = 3,

      [Description("Eary settled")]
      [ShortCode("E")]
      EarlySettled = 4,

      [Description("Fully paid up loan and closed")]
      [ShortCode("F")]
      FullyPaid = 5,

      [Description("Handed over")]
      [ShortCode("H")]
      HandedOver = 6,

      [Description("Journalised")]
      [ShortCode("J")]
      Journalised = 7,

      [Description("New loan without any transactions")]
      [ShortCode("N")]
      NewLoan = 8,

      [Description("Part payment")]
      [ShortCode("P")]
      PartPayment = 9,

      [Description("Refunded")]
      [ShortCode("R")]
      Refunded = 10,

      [Description("Re-scheduled")]
      [ShortCode("S")]
      ReScheduled = 11,

      [Description("Written off")]
      [ShortCode("W")]
      WrittenOff = 12
    }


    public enum SnapshotReasonTypes
    {
      [Description("Not set")]
      NotSet = 0,

      [Description("Snapshot is for a closure")]
      Closure = 1,

      [Description("Snapshot is for a registration")]
      Registration = 2,

      [Description("Snapshot is for monthly, active accounts, excluding the above")]
      Active = 3
    }


    public enum StagingState
    {
      [Description("Not set")]
      NotSet = 0,

      [Description("New file generated and available")]
      New = 1,

      [Description("File ready to be uploaded")]
      Ready = 2,

      [Description("Completed")]
      Completed = 3

    }
  }
}

using System;
using System.ComponentModel;

namespace SchedulerServer.AltechNuPay.Enumerators
{  
  public static class AEDO
  {
    public enum UserType
    {
      [Description("M")]
      Merchant,
      [Description("G")]
      Group,
      [Description("S")]
      SubGroup
    }

    public enum ReportType
    {
      [Description("SuccessTran")]
      SuccessfulTransactions = 1,
      [Description("FailedTran")]
      FailedTransactions = 2,
      [Description("RetryTran")]
      RetryTransactions = 3,
      [Description("FutureTran")]
      FutureTransactions = 4,
      [Description("CancelledTran")]
      CancelledTransactions = 5,
      [Description("NewTranLoaded")]
      NewTransactionsLoaded = 6,
      [Description("SettledTran")]
      SettledTransaction = 7,
      [Description("UnSettledTran")]
      UnsettledTransaction = 8,
      [Description("UnMatchedTran")]
      UnmatchedTransaction = 9
    }
  }
}

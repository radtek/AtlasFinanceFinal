using System;
using System.ComponentModel;

namespace SchedulerServer.AltechNuPay.Enumerators
{
  public static class NAEDO
  {
    public enum ServiceType
    {
      TwoDayDebit = 2,
      SSV = 4, // Same Day Soonest
      CreditCardAccountDebit = 11,
      Naedo = 19
    }
    public enum ReportType
    {
      [Description("UploadedTran")]
      TransactionsUploaded = 1,
      [Description("FutureTran")]
      FutureTransactions = 2,
      [Description("InProcessTran")]
      TransactionsInProgress = 3,
      [Description("SuccessTran")]
      SuccessfulTransactions = 4,
      [Description("FailedTran")]
      FailedTransactions = 5,
      [Description("CancelledTran")]
      CancelledTransactions = 6,
      [Description("DisputedTran")]
      DisputedTransactions = 7
    }
  }
}
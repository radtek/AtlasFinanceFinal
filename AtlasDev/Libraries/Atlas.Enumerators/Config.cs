using System;
using System.ComponentModel;


namespace Atlas.Enumerators
{
  public class Config
  {
    #region Payout

    public enum Payout
    {
      [Description("RTC Export Period")]
      RTCExportPeriod = 61000, // Minutes
      [Description("RTC Threshold Period")]
      RTCThresholdPeriod = 61001, // Minutes
      [Description("RTC Monitor Period")]
      RTCMonitorPeriod = 61002 // Minutes
    }

    #endregion

    public enum AVS
    {
      [Description("Hyphen Key")]
      HyphenKey = 60000,
      [Description("Expiration Period")]
      ExpirationPeriod = 60001, // Days
      [Description("Poll Period")]
      PollPeriod = 60002, // Minutes
      [Description("Monitor Period")]
      MonitorPeriod = 60003, // Minutes
      [Description("ThresholdPeriod")]
      ThresholdPeriod = 60004, // Minutes
      [Description("XDS Key")]
      XDSKey = 60005,
      [Description("CompuScan Key")]
      CompuScanKey = 60006,
      [Description]
      NuCardKey = 60007,
    }

    public enum DebitOrder
    {
      [Description("Export Period")]
      ExportPeriod = 63000, // Minutes
      [Description("Post To GL Period")]
      PostToGLPeriod = 63001, // Minutes
      [Description("Link New Transaction Period")]
      LinkNewTransactionPeriod = 63002, // Minutes
      [Description("Update Debit Order Period")]
      UpdateDebitOrderPeriod = 63003, // Minutes
      [Description("ETL Load Period")]
      ETLLoadPeriod = 63004, // Minutes
      [Description("Monitor Period")]
      MonitorPeriod = 63005, // Minutes
      [Description("Monitor Threshold")]
      MonitorThreshold = 63006, // Minutes
    }

    public enum GeneralLedger
    {
      [Description("AccPac Export Path")]
      AccPaccExportPath = 62001
    }

    #region [dbo].[Config].[DataType] lookup values

    // AEDO settings
    // -------------------------------------------------------------------------------------------------------------------------------------
    // Maximum AEDO monthly instalment
    public const int CONFIG_AEDO_MAX_INSTALMENT = 5000;


    // TCC settings
    // -------------------------------------------------------------------------------------------------------------------------------------
    // Template for a standard contract
    public const int CONFIG_AEDO_TCC_LINE1_TEMPLATE = 10000;
    public const int CONFIG_AEDO_TCC_LINE2_TEMPLATE = 10001;
    public const int CONFIG_AEDO_TCC_LINE3_TEMPLATE = 10002;
    public const int CONFIG_AEDO_TCC_LINE4_TEMPLATE = 10003;

    // Template when we need to split into multiple contracts, when > R5000 instalment
    public const int CONFIG_AEDO_TCC_LINE1_TEMPLATE_MULTI = 10010;
    public const int CONFIG_AEDO_TCC_LINE2_TEMPLATE_MULTI = 10011;
    public const int CONFIG_AEDO_TCC_LINE3_TEMPLATE_MULTI = 10012;
    public const int CONFIG_AEDO_TCC_LINE4_TEMPLATE_MULTI = 10013;

    public enum Alerting
    {
      [Description("Manual receipts- automated report")]
      ManualReceipts = 11000,

      [Description("AVS Usage- automated report")]
      AVSUsage = 11001,

      [Description("Day before still overdue- automated report")]
      DayBeforeOverdue = 11002,

      [Description("First instalment overdue- automated report for all- morning")]
      FirstInstalmentOverdueAll = 11003,

      [Description("First instalment overdue- automated report for sales- evening")]
      FirstInstalmentOverdueSales = 11004,

      [Description("85%- automated report for sales- evening")]
      Receipts85Percent = 11005,

      [Description("Month to month audit trail")]
      MonthToMonthAudit = 11006
    }

    // Tutuka NuCard
    // -------------------------------------------------------------------------------------------------------------------------------------
    public const int CONFIG_NUCARD_XMLRPC_ENDPOINT = 31000; // http://voucherengine.tutuka.com/handlers/remote/profilexmlrpc.cfm

    #endregion


    #region [dbo].[Altech].[AEDOTransaction].[Status]

    public const int AEDOTRANS_CONTRACT_STATUS_OK = 1;
    public const int AEDOTRANS_CONTRACT_STATUS_TO_DELETE = 10;  // Contract to be deleted
    public const int AEDOTRANS_CONTRACT_STATUS_WAS_DELETED = 11; // Contract was deleted
    public const int AEDOTRANS_CONTRACT_STATUS_FAILED_DELETE = 12; // Contract could not be deleted

    #endregion
  }
}

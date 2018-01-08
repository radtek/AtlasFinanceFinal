﻿using System;
using System.Globalization;
using System.Xml.Linq;

using Atlas.Domain.DTO;


namespace SchedulerServer.AltechNuPay.Report.TSP
{
  public class FutureReport
  {
    #region public Fields

    public TSPReportFutureDTO naedoReport;

    #endregion

    public FutureReport(XElement item)
    {
      naedoReport = new TSPReportFutureDTO()
      {
        TransactionId = Int64.Parse(item.Element("TranID").Value),
        TransactionType = item.Element("tran_type").Value,
        ProcessMerchant = item.Element("process_merchant").Value,
        ClientRef1 = item.Element("ClientRef1").Value,
        ClientRef2 = item.Element("ClientRef2").Value,
        ActionDate = DateTime.ParseExact(item.Element("ActionDate").Value, "yyyyMMdd", CultureInfo.InvariantCulture),
        NumInstallments = item.Element("NumInstallments").Value,
        AccountName = item.Element("HomingAccName").Value,
        Value = Convert.ToDecimal(item.Element("value").Value, CultureInfo.InvariantCulture)
      };
    }
  }
}
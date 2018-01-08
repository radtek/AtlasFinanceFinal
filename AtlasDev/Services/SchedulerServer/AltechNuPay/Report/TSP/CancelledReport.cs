using System;
using System.Globalization;
using System.Xml.Linq;

using Atlas.Domain.DTO;


namespace SchedulerServer.AltechNuPay.Report.TSP
{
  public class CancelledReport
  {
    #region public Fields

    public TSPReportCancelledDTO naedoReport;

    #endregion

    public CancelledReport(XElement item)
    {
      naedoReport = new TSPReportCancelledDTO()
      {
        TransactionId = Int64.Parse(item.Element("TranID").Value),
        TransactionType = item.Element("tran_type").Value,
        ProcessMerchant = item.Element("process_merchant").Value,
        ClientRef1 = item.Element("ClientRef1").Value,
        ClientRef2 = item.Element("ClientRef2").Value,
        ActionDate = DateTime.ParseExact(item.Element("ActionDate").Value, "yyyyMMdd", CultureInfo.InvariantCulture),
        CancelDate = DateTime.ParseExact(item.Element("CancelDate").Value, "yyyyMMdd", CultureInfo.InvariantCulture),
        AccountName = item.Element("HomingAccName").Value,
        Value = Convert.ToDecimal(item.Element("value").Value, CultureInfo.InvariantCulture)
      };
    }
  }
}

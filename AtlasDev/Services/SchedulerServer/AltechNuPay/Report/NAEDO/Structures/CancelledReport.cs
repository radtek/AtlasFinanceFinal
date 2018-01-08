using System;
using System.Globalization;
using System.Xml.Linq;

using Atlas.Domain.DTO;


namespace SchedulerServer.AltechNuPay.Report.NAEDO.Structures
{
  public class CancelledReport
  {
    #region public Fields

    public NAEDOReportCancelledDTO naedoReport;

    #endregion

    public CancelledReport(XElement item)
    {
      naedoReport = new NAEDOReportCancelledDTO();

      naedoReport.ReportCancelled.TransactionId = Int64.Parse(item.Element("TranID").Value);
      naedoReport.ReportCancelled.TransactionTypeId = int.Parse(item.Element("TTypeID").Value);
      naedoReport.ReportCancelled.ProcessMerchant = item.Element("process_merchant").Value;
      naedoReport.ReportCancelled.ClientRef1 = item.Element("ClientRef1").Value;
      naedoReport.ReportCancelled.ClientRef2 = item.Element("ClientRef2").Value;
      naedoReport.ReportCancelled.ActionDT = DateTime.ParseExact(item.Element("ActionDate").Value, "M/d/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
      naedoReport.ReplyDT = string.IsNullOrEmpty(item.Element("ReplyDate").Value) ? (DateTime?)null : DateTime.ParseExact(item.Element("ReplyDate").Value, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
      naedoReport.HomingAccountName = item.Element("HomingAccName").Value;
      naedoReport.Amount = decimal.Parse(item.Element("Amount").Value, CultureInfo.InvariantCulture);
      naedoReport.CCardNum = item.Element("CCardNumber").Value;
    }
  }
}

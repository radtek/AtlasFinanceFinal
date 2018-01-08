using System;
using System.Globalization;
using System.Xml.Linq;

using Atlas.Domain.DTO;


namespace SchedulerServer.AltechNuPay.Report.NAEDO.Structures
{
  public class FutureReport
  {
    #region public Fields

    public NAEDOReportFutureDTO naedoReport;

    #endregion

    public FutureReport(XElement item)
    {
      naedoReport = new NAEDOReportFutureDTO();

      naedoReport.ReportFuture.TransactionId = Int64.Parse(item.Element("TranID").Value);
      naedoReport.ReportFuture.TransactionTypeId = int.Parse(item.Element("TTypeID").Value);
      naedoReport.ReportFuture.ProcessMerchant = item.Element("process_merchant").Value;
      naedoReport.ReportFuture.ClientRef1 = item.Element("ClientRef1").Value;
      naedoReport.ReportFuture.ClientRef2 = item.Element("ClientRef2").Value;
      naedoReport.ReportFuture.ActionDT = DateTime.ParseExact(item.Element("ActionDate").Value, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
      naedoReport.NumInstallments = item.Element("NumInstallments").Value;
      naedoReport.HomingAccountName = item.Element("HomingAccName").Value;
      naedoReport.Amount = decimal.Parse(item.Element("Amount").Value, CultureInfo.InvariantCulture);
      naedoReport.CCardNum = item.Element("CCardNumber").Value;
      naedoReport.Tracking = item.Element("tracking").Value;
    }
  }
}

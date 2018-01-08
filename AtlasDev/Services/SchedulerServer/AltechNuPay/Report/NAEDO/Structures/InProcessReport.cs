using System;
using System.Globalization;
using System.Xml.Linq;

using Atlas.Domain.DTO;


namespace SchedulerServer.AltechNuPay.Report.NAEDO.Structures
{
  public class InProcessReport
  {
    #region public Fields

    public NAEDOReportInProcessDTO naedoReport;

    #endregion

    public InProcessReport(XElement item)
    {
      naedoReport = new NAEDOReportInProcessDTO();

      naedoReport.ReportInProcess.TransactionId = Int64.Parse(item.Element("TranID").Value);
      naedoReport.ReportInProcess.TransactionTypeId = int.Parse(item.Element("TTypeID").Value);
      naedoReport.ReportInProcess.ProcessMerchant = item.Element("process_merchant").Value;
      naedoReport.ReportInProcess.ClientRef1 = item.Element("ClientRef1").Value;
      naedoReport.ReportInProcess.ClientRef2 = item.Element("ClientRef2").Value;
      naedoReport.ReportInProcess.ActionDT = DateTime.ParseExact(item.Element("ActionDate").Value, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
      naedoReport.NumInstallments = item.Element("NumInstallments").Value;
      naedoReport.HomingAccountName = item.Element("HomingAccName").Value;
      naedoReport.Amount = decimal.Parse(item.Element("Amount").Value, CultureInfo.InvariantCulture);
      naedoReport.CCardNum = item.Element("CCardNumber").Value;
      naedoReport.Tracking = item.Element("tracking").Value;
      naedoReport.TrackDT = DateTime.ParseExact(item.Element("trackdate").Value, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
      naedoReport.InstStatus = item.Element("instStatus").Value;
    }
  }
}

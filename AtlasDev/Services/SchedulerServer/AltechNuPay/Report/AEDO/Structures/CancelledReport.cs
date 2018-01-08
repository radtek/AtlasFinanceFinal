using System;
using System.Globalization;
using System.Xml.Linq;

using Atlas.Domain.DTO;


namespace SchedulerServer.AltechNuPay.Report.AEDO.Structures
{
  public class CancelledReport
  {
    #region public Fields

    public AEDOReportCancelledDTO aedoReport;

    #endregion

    public CancelledReport(XElement item)
    {
      aedoReport = new AEDOReportCancelledDTO();

      aedoReport.ReportCancelled.TransactionId = Int64.Parse(item.Element("transaction_id").Value);
      aedoReport.ReportCancelled.ContractRef = item.Element("contract_ref").Value;
      aedoReport.ReportCancelled.ServiceType = item.Element("service_type").Value;
      aedoReport.ReportCancelled.ValueDT = DateTime.ParseExact(item.Element("value_date").Value, "yyyyMMdd", CultureInfo.InvariantCulture);
      aedoReport.ReportCancelled.CancelDT = DateTime.ParseExact(string.Format("{0} {1}", item.Element("cancel_date").Value, item.Element("cancel_time").Value), "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture);
      aedoReport.TrackingIndicator = item.Element("tracking_indicator").Value;
      aedoReport.ContractNum = item.Element("contract_no").Value;
      aedoReport.CancellationType = item.Element("type_cancellation").Value;
      aedoReport.CancelMerchant = item.Element("cancel_merchant").Value;
      aedoReport.EmployerCode = item.Element("employer_code").Value;
      aedoReport.Pan = item.Element("pan").Value;
      aedoReport.TerminalNum = int.Parse(item.Element("terminal_no").Value);
      aedoReport.InstitutionId = item.Element("institution_id").Value;
      aedoReport.IdNumber = item.Element("id_number").Value;
    }
  }
}

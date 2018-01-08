using System;
using System.Globalization;
using System.Xml.Linq;

using Atlas.Domain.DTO;


namespace SchedulerServer.AltechNuPay.Report.AEDO.Structures
{
  public class SuccessReport
  {
    #region public Fields

    public AEDOReportSuccessDTO aedoReport;

    #endregion

    public SuccessReport(XElement item)
    {
      aedoReport = new AEDOReportSuccessDTO();

      aedoReport.ReportSuccess.TransactionId = Int64.Parse(item.Element("transaction_id").Value);
      aedoReport.ReportSuccess.ContractRef = item.Element("contract_ref").Value;
      aedoReport.ReportSuccess.ServiceType = item.Element("service_type").Value;
      aedoReport.ReportSuccess.ValueDT = DateTime.ParseExact(item.Element("value_date").Value, "yyyyMMdd", CultureInfo.InvariantCulture);
      aedoReport.ReportSuccess.SuccessDT = DateTime.ParseExact(string.Format("{0} {1}", item.Element("success_date").Value, item.Element("success_time").Value), "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture);
      aedoReport.StartDT = DateTime.ParseExact(item.Element("start_date").Value, "yyyyMMdd", CultureInfo.InvariantCulture);
      aedoReport.ContractNum = item.Element("contract_no").Value;
      aedoReport.ContractAmount = FileStructureHelper.ConvertFromCents(item.Element("contract_amount").Value);
      aedoReport.InstalmentNum = int.Parse(item.Element("instalment_no").Value);
      aedoReport.InstalmentAmount = FileStructureHelper.ConvertFromCents(item.Element("instalment_amount").Value);
      aedoReport.EmployerCode = item.Element("employer_code").Value;
      aedoReport.TrackingIndicator = item.Element("tracking_indicator").Value;
      aedoReport.Frequency = int.Parse(item.Element("frequency").Value);
      aedoReport.Pan = item.Element("pan").Value;
      aedoReport.TerminalNum = int.Parse(item.Element("terminal_no").Value);
      aedoReport.CardAcceptor = item.Element("card_acceptor").Value;
      aedoReport.InstitutionId = item.Element("institution_id").Value;
      aedoReport.IdNumber = item.Element("id_number").Value;
    }
  }
}

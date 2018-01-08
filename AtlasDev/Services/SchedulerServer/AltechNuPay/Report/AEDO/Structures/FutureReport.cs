using System;
using System.Globalization;
using System.Xml.Linq;

using Atlas.Domain.DTO;


namespace SchedulerServer.AltechNuPay.Report.AEDO.Structures
{
  public class FutureReport
  {
    #region public Fields

    public AEDOReportFutureDTO aedoReport;

    #endregion

    public FutureReport(XElement item)
    {
      aedoReport = new AEDOReportFutureDTO();

      aedoReport.ReportFuture.TransactionId = Int64.Parse(item.Element("transaction_id").Value);
      aedoReport.ReportFuture.ContractRef = item.Element("contract_ref").Value;
      aedoReport.ReportFuture.ServiceType = item.Element("service_type").Value;
      aedoReport.ReportFuture.ValueDT = DateTime.ParseExact(item.Element("value_date").Value, "yyyyMMdd", CultureInfo.InvariantCulture);
      aedoReport.ReportFuture.SubmitDT = DateTime.ParseExact(string.Format("{0} {1}", item.Element("submit_date").Value, item.Element("submit_time").Value), "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture);
      aedoReport.StartDT = DateTime.ParseExact(item.Element("start_date").Value, "yyyyMMdd", null);
      aedoReport.LastSubmissionDT = !string.IsNullOrEmpty(item.Element("last_submission_date").Value) ?
        DateTime.ParseExact(string.Format("{0} {1}", item.Element("last_submission_date").Value, item.Element("last_submission_time").Value), "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture) : (DateTime?)null;
      aedoReport.SubmitCount = item.Element("submit_count").Value;
      aedoReport.RetryReason = item.Element("retry_reason").Value;
      aedoReport.ContractNum = item.Element("contract_no").Value;
      aedoReport.ContractAmount = FileStructureHelper.ConvertFromCents(item.Element("contract_amount").Value);
      aedoReport.Term = int.Parse(item.Element("term").Value);
      aedoReport.Instalments = int.Parse(item.Element("instalments").Value);
      aedoReport.InstalmentNum = int.Parse(item.Element("instalment_no").Value);
      aedoReport.InstalmentAmount = FileStructureHelper.ConvertFromCents(item.Element("instalment_amount").Value);
      aedoReport.EmployerCode = item.Element("employer_code").Value;
      aedoReport.Frequency = int.Parse(item.Element("frequency").Value);
      aedoReport.DateAdjustRule = int.Parse(item.Element("date_adjust_rule").Value);
      aedoReport.TrackingIndicator = item.Element("tracking_indicator").Value;
      aedoReport.Pan = item.Element("pan").Value;
      aedoReport.TerminalNum = int.Parse(item.Element("terminal_no").Value);
      aedoReport.CardAcceptor = item.Element("card_acceptor").Value;
      aedoReport.Active = string.Compare(item.Element("active").Value, "1") == 0;
      aedoReport.InstitutionId = item.Element("institution_id").Value;
      aedoReport.IdNumber = item.Element("id_number").Value;
    }
  }
}

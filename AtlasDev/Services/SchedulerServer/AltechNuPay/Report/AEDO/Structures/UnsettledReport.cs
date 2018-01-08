using System;
using System.Globalization;
using System.Xml.Linq;

using Atlas.Domain.DTO;


namespace SchedulerServer.AltechNuPay.Report.AEDO.Structures
{
  public class UnsettledReport
  {
    #region public Fields

    public AEDOReportUnsettledDTO aedoReport;

    #endregion

    public UnsettledReport(XElement item)
    {
      aedoReport = new AEDOReportUnsettledDTO();

      aedoReport.ReportUnsettled.TransactionId = Int64.Parse(item.Element("transaction_id").Value);
      aedoReport.ReportUnsettled.ValueDT = DateTime.ParseExact(item.Element("value_date").Value, "yyyyMMdd", CultureInfo.InvariantCulture);
      aedoReport.TermId = int.Parse(item.Element("term_id").Value);
      aedoReport.MerchantNum = item.Element("merchant_no").Value;
      aedoReport.Pan = item.Element("pan").Value;
      aedoReport.Instalment = int.Parse(item.Element("instalment").Value);
      aedoReport.ContractAmount = FileStructureHelper.ConvertFromCents(item.Element("contract_amount").Value);
      aedoReport.ActualDT = DateTime.ParseExact(item.Element("actual_date").Value, "yyyyMMdd", CultureInfo.InvariantCulture);
      aedoReport.ContractNum = item.Element("contract_no").Value;
      aedoReport.StartDT = DateTime.ParseExact(item.Element("start_date").Value, "yyyyMMdd", CultureInfo.InvariantCulture);
      aedoReport.CurrencyCode = item.Element("currency_code_tran").Value;
      aedoReport.Amount = FileStructureHelper.ConvertFromCents(item.Element("amount").Value);
      aedoReport.Frequency = int.Parse(item.Element("frequency").Value);
      aedoReport.EmployerCode = item.Element("employer_code").Value;
      aedoReport.ContractRef = item.Element("contract_ref").Value;
      aedoReport.IdNumber = item.Element("id_number").Value;
    }

  }
}

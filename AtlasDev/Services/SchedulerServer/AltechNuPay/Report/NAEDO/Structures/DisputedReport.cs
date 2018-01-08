using System;
using System.Globalization;
using System.Xml.Linq;

using Atlas.Domain.DTO;


namespace SchedulerServer.AltechNuPay.Report.NAEDO.Structures
{
  public class DisputedReport
  {
    #region public Fields

    public NAEDOReportDisputedDTO naedoReport;

    #endregion

    public DisputedReport(XElement item)
    {
      naedoReport = new NAEDOReportDisputedDTO();

      naedoReport.ReportDisputed.TransactionId = Int64.Parse(item.Element("TranID").Value);
      naedoReport.ReportDisputed.TransactionTypeId = int.Parse(item.Element("TTypeID").Value);
      naedoReport.ReportDisputed.ProcessMerchant = item.Element("process_merchant").Value;
      naedoReport.ReportDisputed.ClientRef1 = item.Element("ClientRef1").Value;
      naedoReport.ReportDisputed.ClientRef2 = item.Element("ClientRef2").Value;
      naedoReport.ReportDisputed.ActionDT = DateTime.ParseExact(item.Element("ActionDate").Value, "M/d/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
      naedoReport.ReplyDT = string.IsNullOrEmpty(item.Element("ReplyDate").Value) ?
        (DateTime?)null : DateTime.ParseExact(item.Element("ReplyDate").Value, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
      naedoReport.NumInstallments = item.Element("NumInstallments").Value;
      naedoReport.HomingAccountName = item.Element("HomingAccName").Value;
      naedoReport.Amount = decimal.Parse(item.Element("Amount").Value, CultureInfo.InvariantCulture);
      naedoReport.RCode = item.Element("RCode").Value;
      naedoReport.QCode = item.Element("QCode").Value;
      naedoReport.AccountType = item.Element("acctype").Value;
      naedoReport.HomingBranch = item.Element("HomingBranch").Value;
      naedoReport.CCardNum = item.Element("CCardNumber").Value;
    }
  }
}

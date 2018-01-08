using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Atlas.Domain.DTO;
using System.Globalization;

namespace Atlas.ThirdParty.AltechNuPay.Report.NAEDO
{
    public class FailedReport
    {
        #region public Fields

        public NAEDOReportFailedDTO naedoReport;

        #endregion

        public FailedReport(XElement item)
        {
            naedoReport = new NAEDOReportFailedDTO();

            naedoReport.ReportFailed.TransactionId = Int64.Parse(item.Element("TranID").Value);
            naedoReport.ReportFailed.TransactionTypeId = int.Parse(item.Element("TTypeID").Value);
            naedoReport.ReportFailed.ProcessMerchant = item.Element("process_merchant").Value;
            naedoReport.ReportFailed.ClientRef1 = item.Element("ClientRef1").Value;
            naedoReport.ReportFailed.ClientRef2 = item.Element("ClientRef2").Value;
            naedoReport.ReportFailed.ActionDT = DateTime.ParseExact(item.Element("ActionDate").Value, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
            naedoReport.ReplyDT = string.IsNullOrEmpty(item.Element("ReplyDate").Value) ? (DateTime?)null : DateTime.ParseExact(item.Element("ReplyDate").Value, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
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

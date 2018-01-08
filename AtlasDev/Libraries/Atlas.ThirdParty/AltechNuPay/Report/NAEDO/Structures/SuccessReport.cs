using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using System.Globalization;

namespace Atlas.ThirdParty.AltechNuPay.Report.NAEDO
{
    public class SuccessReport
    {
        public NAEDOReportSuccessDTO naedoReport;

        public SuccessReport(XElement item)
        {
            naedoReport = new NAEDOReportSuccessDTO();

            naedoReport.ReportSuccess.TransactionId = Int64.Parse(item.Element("TranID").Value);
            naedoReport.ReportSuccess.TransactionTypeId = int.Parse(item.Element("TTypeID").Value);
            naedoReport.ReportSuccess.ProcessMerchant = item.Element("process_merchant").Value;
            naedoReport.ReportSuccess.ClientRef1 = item.Element("ClientRef1").Value;
            naedoReport.ReportSuccess.ClientRef2 = item.Element("ClientRef2").Value;
            naedoReport.ReportSuccess.ActionDT = Convert.ToDateTime(item.Element("ActionDate").Value);
            naedoReport.ReplyDT = string.IsNullOrEmpty(item.Element("ReplyDate").Value) ? (DateTime?)null : Convert.ToDateTime(item.Element("ReplyDate").Value, CultureInfo.InvariantCulture);
            naedoReport.NumInstallments = item.Element("NumInstallments").Value;
            naedoReport.HomingAccountName = item.Element("HomingAccName").Value;
            naedoReport.Amount = decimal.Parse(item.Element("Amount").Value, CultureInfo.InvariantCulture);
            naedoReport.CCardNum = item.Element("CCardNumber").Value;
        }


    }
}

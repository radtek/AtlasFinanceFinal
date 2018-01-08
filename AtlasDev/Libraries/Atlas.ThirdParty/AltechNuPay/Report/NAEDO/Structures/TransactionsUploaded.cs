using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Atlas.Domain.DTO;
using System.Globalization;

namespace Atlas.ThirdParty.AltechNuPay.Report.NAEDO
{
    public class TransactionsUploaded
    {
        #region public Fields

        public NAEDOReportTransactionUploadedDTO naedoReport;

        #endregion

        public TransactionsUploaded(XElement item)
        {
            naedoReport = new NAEDOReportTransactionUploadedDTO();
            naedoReport.ReportTransactionUploaded.TransactionId = Int64.Parse(item.Element("TranID").Value);
            naedoReport.ReportTransactionUploaded.TransactionTypeId = int.Parse(item.Element("TTypeID").Value);
            naedoReport.ReportTransactionUploaded.ProcessMerchant = item.Element("process_merchant").Value;
            naedoReport.ReportTransactionUploaded.ClientRef1 = item.Element("ClientRef1").Value;
            naedoReport.ReportTransactionUploaded.ClientRef2 = item.Element("ClientRef2").Value;
            naedoReport.ReportTransactionUploaded.ActionDT = DateTime.ParseExact(item.Element("ActionDate").Value, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
            naedoReport.ReplyDT = string.IsNullOrEmpty(item.Element("ReplyDate").Value) ? (DateTime?)null : DateTime.ParseExact(item.Element("ReplyDate").Value, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
            naedoReport.HomingAccountName = item.Element("HomingAccName").Value;
            naedoReport.HomingAccountNum = item.Element("HomingAccNo").Value;
            naedoReport.Amount = decimal.Parse(item.Element("Amount").Value, CultureInfo.InvariantCulture);
            naedoReport.HomingBranch = item.Element("HomingBranch").Value;
            naedoReport.RCode = item.Element("RCode").Value;
            naedoReport.QCode = item.Element("QCode").Value;
            naedoReport.CCardNum = item.Element("CCardNumber").Value;
        }
    }
}

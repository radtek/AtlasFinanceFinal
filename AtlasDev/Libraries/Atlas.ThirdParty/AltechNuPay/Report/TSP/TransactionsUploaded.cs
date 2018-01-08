using Atlas.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Atlas.ThirdParty.AltechNuPay.Report.TSP
{
    public class TransactionsUploaded
    {
        #region public Fields

        public TSPReportTransactionUploadedDTO naedoReport;

        #endregion

        public TransactionsUploaded(XElement item)
        {
            naedoReport = new TSPReportTransactionUploadedDTO();

            naedoReport.TransactionId = Int64.Parse(item.Element("TranID").Value);
            naedoReport.TransactionType = item.Element("tran_type").Value;
            naedoReport.ProcessMerchant = item.Element("process_merchant").Value;
            naedoReport.ClientRef1 = item.Element("ClientRef1").Value;
            naedoReport.ClientRef2 = item.Element("ClientRef2").Value;
            naedoReport.ActionDate = DateTime.ParseExact(item.Element("ActionDate").Value, "yyyyMMdd", CultureInfo.InvariantCulture);
            naedoReport.ReplyDate = string.IsNullOrEmpty(item.Element("ReplyDate").Value) ? (DateTime?)null : DateTime.ParseExact(item.Element("ReplyDate").Value, "yyyyMMdd", CultureInfo.InvariantCulture);
            naedoReport.AccountName = item.Element("account_name").Value;
            naedoReport.Value = Convert.ToDecimal(item.Element("value").Value, CultureInfo.InvariantCulture);
            naedoReport.BranchCode = item.Element("branchcode").Value;
            naedoReport.RCode = item.Element("RCode").Value;
            naedoReport.QCode = item.Element("QCode").Value;
            naedoReport.AccountNumber = item.Element("account_no").Value;
        }
    }
}

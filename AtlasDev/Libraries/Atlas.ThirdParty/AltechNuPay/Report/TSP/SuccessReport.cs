using Atlas.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Atlas.ThirdParty.AltechNuPay.Report.TSP
{
    public class SuccessReport
    {
        #region public Fields

        public TSPReportSuccessDTO naedoReport;

        #endregion

        public SuccessReport(XElement item)
        {
            naedoReport = new TSPReportSuccessDTO();

            naedoReport.TransactionId = Int64.Parse(item.Element("TranID").Value);
            naedoReport.TransactionType = item.Element("tran_type").Value;
            naedoReport.ProcessMerchant = item.Element("process_merchant").Value;
            naedoReport.ClientRef1 = item.Element("ClientRef1").Value;
            naedoReport.ClientRef2 = item.Element("ClientRef2").Value;
            naedoReport.ActionDate = DateTime.ParseExact(item.Element("ActionDate").Value, "yyyyMMdd", CultureInfo.InvariantCulture);
            naedoReport.ResultDate = string.IsNullOrEmpty(item.Element("ResultDate").Value) ? (DateTime?)null : DateTime.ParseExact(item.Element("ResultDate").Value, "yyyyMMdd", CultureInfo.InvariantCulture);
            naedoReport.NumInstallments = item.Element("NumInstallments").Value;
            naedoReport.AccountName = item.Element("HomingAccName").Value;
            naedoReport.Value = Convert.ToDecimal(item.Element("value").Value, CultureInfo.InvariantCulture);
        }
    }
}

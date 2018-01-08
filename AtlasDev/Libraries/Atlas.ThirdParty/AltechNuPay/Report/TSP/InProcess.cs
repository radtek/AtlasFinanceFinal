using Atlas.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Atlas.ThirdParty.AltechNuPay.Report.TSP
{
    public class InProcess
    {
        #region public Fields

        public TSPReportInProcessDTO naedoReport;

        #endregion

        public InProcess(XElement item)
        {
            naedoReport = new TSPReportInProcessDTO();

            naedoReport.TransactionId = Int64.Parse(item.Element("TranID").Value);
            naedoReport.TransactionType = item.Element("tran_type").Value;
            naedoReport.ProcessMerchant = item.Element("process_merchant").Value;
            naedoReport.ClientRef1 = item.Element("ClientRef1").Value;
            naedoReport.ClientRef2 = item.Element("ClientRef2").Value;
            naedoReport.ActionDate = DateTime.ParseExact(item.Element("ActionDate").Value, "yyyyMMdd", CultureInfo.InvariantCulture);
            naedoReport.NumInstallments = item.Element("NumInstallments").Value;
            naedoReport.AccountName = item.Element("HomingAccName").Value;
            naedoReport.Value = Convert.ToDecimal(item.Element("value").Value, CultureInfo.InvariantCulture);
        }
    }
}

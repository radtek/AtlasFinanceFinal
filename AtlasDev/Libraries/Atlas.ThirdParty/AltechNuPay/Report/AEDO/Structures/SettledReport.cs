using Atlas.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Atlas.ThirdParty.AltechNuPay.Report.AEDO
{
    public class SettledReport
    {
        #region public Fields

        public AEDOReportSettledDTO aedoReport;

        #endregion

        public SettledReport(XElement item)
        {
            aedoReport = new AEDOReportSettledDTO();

            aedoReport.ReportSettled.TransactionId = Int64.Parse(item.Element("transaction_id").Value);
            aedoReport.ReportSettled.SettlementDT = DateTime.ParseExact(item.Element("settlement_date").Value, "yyyyMMdd", CultureInfo.InvariantCulture); 
            aedoReport.TermId = int.Parse(item.Element("term_id").Value);
            aedoReport.MerchantNum = item.Element("merchant_no").Value;
            aedoReport.Pan = item.Element("pan").Value;
            aedoReport.ReportSettled.Instalment = int.Parse(item.Element("instalment").Value);
            aedoReport.TransmitDT = DateTime.ParseExact(item.Element("transmit_time").Value, "yyyyMMdd", CultureInfo.InvariantCulture);
            aedoReport.ContractNum = item.Element("contract_no").Value;
            aedoReport.TransactionAmount = FileStructureHelper.ConvertStringToDecimalPoint(item.Element("tran_amount").Value);
            aedoReport.ContractRef = item.Element("contract_ref").Value;
            aedoReport.IdNumber = item.Element("id_number").Value;
        }
    }
}

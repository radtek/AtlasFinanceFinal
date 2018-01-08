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
    public class UnmatchedReport
    {
        #region public Fields

        public AEDOReportUnmatchedDTO aedoReport;

        #endregion

        public UnmatchedReport(XElement item)
        {
            aedoReport = new AEDOReportUnmatchedDTO();

            aedoReport.SettlementDT = DateTime.ParseExact(item.Element("settlement_date").Value, "yyyyMMdd h:mm:ss tt", CultureInfo.InvariantCulture);
            aedoReport.TermId = int.Parse(item.Element("term_id").Value);
            aedoReport.MerchantNum = item.Element("merchant_no").Value;
            aedoReport.Pan = item.Element("pan").Value;
            aedoReport.TransactionAmount = FileStructureHelper.ConvertStringToDecimalPoint(item.Element("tran_amount").Value);
            aedoReport.ContractRef = item.Element("contract_ref").Value;
        }
    }
}

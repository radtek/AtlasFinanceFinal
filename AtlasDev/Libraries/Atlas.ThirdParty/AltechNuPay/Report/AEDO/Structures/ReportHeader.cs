using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Atlas.Domain.DTO;
using System.Globalization;

namespace Atlas.ThirdParty.AltechNuPay.Report.AEDO
{
    public class ReportHeader
    {
        public AEDOReportBatchDTO AEDOReportBatch;

        public ReportHeader(XElement item)
        {
            AEDOReportBatch = new AEDOReportBatchDTO();

            AEDOReportBatch.MerchantNum = item.Element("merchant_number").Value;
            AEDOReportBatch.ReportType = int.Parse(item.Element("report_type").Value);
            AEDOReportBatch.ReportFromDT = DateTime.ParseExact(item.Element("date_from").Value, "yyyyMMdd", CultureInfo.InvariantCulture);
            AEDOReportBatch.ReportToDT = DateTime.ParseExact(item.Element("date_to").Value, "yyyyMMdd", CultureInfo.InvariantCulture);
            AEDOReportBatch.TokenNum = Int64.Parse(item.Element("token_number").Value);
            AEDOReportBatch.BlockNum = Int64.Parse(item.Element("block_number").Value);
            AEDOReportBatch.ReportGenerationDT = string.IsNullOrEmpty(item.Element("datetime_stamp").Value) ? (DateTime?)null : DateTime.ParseExact(item.Element("datetime_stamp").Value, "yyyy/MM/dd hh:mm:ss tt", CultureInfo.InvariantCulture);
            AEDOReportBatch.CreatedDT = DateTime.Now;
        }
    }
}

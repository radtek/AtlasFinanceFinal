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
    public class ReportHeader
    {
        public NAEDOReportBatchDTO NAEDOReportBatch;

        public ReportHeader(XElement item)
        {
            NAEDOReportBatch = new NAEDOReportBatchDTO();

            NAEDOReportBatch.MerchantNum = item.Element("merchant_number").Value;
            NAEDOReportBatch.ServiceType = item.Element("service_type").Value;
            NAEDOReportBatch.ReportType = int.Parse(item.Element("report_type").Value);
            NAEDOReportBatch.ReportFromDT = DateTime.ParseExact(item.Element("date_from").Value, "yyyyMMdd", CultureInfo.InvariantCulture);
            NAEDOReportBatch.ReportToDT = DateTime.ParseExact(item.Element("date_to").Value, "yyyyMMdd", CultureInfo.InvariantCulture);
            NAEDOReportBatch.TokenNum = Int64.Parse(item.Element("token_number").Value);
            NAEDOReportBatch.BlockNum = Int64.Parse(item.Element("block_number").Value);
            NAEDOReportBatch.ReportGenerationDT = string.IsNullOrEmpty(item.Element("datetime_stamp").Value) ? (DateTime?)null : DateTime.ParseExact(item.Element("datetime_stamp").Value, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
            NAEDOReportBatch.CreateDT = DateTime.Now;
        }
    }
}

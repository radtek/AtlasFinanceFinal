using System;
using System.Globalization;
using System.Xml.Linq;

using Atlas.Domain.DTO;


namespace SchedulerServer.AltechNuPay.Report.NAEDO.Structures
{
  public class ReportHeader
  {
    public NAEDOReportBatchDTO NAEDOReportBatch;

    public ReportHeader(XElement item)
    {
      NAEDOReportBatch = new NAEDOReportBatchDTO()
      {
        MerchantNum = item.Element("merchant_number").Value,
        ServiceType = item.Element("service_type").Value,
        ReportType = int.Parse(item.Element("report_type").Value),
        ReportFromDT = DateTime.ParseExact(item.Element("date_from").Value, "yyyyMMdd", CultureInfo.InvariantCulture),
        ReportToDT = DateTime.ParseExact(item.Element("date_to").Value, "yyyyMMdd", CultureInfo.InvariantCulture),
        TokenNum = Int64.Parse(item.Element("token_number").Value),
        BlockNum = Int64.Parse(item.Element("block_number").Value),
        ReportGenerationDT = string.IsNullOrEmpty(item.Element("datetime_stamp").Value) ? 
          (DateTime?)null : DateTime.ParseExact(item.Element("datetime_stamp").Value, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
        CreateDT = DateTime.Now
      };
    }
  }
}

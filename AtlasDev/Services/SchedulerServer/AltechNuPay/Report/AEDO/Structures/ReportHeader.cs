using System;
using System.Globalization;
using System.Xml.Linq;

using Atlas.Domain.DTO;


namespace SchedulerServer.AltechNuPay.Report.AEDO.Structures
{
  public class ReportHeader
  {
    public AEDOReportBatchDTO AEDOReportBatch;

    public ReportHeader(XElement item)
    {
      AEDOReportBatch = new AEDOReportBatchDTO()
      {
        MerchantNum = item.Element("merchant_number").Value,
        ReportType = int.Parse(item.Element("report_type").Value),
        ReportFromDT = DateTime.ParseExact(item.Element("date_from").Value, "yyyyMMdd", CultureInfo.InvariantCulture),
        ReportToDT = DateTime.ParseExact(item.Element("date_to").Value, "yyyyMMdd", CultureInfo.InvariantCulture),
        TokenNum = Int64.Parse(item.Element("token_number").Value),
        BlockNum = Int64.Parse(item.Element("block_number").Value),
        ReportGenerationDT = string.IsNullOrEmpty(item.Element("datetime_stamp").Value) ?
          (DateTime?)null :
          DateTime.ParseExact(item.Element("datetime_stamp").Value, "yyyy/MM/dd hh:mm:ss tt", CultureInfo.InvariantCulture),
        CreatedDT = DateTime.Now
      };
    }
  }
}

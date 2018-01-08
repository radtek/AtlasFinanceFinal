using System;
using System.Globalization;
using System.Xml.Linq;

using Atlas.Domain.DTO;


namespace SchedulerServer.AltechNuPay.Report.AEDO.Structures
{
  public class UnmatchedReport
  {
    #region public Fields

    public AEDOReportUnmatchedDTO aedoReport;

    #endregion

    public UnmatchedReport(XElement item)
    {
      aedoReport = new AEDOReportUnmatchedDTO()
      {
        SettlementDT = DateTime.ParseExact(item.Element("settlement_date").Value, "yyyyMMdd h:mm:ss tt", CultureInfo.InvariantCulture),
        TermId = int.Parse(item.Element("term_id").Value),
        MerchantNum = item.Element("merchant_no").Value,
        Pan = item.Element("pan").Value,
        TransactionAmount = FileStructureHelper.ConvertFromCents(item.Element("tran_amount").Value),
        ContractRef = item.Element("contract_ref").Value
      };
    }
  }
}

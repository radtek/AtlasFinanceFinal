using System;
using System.Globalization;
using System.Xml.Linq;

using Atlas.Domain.DTO;


namespace SchedulerServer.AltechNuPay.Report.TSP
{
  public class TransactionsUploaded
  {
    #region public Fields

    public TSPReportTransactionUploadedDTO naedoReport;

    #endregion

    public TransactionsUploaded(XElement item)
    {
      naedoReport = new TSPReportTransactionUploadedDTO()
      {
        TransactionId = Int64.Parse(item.Element("TranID").Value),
        TransactionType = item.Element("tran_type").Value,
        ProcessMerchant = item.Element("process_merchant").Value,
        ClientRef1 = item.Element("ClientRef1").Value,
        ClientRef2 = item.Element("ClientRef2").Value,
        ActionDate = DateTime.ParseExact(item.Element("ActionDate").Value, "yyyyMMdd", CultureInfo.InvariantCulture),
        ReplyDate = string.IsNullOrEmpty(item.Element("ReplyDate").Value) ?
          (DateTime?)null : DateTime.ParseExact(item.Element("ReplyDate").Value, "yyyyMMdd", CultureInfo.InvariantCulture),
        AccountName = item.Element("account_name").Value,
        Value = Convert.ToDecimal(item.Element("value").Value, CultureInfo.InvariantCulture),
        BranchCode = item.Element("branchcode").Value,
        RCode = item.Element("RCode").Value,
        QCode = item.Element("QCode").Value,
        AccountNumber = item.Element("account_no").Value
      };
    }
  }
}

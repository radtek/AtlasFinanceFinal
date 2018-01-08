using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data;

using Quartz;
using DevExpress.Spreadsheet;
using DevExpress.Xpo;

using Atlas.Domain.Model;
using Atlas.Common.Extensions;
using Atlas.Accounts.Reports.Utils;
using Atlas.Common.Interface;


namespace Atlas.Accounts.Reports.QuartzTasks
{
  [DisallowConcurrentExecution]
  internal class ManualReceipts : IJob
  {
    public ManualReceipts(ILogging log, IConfigSettings config)
    {
      _log = log;
      _config = config;
    }


    public void Execute(IJobExecutionContext context)
    {
      _log.Information("ManualReceipts.Execute starting");
      try
      {
        var recipients = GetManualReceiptRecipients();
        if (recipients.Any())
        {
          // If run before 18:00 and the 1st of the month, run for full last month period
          var start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).Date;
          if (DateTime.Today.Day == 1 && DateTime.Now.Hour < 18)
          {
            start = start.Subtract(TimeSpan.FromDays(1));
            start = new DateTime(start.Year, start.Month, 1).Date;
          }
          var end = new DateTime(start.Year, start.Month, DateTime.DaysInMonth(start.Year, start.Month), 23, 59, 59);

          var branchList = GetBranchList();

          var dataAedoNaedo = GetData(Properties.Resources.SqlManReceiptsAedoNaedo, start, end, branchList).OrderBy(s => s.brnum).ThenBy(s => s.trdate).ToList();
          var dataBank = GetData(Properties.Resources.SqlManReceiptsBank, start, end, branchList).OrderBy(s => s.brnum).ThenBy(s => s.trdate).ToList();
          var dataHub = GetData(Properties.Resources.SqlManReceiptsHub, start, end, branchList).OrderBy(s => s.brnum).ThenBy(s => s.trdate).ToList();

          if (dataAedoNaedo.Any() || dataBank.Any() || dataHub.Any())
          {
            _log.Information("Sending to: {@Recipients}", recipients);

            var tempFile = CreateSpreadsheetFile(start, end, dataAedoNaedo, dataBank, dataHub);

            EMailUtils.SendEMail(_log, "Manual receipts month-to-date report", "Please see attached Excel report", recipients, tempFile);
            File.Delete(tempFile);
          }
        }
        _log.Information("ManualReceipts.Execute completed");
      }
      catch (Exception err)
      {
        _log.Error(err, "ManualReceipts.Execute");
      }
    }


    #region Excel

    private string CreateSpreadsheetFile(DateTime start, DateTime end,
      List<ReceiptRow> dataAedoNeado, List<ReceiptRow> dataBank, List<ReceiptRow> dataHub)
    {
      var tempFile = Path.Combine(Path.GetTempPath(), string.Format("{0}.xlsx", Guid.NewGuid().ToString("N")));
      using (var workbook = new Workbook())
      {
        workbook.CreateNewDocument();
        workbook.Options.Culture = System.Globalization.CultureInfo.InvariantCulture;

        workbook.BeginUpdate();

        var currSheet = workbook.Worksheets[0];
        currSheet.Name = "Aedo_Naedo";
        AddSheet(currSheet, dataAedoNeado);

        currSheet = workbook.Worksheets.Add("Bank");
        AddSheet(currSheet, dataBank);

        currSheet = workbook.Worksheets.Add("Hub");
        AddSheet(currSheet, dataHub);

        workbook.Worksheets.ActiveWorksheet = workbook.Worksheets[0];

        workbook.EndUpdate();
        workbook.SaveDocument(tempFile);
      }

      return tempFile;
    }


    private void AddSheet(Worksheet currSheet, List<ReceiptRow> data)
    {
      var rowIdx = 0;

      AddHeader(currSheet, rowIdx++);
      foreach (var row in data)
      {
        AddRow(currSheet, rowIdx++, row);
      }

      rowIdx++;
      try
      {
        currSheet.Cells[rowIdx, 10].Formula = $"=SUBTOTAL(9, {currSheet[1, 10].GetReferenceA1()}:{currSheet[rowIdx - 2, 10].GetReferenceA1()})";
      }
      catch (Exception err)
      {
        // WHY error only on ther server?
        _log.Error(err, "AddSheet()");
      }

      currSheet.Cells[rowIdx, 10].Font.Bold = true;
      currSheet.Cells[rowIdx, 10].Borders.BottomBorder.LineStyle = BorderLineStyle.Thin;

      // Do formats after formulas...
      currSheet.Columns[7].NumberFormat = "YYYY-MM-DD";
      currSheet.Columns[8].NumberFormat = "YYYY-MM-DD";
      currSheet.Columns[9].NumberFormat = "HH:MM:SS";
      currSheet.Columns[10].NumberFormat = ExcelFmtCurrency;

      // Enable filtering for the specified cell range.           
      currSheet.AutoFilter.Apply(currSheet[string.Format("A1:V{0}", data.Count + 1)]);

      currSheet.Columns.AutoFit(0, 22);
    }


    private static void AddHeader(Worksheet currSheet, int rowIdx)
    {
      var col = 0;
      currSheet.Cells[rowIdx, col++].Value = "Br";
      currSheet.Cells[rowIdx, col++].Value = "Desc";
      currSheet.Cells[rowIdx, col++].Value = "Client";
      currSheet.Cells[rowIdx, col++].Value = "Loan";
      currSheet.Cells[rowIdx, col++].Value = "Region";
      currSheet.Cells[rowIdx, col++].Value = "Term";
      currSheet.Cells[rowIdx, col++].Value = "Qty";
      currSheet.Cells[rowIdx, col++].Value = "Loan date";
      currSheet.Cells[rowIdx, col++].Value = "Trans date";
      currSheet.Cells[rowIdx, col++].Value = "Trans time";
      currSheet.Cells[rowIdx, col++].Value = "Amount";
      currSheet.Cells[rowIdx, col++].Value = "GL";
      currSheet.Cells[rowIdx, col++].Value = "B-Acct";
      currSheet.Cells[rowIdx, col++].Value = "TRType";
      currSheet.Cells[rowIdx, col++].Value = "TRStat";
      currSheet.Cells[rowIdx, col++].Value = "Order";
      currSheet.Cells[rowIdx, col++].Value = "SeqNo";
      currSheet.Cells[rowIdx, col++].Value = "ReceiptNo";
      currSheet.Cells[rowIdx, col++].Value = "Legal";
      currSheet.Cells[rowIdx, col++].Value = "Oper";
      currSheet.Cells[rowIdx, col++].Value = "Oper name";
      currSheet.Cells[rowIdx, col++].Value = "Station";

      var endCol = currSheet.Cells[0, col].GetReferenceA1();
      currSheet.Range[string.Format("A1:{0}", endCol)].Font.Bold = true;
      currSheet.Range[string.Format("A1:{0}", endCol)].Borders.BottomBorder.LineStyle = BorderLineStyle.Thin;
      currSheet.FreezeRows(0);
      //var x = currSheet.Cells[rowIdx, col].GetReferenceA1();
    }


    private static void AddRow(Worksheet currSheet, int rowIdx, ReceiptRow row)
    {
      var col = 0;
      currSheet.Cells[rowIdx, col++].Value = row.brnum;
      currSheet.Cells[rowIdx, col++].Value = row.brdesc;
      currSheet.Cells[rowIdx, col++].Value = row.client;
      currSheet.Cells[rowIdx, col++].Value = row.loan;
      currSheet.Cells[rowIdx, col++].Value = row.regiondesc;
      currSheet.Cells[rowIdx, col++].Value = row.loanmeth;
      currSheet.Cells[rowIdx, col++].Value = (double)row.payno;
      currSheet.Cells[rowIdx, col++].Value = row.loandate;
      currSheet.Cells[rowIdx, col++].Value = row.trdate;
      currSheet.Cells[rowIdx, col++].Value = row.usertime;
      currSheet.Cells[rowIdx, col++].Value = (double)row.tramount;
      currSheet.Cells[rowIdx, col++].Value = row.glbank;
      currSheet.Cells[rowIdx, col++].Value = row.bank_acct;

      switch (row.trtype)
      {
        case "F":
          currSheet.Cells[rowIdx, col++].Value = "Refund";
          break;

        case "G":
          currSheet.Cells[rowIdx, col++].Value = "Refund";
          break;

        case "P":
          currSheet.Cells[rowIdx, col++].Value = "Payment";
          break;

        default:
          currSheet.Cells[rowIdx, col++].Value = row.trtype;
          break;
      }

      currSheet.Cells[rowIdx, col++].Value = row.trstat;
      currSheet.Cells[rowIdx, col++].Value = (int)row.order;
      currSheet.Cells[rowIdx, col++].Value = (int)row.seqno;
      currSheet.Cells[rowIdx, col++].Value = (int)row.receiptno;
      currSheet.Cells[rowIdx, col].Value = row.legal == "Y" ? 1 : -1;
      currSheet.Cells[rowIdx, col++].NumberFormat = "\"Yes\";\"No\";\"\"";

      currSheet.Cells[rowIdx, col++].Value = row.oper;
      currSheet.Cells[rowIdx, col++].Value = row.user_name;
      currSheet.Cells[rowIdx, col++].Value = row.station;
    }

    #endregion


    //
    // GLBANK : 'NAD' or NUP' - AEDO OR NAEDO
    // TRTYPE : 'P'- payment/receipt
    //          'G'- refund
    // NPAYTRANID is blank when manual receipt
    // TRAMOUNT is negative when reversed
    //


    private List<ReceiptRow> GetData(string sql, DateTime startDate, DateTime endDate, Dictionary<string, string> branchList)
    {
      var data = new List<ReceiptRow>();
      var connStr = _config.GetAssConnectionString();
      var errs = 0;
      var done = false;

      while (errs < 5 && !done)
      {
        try
        {
          using (var conn = new Npgsql.NpgsqlConnection(connStr))
          {
            conn.Open();

            using (var cmd = conn.CreateCommand())
            {
              //
              // GLBANK : 'NAD' or NUP' - AEDO OR NAEDO
              // TRTYPE : 'P'- payment/receipt
              //          'G'- refund
              // NPAYTRANID is blank when manual receipt
              // TRAMOUNT is negative when reversed
              //
              cmd.CommandTimeout = 600;
              cmd.CommandText = sql;
              cmd.CommandType = System.Data.CommandType.Text;
              cmd.Parameters.AddWithValue("startDate", NpgsqlTypes.NpgsqlDbType.Date, startDate);
              cmd.Parameters.AddWithValue("endDate", endDate);

              using (var rdr = cmd.ExecuteReader())
              {
                done = true;

                while (rdr.Read())
                {
                  data.Add(ReadRow(rdr, branchList));
                }
              }
            }
          }
        }
        catch (Exception err)
        {
          _log.Error(err, "GetData()");
          errs++;
        }
      }

      return data;
    }


    private ReceiptRow ReadRow(IDataReader rdr, Dictionary<string, string> branchList)
    {
      var time = DateTime.MinValue;
      try
      {
        time = DateTime.ParseExact(rdr.GetString(4), "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
      }
      catch (Exception err)
      {
        _log.Error(err, "Invalid time in data: {Time}", rdr.GetString(4));
      }

      return new ReceiptRow
      {
        brnum = rdr.GetString(0),
        brdesc = (branchList.ContainsKey(rdr.GetString(0))) ? branchList[rdr.GetString(0)] : string.Format("Not found: {0}", rdr.GetString(0)),
        loan = rdr.GetString(1),
        client = rdr.GetString(2),
        trdate = rdr.GetDateTime(3),
        usertime = time,
        tramount = rdr.GetDecimal(5),
        glbank = rdr.GetString(6),
        trtype = rdr.GetString(7),
        trstat = rdr.IsDBNull(8) ? string.Empty : rdr.GetString(8),
        order = rdr.IsDBNull(9) ? 0 : rdr.GetDecimal(9),
        seqno = rdr.GetDecimal(10),
        receiptno = rdr.GetDecimal(11),
        legal = rdr.GetString(12),
        regiondesc = rdr.GetString(13),
        loanmeth = rdr.GetString(14),
        payno = rdr.GetDecimal(15),
        loandate = rdr.GetDateTime(16),
        oper = rdr.GetString(17),
        user_name = rdr.GetString(18),
        station = rdr.IsDBNull(19) ? string.Empty : rdr.GetString(19),
        bank_acct = rdr.GetString(20)
      };
    }


    private static List<string> GetManualReceiptRecipients()
    {
      using (var uow = new UnitOfWork())
      {
        return uow.Query<Config>()
          .Where(s => s.DataType == Enumerators.Config.Alerting.ManualReceipts.ToInt())
          .Select(s => s.DataValue)
          .ToList();
      }
    }


    private Dictionary<string, string> GetBranchList()
    {
      var result = new Dictionary<string, string>();
      var connStr = _config.GetAssConnectionString();
      using (var conn = new Npgsql.NpgsqlConnection(connStr))
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = "SELECT brnum, sysname FROM company.asbranch";
          using (var rdr = cmd.ExecuteReader())
          {
            while (rdr.Read())
            {
              if (!result.ContainsKey(rdr.GetString(0)))
              {
                result.Add(rdr.GetString(0), rdr.GetString(1));
              }
            }
          }
        }
      }

      return result;
    }


    private const string ExcelFmtCurrency = "#,##0.00;-#,##0.00";

    private readonly ILogging _log;
    private readonly IConfigSettings _config;

    private class ReceiptRow
    {
      public string brnum { get; set; }
      public string brdesc { get; internal set; }
      public string loan { get; set; }
      public string client { get; set; }
      public DateTime trdate { get; set; }
      public DateTime usertime { get; set; }
      public Decimal tramount { get; set; }
      public string glbank { get; set; }
      public string trtype { get; set; }
      public string trstat { get; set; }
      public decimal order { get; set; }
      public decimal seqno { get; set; }
      public decimal receiptno { get; set; }
      public string legal { get; set; }
      public string regiondesc { get; set; }
      public string loanmeth { get; set; }
      public decimal payno { get; set; }
      public DateTime loandate { get; set; }
      public string oper { get; set; }
      public string user_name { get; set; }
      public string station { get; set; }
      public string bank_acct { get; set; }
    }

  }
}

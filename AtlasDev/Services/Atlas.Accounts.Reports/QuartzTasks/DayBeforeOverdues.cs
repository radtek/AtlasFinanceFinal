using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

using Quartz;
using DevExpress.Xpo;
using DevExpress.Spreadsheet;
using Dapper;

using Atlas.Domain.Model;
using Atlas.Common.Extensions;
using Atlas.Accounts.Reports.Utils;
using Atlas.Common.Interface;


namespace Atlas.Accounts.Reports.QuartzTasks
{
  internal class DayBeforeOverdues : IJob
  {
    public DayBeforeOverdues(ILogging log, IConfigSettings config)
    {
      _log = log;
      _config = config;
    }


    public void Execute(IJobExecutionContext context)
    {
      _log.Information("DayBeforeOverdues.Execute starting");

      try
      {
        var recipients = GetRecipients();
        if (recipients.Any())
        {
          var longReport = (DateTime.Now.Hour < 7 && DateTime.Today.DayOfWeek == DayOfWeek.Wednesday);
          if (longReport)
          {
            var rows = GetData(true);
            _log.Information("Long overdue report- Got {Rows} rows data", rows.Count);

            if (rows.Any())
            {
              var tempFile = CreateSpreadsheet(rows);
              EMailUtils.SendEMail(_log, "Overdue report (33-3 days past)", "Please see attached Excel report", recipients, tempFile);
              File.Delete(tempFile);
            }
          }

          var rows3Days = GetData(false);
          _log.Information("Overdue report- Got {Rows} rows data", rows3Days.Count);

          if (rows3Days.Any())
          {
            var tempFile = CreateSpreadsheet(rows3Days);
            EMailUtils.SendEMail(_log, "Overdue report (2 days past)", "Please see attached Excel report", recipients, tempFile);
            File.Delete(tempFile);
          }
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "Execute()");
      }

      _log.Information("DayBeforeOverdues.Execute completed");
    }


    /// <summary>
    /// Gets overdue data
    /// </summary>
    /// <returns></returns>
    private List<Overdue> GetData(bool longReport)
    {
      // First early morning run to be 34 to 4 days back, else 3 days back
      var dateWhere = longReport ?
        "tr.trdate BETWEEN (CURRENT_DATE - 33) AND (CURRENT_DATE - 3)" :
        "tr.trdate = CURRENT_DATE - 2";

      var sql =
          "select tr.brnum as branchnum, br.sysname as branchname, br.regioncode as regioncode, tr.client, tr.loan, " +
          "ln.loanmeth, ln.payno_orig as LoanTerm, ln.nctrantype as TranType, tr.\"order\" as InstalNum,  " +
          "tr.trdate as DueDate, tr.tramount as Amount " +
          //"  case when ln.nctrantype IN ('VAP') then tr.tramount Else 0 END as VAPOwing, " +
          //"  case when ln.nctrantype IN('USE', 'N/A') then tr.tramount Else 0 END as LOANOwing, " +
          //"  case when ln.nctrantype IN('USE', 'N/A', 'VAP') then 0 Else tr.tramount END as SALEOwing " +
          "from company.trans tr, company.loans ln, company.asbranch br " +
          $"where tr.trtype = 'R' and CHAR_LENGTH(TRIM(BOTH ' ' FROM COALESCE(tr.trstat, ''))) = 0 and {dateWhere} " +
          "  AND ln.nctrantype IN ('USE', 'N/A', 'VAP') " +
          //" --and NULLIF(tr.itm1_sp_ex, 0) IS NULL and NULLIF(tr.itm2_sp_ex, 0) IS NULL " +
          "  and COALESCE(tr.tramount, 0) > 0 " +
          "  and tr.brnum = ln.brnum and tr.client = ln.client and tr.loan = ln.loan " +
          "  and br.brnum = tr.brnum " +
          "  order by tr.brnum, tr.client, tr.loan";

      using (var conn = new Npgsql.NpgsqlConnection(_config.GetAssConnectionString()))
      {
        conn.Open();
        return conn.Query<Overdue>(sql: sql, commandTimeout: 500).ToList();
      }
    }


    private static string CreateSpreadsheet(List<Overdue> rows)
    {
      if (rows == null || !rows.Any())
      {
        return null;
      }

      var filename = Path.Combine(Path.GetTempPath(), string.Format("{0}.xlsx", Guid.NewGuid().ToString("N")));
      using (var workbook = new Workbook())
      {
        workbook.BeginUpdate();

        var sheet = workbook.Worksheets[0];
        var currRowNo = 0;
        AddHeader(currRowNo++, sheet);
        var excelRow = MapOverdue(rows[0]);
        var haveData = true;
        foreach (var row in rows.Skip(1))
        {
          var loan = int.Parse(row.Loan, NumberStyles.Integer, CultureInfo.InvariantCulture);
          var client = int.Parse(row.Client, NumberStyles.Integer, CultureInfo.InvariantCulture);

          // A VAP row for client
          if (excelRow.BranchNum == row.BranchNum && excelRow.Client == client &&
            (row.TranType.Trim() == "VAP" && (excelRow.TransType.Trim() == "N/A" || excelRow.TransType.Trim() == "USE") && loan == excelRow.Loan + 1))
          {
            excelRow.Amount += row.Amount;
            AddRow(currRowNo++, sheet, excelRow);
            haveData = false;
          }
          else if (row.TranType.Trim() != "VAP")
          {
            if (haveData)
            {
              AddRow(currRowNo++, sheet, excelRow);
            }

            excelRow = MapOverdue(row);
            haveData = true;
          }
        }

        #region Column widths
        var range = sheet.Range[string.Format("A1:J{0}", currRowNo)];
        range.AutoFitColumns();        
        sheet.Columns[10].WidthInCharacters = 10;
        sheet.Columns[11].WidthInCharacters = 30;
        sheet.Columns[12].WidthInCharacters = 10;
        sheet.Columns[13].WidthInCharacters = 30;
        sheet.Columns[14].WidthInCharacters = 30;
        sheet.Columns[15].WidthInCharacters = 10;
        sheet.Columns[16].WidthInCharacters = 30;
        #endregion

        range = sheet.Range[string.Format("A1:Q{0}", currRowNo)];
        sheet.AutoFilter.Apply(range);
        sheet.FreezeRows(0);

        workbook.EndUpdate();
        workbook.SaveDocument(filename, DocumentFormat.Xlsx);
      }

      return filename;
    }


    private static List<string> GetRecipients()
    {
      using (var uow = new UnitOfWork())
      {
        return uow.Query<Config>()
          .Where(s => s.DataType == Enumerators.Config.Alerting.DayBeforeOverdue.ToInt())
          .Select(s => s.DataValue)
          .ToList();
      }
    }


    private static void AddHeader(int currRowNo, Worksheet sheet)
    {
      sheet.Cells[currRowNo, 0].Value = "BNum";
      sheet.Cells[currRowNo, 1].Value = "Branch";
      sheet.Cells[currRowNo, 2].Value = "Region";
      sheet.Cells[currRowNo, 3].Value = "Client";
      sheet.Cells[currRowNo, 4].Value = "Loan";
      sheet.Cells[currRowNo, 5].Value = "Period";
      sheet.Cells[currRowNo, 6].Value = "Term";
      sheet.Cells[currRowNo, 7].Value = "Instalment";
      sheet.Cells[currRowNo, 8].Value = "Due date";
      sheet.Cells[currRowNo, 9].Value = "Amount";
      sheet.Cells[currRowNo, 10].Value = "Date";
      sheet.Cells[currRowNo, 11].Value = "Follow up";
      sheet.Cells[currRowNo, 12].Value = "Date";
      sheet.Cells[currRowNo, 13].Value = "Follow up";
      sheet.Cells[currRowNo, 14].Value = "Number client contacted on";
      sheet.Cells[currRowNo, 15].Value = "Date";
      sheet.Cells[currRowNo, 16].Value = "Follow up";
      sheet.Range["A1:Q1"].Font.Bold = true;
    }

    private static void AddRow(int rowNo, Worksheet sheet, OverdueCalc row)
    {
      sheet.Cells[rowNo, 0].Value = row.BranchNum;
      sheet.Cells[rowNo, 1].Value = row.BranchName;
      sheet.Cells[rowNo, 2].Value = row.Region.Substring(1);
      sheet.Cells[rowNo, 3].Value = row.Client.ToString("00000");
      sheet.Cells[rowNo, 4].Value = row.Loan.ToString("0000");
      sheet.Cells[rowNo, 5].Value = row.Period;
      sheet.Cells[rowNo, 6].Value = row.Term;
      sheet.Cells[rowNo, 7].Value = row.InstalNum;
      sheet.Cells[rowNo, 8].Value = row.DueDate;
      sheet.Cells[rowNo, 9].Value = (float)row.Amount;
    }


    private static OverdueCalc MapOverdue(Overdue row)
    {
      return new OverdueCalc
      {
        Amount = row.Amount,
        BranchName = row.BranchName,
        BranchNum = row.BranchNum,
        Client = int.Parse(row.Client, NumberStyles.Integer, CultureInfo.InvariantCulture),
        Loan = int.Parse(row.Loan, NumberStyles.Integer, CultureInfo.InvariantCulture),
        DueDate = row.DueDate,
        InstalNum = row.InstalNum,
        Period = row.LoanMeth,
        Region = row.RegionCode,
        Term = row.LoanTerm,
        TransType = row.TranType
      };
    }

    class Overdue
    {
      public string BranchNum { get; set; }
      public string BranchName { get; set; }
      public string RegionCode { get; set; }
      public string Client { get; set; }
      public string Loan { get; set; }
      public string LoanMeth { get; set; }
      public int LoanTerm { get; set; }

      public int InstalNum { get; set; }
      public DateTime DueDate { get; set; }
      public decimal Amount { get; set; }
      public string TranType { get; set; }
    }


    class OverdueCalc
    {
      public string BranchNum { get; set; }
      public string BranchName { get; set; }
      public string Region { get; set; }
      public int Client { get; set; }
      public int Loan { get; set; }
      public string Period { get; set; }
      public int Term { get; set; }
      public DateTime DueDate { get; set; }
      public decimal Amount { get; set; }
      public string TransType { get; set; }
      public int InstalNum { get; internal set; }
    }


    private readonly ILogging _log;
    private readonly IConfigSettings _config;
  }
}

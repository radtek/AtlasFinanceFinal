using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Dapper;
using DevExpress.Spreadsheet;
using DevExpress.Xpo;
using System.Diagnostics;
using Quartz;

using Atlas.Common.Interface;
using Atlas.Common.Extensions;
using Atlas.Domain.Model;


namespace Atlas.Accounts.Reports.QuartzTasks
{
  class Receipts85Percent : IJob
  {
    public Receipts85Percent(ILogging log, IConfigSettings config)
    {
      _log = log;
      _config = config;
    }


    public void Execute(IJobExecutionContext context)
    {
      try
      {
        _log.Information("Receipts85Percent starting...");
        var recipients = GetRecipients();
        if (recipients.Any())
        {
          var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
          var endDate = new DateTime(startDate.Year, startDate.Month, DateTime.DaysInMonth(startDate.Year, startDate.Month));

          #region Calculate for periods 
          var amounts = new List<Amounts>();
          var sw = Stopwatch.StartNew();
          GetData(_log, _config, startDate, endDate, Amounts.Periods.TillMonthEnd, amounts);
          _log.Information($"MTD in {sw.ElapsedMilliseconds}ms");

          _log.Information("Calculating today...");
          sw.Restart();
          GetData(_log, _config, DateTime.Today, DateTime.Today, Amounts.Periods.Today, amounts);
          _log.Information($"MTD in {sw.ElapsedMilliseconds}ms");

          if (DateTime.Today.Day > 1)
          {
            sw.Restart();
            _log.Information("Calculating yesterday MTD...");
            GetData(_log, _config, startDate, DateTime.Today.Subtract(TimeSpan.FromDays(1)), Amounts.Periods.TillYesterday, amounts);
            _log.Information($"MTD in {sw.ElapsedMilliseconds}ms");
          }
          #endregion

          var tempFile = ExportToExcel(amounts, startDate, endDate);
          _log.Information("Receipts85Percent.Execute Sending to {@Recipients}", recipients);
          Utils.EMailUtils.SendEMail(_log, "85% Collection Report", "Please find attached the 85% Collection Report", recipients, tempFile);
          File.Delete(tempFile);
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "Receipts85Percent.Execute");
      }

      _log.Information("Receipts85Percent completed");
    }


    private static void GetData(ILogging log, IConfigSettings config, DateTime startDate, DateTime endDate, Amounts.Periods period, List<Amounts> allCalcs)
    {
      log.Information("Connecting...");
      var sw = Stopwatch.StartNew();

      using (var conn = new Npgsql.NpgsqlConnection(config.GetAssConnectionString()))
      {
        conn.Open();
        log.Information("Connected. Getting data....");
        var loaded = conn.Query<RawData>(@"
-- The Instalment value will be the instal_value less the adjustment and will vary each day.
-- The paid amount will be shown separately
SELECT  tr.brnum AS branchnum, br.sysname AS branchname, br.regioncode AS regioncode, 'INSTAL' as rowtype, 
  COUNT(*) AS Qty_Tot, SUM(CASE WHEN ln.nctrantype IN ('VAP') THEN 1 ELSE 0 END) AS Qty_Vap, 
  SUM(tr.tramount) AS instal_value
FROM company.trans tr, company.loans ln, company.asbranch br
WHERE tr.trtype = 'R' " +
$"AND (tr.trdate BETWEEN '{startDate}' AND '{endDate}') " + @"
  AND  tr.""order"" = tr.seqno
  AND COALESCE(tr.tramount, 0) != 0
  AND ln.nctrantype IN ('USE', 'N/A', 'VAP')   
  AND tr.brnum = ln.brnum AND tr.client = ln.client AND tr.loan = ln.loan AND br.brnum = tr.brnum
GROUP BY tr.brnum, br.sysname, br.regioncode

UNION ALL

SELECT tr.brnum AS branchnum, br.sysname AS branchname, br.regioncode AS regioncode, 'ADJUST' as rowtype,
0 AS Qty_Tot, 0 AS Qty_Vap, SUM(CASE WHEN tr.trtype = 'A' THEN tr.tramount * -1 ELSE tr.tramount END) AS instal_value
FROM company.trans tr, company.loans ln, company.asbranch br
WHERE(tr.trtype IN('E', 'C', 'H', 'W', 'A'))
AND EXISTS (select TR1.trdate FROM company.trans TR1
    WHERE TR1.brnum = TR.brnum AND TR1.client = TR.client AND TR1.loan = TR.loan
    AND TR1.""order"" = TR.""order""
    AND TR1.seqno::bigint = TR.""order""::bigint
    AND TR1.trtype = 'R'" +
  $"AND(TR1.trdate BETWEEN '{startDate}' AND '{endDate}')) " + @"
  AND COALESCE(tr.tramount, 0) != 0
  AND ln.nctrantype IN ('USE', 'N/A', 'VAP')
  AND tr.brnum = ln.brnum AND tr.client = ln.client AND tr.loan = ln.loan AND br.brnum = tr.brnum
GROUP BY tr.brnum, br.sysname, br.regioncode

UNION ALL

SELECT tr.brnum AS branchnum, br.sysname AS branchname, br.regioncode AS regioncode, 'PAID' as rowtype,
  COUNT(*) AS Qty_Tot, SUM(CASE WHEN ln.nctrantype IN ('VAP') THEN 1 ELSE 0 END) AS Qty_Vap, SUM(tr.tramount) AS instal_value
FROM company.trans tr, company.loans ln, company.asbranch br
WHERE(tr.trtype = 'P' or tr.trtype = 'F' or tr.trtype = 'G')
AND EXISTS(SELECT TR1.trdate FROM company.trans TR1
    WHERE TR1.brnum = TR.brnum
    AND   TR1.client = TR.client
    AND   TR1.loan = TR.loan
    AND   TR1.""order"" = TR.""order""
    AND   TR1.seqno::bigint = TR.""order""::bigint
    AND   TR1.trtype = 'R'" +
  $"AND(TR1.trdate BETWEEN '{startDate}' AND '{endDate}')) " + @"
  AND   COALESCE(tr.tramount, 0) != 0
  AND   ln.nctrantype IN('USE', 'N/A', 'VAP')
  AND   tr.brnum = ln.brnum
  AND   tr.client = ln.client
  AND   tr.loan = ln.loan
  AND   br.brnum = tr.brnum
  GROUP BY tr.brnum, br.sysname, br.regioncode
  ORDER by branchnum", null, null, true, 500).ToList();

        log.Information("Loaded- {0}ms", sw.ElapsedMilliseconds);
        var calcs = loaded
          .Select(s => new { s.branchnum, s.branchname })
          .GroupBy(s => $"{s.branchnum}")
          .Select(s => new Amounts { Branch = s.First().branchnum, BranchName = s.First().branchname, Period = period })
          .ToList();

        foreach (var row in calcs)
        {
          var instal = loaded.FirstOrDefault(s => s.branchnum == row.Branch && s.rowtype == "INSTAL");
          var adjust = loaded.FirstOrDefault(s => s.branchnum == row.Branch && s.rowtype == "ADJUST");
          var paid = loaded.FirstOrDefault(s => s.branchnum == row.Branch && s.rowtype == "PAID");

          row.Paid_Qty = (paid?.Qty_Tot ?? 0) - (paid?.Qty_Vap ?? 0);
          row.Paid_Val = paid?.instal_value ?? 0;
          row.Sched_Qty = (instal?.Qty_Tot ?? 0) - (instal?.Qty_Vap ?? 0);
          row.Sched_Val = (instal?.instal_value ?? 0) - (adjust?.instal_value ?? 0);
        }

        allCalcs.AddRange(calcs);
      }
    }


    private static string ExportToExcel(List<Amounts> calcs, DateTime start, DateTime end)
    {
      var filename = Path.Combine(Path.GetTempPath(), string.Format("{0}.xlsx", Guid.NewGuid().ToString("N")));
      using (var workbook = new Workbook())
      {
        workbook.BeginUpdate();

        var sheet = workbook.Worksheets[0];
        sheet.Name = "85% MTD collections";

        CreateExcelHeader(sheet, end);
        var data = calcs.OrderBy(s => (s.Period == Amounts.Periods.TillYesterday) ? 0 : 1).ThenBy(s => (s.Sched_Val > 0) ? s.Paid_Val / s.Sched_Val : 0).ToList();
        CreateData(sheet, data, end, new[] { 11 }, 0.85);

        sheet = workbook.Worksheets.Add("85% collections Yesterday");
        CreateExcelHeader(sheet, end);
        data = calcs.OrderBy(s => (s.Period == Amounts.Periods.Today) ? 0 : 1).ThenBy(s => (s.Sched_Val > 0) ? s.Paid_Val / s.Sched_Val : 0).ToList();
        CreateData(sheet, data, end, new[] { 6, 11 }, 0.85);

        workbook.Worksheets.ActiveWorksheet = workbook.Worksheets[0];
        workbook.EndUpdate();
        workbook.SaveDocument(filename, DocumentFormat.Xlsx);
      }

      return filename;
    }


    private static void CreateData(Worksheet sheet, List<Amounts> calcs, DateTime end, IEnumerable<int> colsBelow, double belowVal)
    {
      var row = 5;
      foreach (var calc in calcs.GroupBy(s => s.Branch))
      {
        sheet.Cells[row, 0].Value = calc.Key;
        sheet.Cells[row, 1].Value = calc.First().BranchName;

        var today = calc.FirstOrDefault(s => s.Period == Amounts.Periods.Today);
        if (today != null)
        {
          sheet.Cells[row, 2].Value = today.Sched_Qty;
          sheet.Cells[row, 3].Value = (double)today.Sched_Val;
          sheet.Cells[row, 4].Value = today.Paid_Qty;
          sheet.Cells[row, 5].Value = (double)today.Paid_Val;
          sheet.Cells[row, 6].Value = today.Sched_Val != 0 ? (double)(today.Paid_Val / today.Sched_Val) : 0;
        }

        var yesterday = calc.FirstOrDefault(s => s.Period == Amounts.Periods.TillYesterday);
        if (yesterday != null)
        {
          sheet.Cells[row, 7].Value = yesterday.Sched_Qty;
          sheet.Cells[row, 8].Value = (double)yesterday.Sched_Val;
          sheet.Cells[row, 9].Value = yesterday.Paid_Qty;
          sheet.Cells[row, 10].Value = (double)yesterday.Paid_Val;
          sheet.Cells[row, 11].Value = yesterday.Sched_Val != 0 ? (double)(yesterday.Paid_Val / yesterday.Sched_Val) : 0;
        }

        var monthEnd = calc.First(s => s.Period == Amounts.Periods.TillMonthEnd);
        if (monthEnd != null)
        {
          sheet.Cells[row, 12].Value = monthEnd.Sched_Qty;
          sheet.Cells[row, 13].Value = (double)monthEnd.Sched_Val;
          sheet.Cells[row, 14].Value = monthEnd.Paid_Qty;
          sheet.Cells[row, 15].Value = (double)monthEnd.Paid_Val;
          sheet.Cells[row, 16].Value = monthEnd.Sched_Val != 0 ? (double)(monthEnd.Paid_Val / monthEnd.Sched_Val) : 0;
        }

        foreach (var col in colsBelow)
        {
          if (!sheet.Cells[row, col].Value.IsEmpty && sheet.Cells[row, col].Value.NumericValue < belowVal)
          {
            sheet.Cells[row, col].FillColor = System.Drawing.Color.FromArgb(250, 192, 144);
          }
        }
        row++;
      }

      sheet.Range[$"A6:Q{row}"].Borders.SetAllBorders(System.Drawing.Color.Black, BorderLineStyle.Thin);

      sheet.Range["A4:Q4"].Borders.BottomBorder.LineStyle = BorderLineStyle.Medium;

      row++;
      sheet.Range[$"B5:B{row}"].Borders.RightBorder.LineStyle = BorderLineStyle.Medium;
      sheet.Range[$"G5:G{row}"].Borders.RightBorder.LineStyle = BorderLineStyle.Medium;
      sheet.Range[$"L5:L{row}"].Borders.RightBorder.LineStyle = BorderLineStyle.Medium;
      sheet.Range[$"Q5:Q{row}"].Borders.RightBorder.LineStyle = BorderLineStyle.Medium;

      sheet.Range[$"A{row}:Q{row}"].Borders.BottomBorder.LineStyle = BorderLineStyle.Medium;
      sheet.Range[$"A{row}:Q{row}"].Borders.RightBorder.LineStyle = BorderLineStyle.Medium;

      row++;
      sheet.Cells[row, 1].Value = "Totals";
      sheet.Cells[row, 2].Formula = $"=SUM(C6:C{row - 1})";
      sheet.Cells[row, 3].Formula = $"=SUM(D6:D{row - 1})";
      sheet.Cells[row, 4].Formula = $"=SUM(E6:E{row - 1})";
      sheet.Cells[row, 5].Formula = $"=SUM(F6:F{row - 1})";
      sheet.Cells[row, 6].Formula = $"=SUM(F{row + 1}/D{row + 1})";

      sheet.Cells[row, 7].Formula = $"=SUM(H6:H{row - 1})";
      sheet.Cells[row, 8].Formula = $"=SUM(I6:I{row - 1})";
      sheet.Cells[row, 9].Formula = $"=SUM(J6:J{row - 1})";
      sheet.Cells[row, 10].Formula = $"=SUM(K6:K{row - 1})";
      sheet.Cells[row, 11].Formula = $"=SUM(K{row + 1}/I{row + 1})";

      sheet.Cells[row, 12].Formula = $"=SUM(M6:M{row - 1})";
      sheet.Cells[row, 13].Formula = $"=SUM(N6:N{row - 1})";
      sheet.Cells[row, 14].Formula = $"=SUM(O6:O{row - 1})";
      sheet.Cells[row, 15].Formula = $"=SUM(P6:P{row - 1})";
      sheet.Cells[row, 16].Formula = $"=SUM(P{row + 1}/N{row + 1})";

      sheet.Range[$"B{row + 1}:Q{row + 1}"].Borders.SetAllBorders(System.Drawing.Color.Black, BorderLineStyle.Thin);

      #region Formats
      sheet.Range[$"C6:F{row + 1}"].NumberFormat = ExcelFmtCurrency;
      sheet.Range[$"G6:G{row + 1}"].NumberFormat = ExcelFmtPercent;

      sheet.Range[$"H6:K{row + 1}"].NumberFormat = ExcelFmtCurrency;
      sheet.Range[$"L6:L{row + 1}"].NumberFormat = ExcelFmtPercent;

      sheet.Range[$"M6:P{row + 1}"].NumberFormat = ExcelFmtCurrency;
      sheet.Range[$"Q6:Q{row + 1}"].NumberFormat = ExcelFmtPercent;
      #endregion

      #region Col widths
      var range = sheet.Range[$"A:Q"];
      range.AutoFitColumns();
      for (var i = 2; i < 17; i++)
      {
        sheet.Columns[i].Width = sheet.Columns[i].Width * 1.5;
      }
      sheet.FreezeRows(3);
      #endregion

      sheet.ScrollTo(0, 0);

      #region Print options
      sheet.SetPrintRange(sheet[$"A1:Q{row + 2}"]);

      sheet.ActiveView.Orientation = PageOrientation.Portrait;
      sheet.ActiveView.PaperKind = System.Drawing.Printing.PaperKind.A4;
      sheet.ActiveView.ShowHeadings = true;

      var printOptions = sheet.PrintOptions;
      printOptions.PrintGridlines = true;
      printOptions.FitToPage = true;
      printOptions.FitToHeight = 9999;
      printOptions.FitToWidth = 1;

      // Header at top
      var headerRange = sheet.Range.Parse("$1:$4");
      sheet.DefinedNames.Add("_xlnm.Print_Titles", headerRange.GetReferenceA1(ReferenceElement.IncludeSheetName | ReferenceElement.RowAbsolute));
      #endregion
    }


    private static void CreateExcelHeader(Worksheet sheet, DateTime end)
    {
      var row = 1;
      sheet[row, 2].Value = $"Instalment today {DateTime.Today:dd/MM/yy}";
      sheet[row, 7].Value = $"Instalments Mth-To-Yesterday {DateTime.Today.Subtract(TimeSpan.FromDays(1)):dd/MM/yy}";
      sheet[row, 12].Value = $"Instalments Mth-To-Date {end:dd/MM/yy}";

      row++;
      sheet[row, 2].Value = "Scheduled";
      sheet[row, 4].Value = "Paid";

      sheet[row, 7].Value = $"Scheduled";
      sheet[row, 9].Value = "Paid";

      sheet[row, 12].Value = "Scheduled";
      sheet[row, 14].Value = "Paid";

      row++;
      sheet[row, 0].Value = "Brnum";
      sheet[row, 1].Value = "BrName";
      sheet[row, 2].Value = "Qty";
      sheet[row, 3].Value = "Value";
      sheet[row, 4].Value = "Qty";
      sheet[row, 5].Value = "Value";
      sheet[row, 6].Value = "%";

      sheet[row, 7].Value = "Qty";
      sheet[row, 8].Value = "Value";
      sheet[row, 9].Value = "Qty";
      sheet[row, 10].Value = "Value";
      sheet[row, 11].Value = "%";

      sheet[row, 12].Value = "Qty";
      sheet[row, 13].Value = "Value";
      sheet[row, 14].Value = "Qty";
      sheet[row, 15].Value = "Value";
      sheet[row, 16].Value = "%";

      sheet.MergeCells(sheet.Range["C2:G2"]);
      sheet.MergeCells(sheet.Range["H2:L2"]);
      sheet.MergeCells(sheet.Range["M2:Q2"]);

      sheet.MergeCells(sheet.Range["C3:D3"]);
      sheet.MergeCells(sheet.Range["E3:F3"]);
      sheet.MergeCells(sheet.Range["H3:I3"]);

      sheet.MergeCells(sheet.Range["J3:K3"]);
      sheet.MergeCells(sheet.Range["M3:N3"]);
      sheet.MergeCells(sheet.Range["O3:P3"]);

      sheet.Range["A2:Q4"].Font.Bold = true;
      sheet.Range["A2:Q4"].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
      sheet.Range["A2:Q4"].Borders.SetAllBorders(System.Drawing.Color.Black, BorderLineStyle.Thin);
    }


    private static List<string> GetRecipients()
    {
      using (var uow = new UnitOfWork())
      {
        return uow.Query<Config>()
          .Where(s => s.DataType == Enumerators.Config.Alerting.Receipts85Percent.ToInt())
          .Select(s => s.DataValue)
          .ToList();
      }
    }


    class RawData
    {
      public string branchnum { get; set; }
      public string branchname { get; set; }
      public string regioncode { get; set; }
      public string rowtype { get; set; }
      public long Qty_Tot { get; set; }
      public long Qty_Vap { get; set; }
      public decimal instal_value { get; set; }
    }


    private const string ExcelFmtCurrency = "#,##0;-#,##0";
    private const string ExcelFmtPercent = "0.0%";

    private readonly ILogging _log;
    private readonly IConfigSettings _config;
  }
}

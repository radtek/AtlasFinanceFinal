using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Quartz;
using Dapper;
using DevExpress.Spreadsheet;
using DevExpress.Xpo;

using Atlas.Common.Interface;
using Atlas.Common.Extensions;
using Atlas.Domain.Model;


namespace Atlas.Accounts.Reports.QuartzTasks
{
  class Audit_Trail : IJob
  {
    public Audit_Trail(ILogging log, IConfigSettings config)
    {
      _log = log;
      _config = config;
    }


    public void Execute(IJobExecutionContext context)
    {
      try
      {
        var recipients = GetRecipients();
        if (recipients.Any())
        {
          // Create like for like monthly periods, start with 1st of month and ending with yesterday (1-10 June & 1-10 May, 1-31 Mar & 1-28 Feb)
          var thisMonthEnd = DateTime.Today.Subtract(TimeSpan.FromDays(1));
          var thisMonthSTart = new DateTime(thisMonthEnd.Year, thisMonthEnd.Month, 1);

          var prevMonth = thisMonthSTart.Subtract(TimeSpan.FromDays(1));
          var prevMonthStart = new DateTime(prevMonth.Year, prevMonth.Month, 1);
          var prevMonthEnd = new DateTime(prevMonth.Year, prevMonth.Month, Math.Min(DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month), thisMonthEnd.Day));

          // If we are on the last day of this month, we must also be on the last day of previous month
          if (DateTime.DaysInMonth(thisMonthEnd.Year, thisMonthEnd.Month) == thisMonthEnd.Day)
          {
            prevMonthEnd = new DateTime(prevMonthEnd.Year, prevMonthEnd.Month, DateTime.DaysInMonth(prevMonthEnd.Year, prevMonthEnd.Month));
          }
          _log.Information("Audit_Trail starting");
          using (var conn = new Npgsql.NpgsqlConnection(_config.GetAssConnectionString()))
          {
            conn.Open();

            var rows1 = conn.Query<AuditTrailItem>(QUERY_AUDIT, new { StartDate = thisMonthSTart, EndDate = thisMonthEnd }, null, true, 60).ToList();
            var rows2 = conn.Query<AuditTrailItem>(QUERY_AUDIT, new { StartDate = prevMonthStart, EndDate = prevMonthEnd }, null, true, 60).ToList();

            using (var workbook = new Workbook())
            {
              workbook.BeginUpdate();
              var sheet = workbook.Worksheets[0];
              sheet.Name = "Audit trail";

              var row = 0;
              sheet[row, 0].Value = "Date range";
              sheet.MergeCells(sheet.Range.FromLTRB(0, row, 1, row));

              sheet[row, 2].Value = "Loans issued";
              sheet.MergeCells(sheet.Range.FromLTRB(2, row, 4, row));

              sheet[row, 5].Value = "Income";
              sheet.MergeCells(sheet.Range.FromLTRB(5, row, 12, row));

              sheet[row, 13].Value = "Debtors";
              sheet.MergeCells(sheet.Range.FromLTRB(13, row, 17, row));

              row++;
              var col = 0;
              sheet[row, col++].Value = "Start";
              sheet[row, col++].Value = "End";
              sheet[row, col++].Value = "New clients";
              sheet[row, col++].Value = "New loans";
              sheet[row, col++].Value = "Loans";
              sheet[row, col++].Value = "Int. & fees";
              sheet[row, col++].Value = "Insurance";
              sheet[row, col++].Value = "Nucard fee";
              sheet[row, col++].Value = "VAP";
              sheet[row, col++].Value = "Net Jnl/WO";
              sheet[row, col++].Value = "Discount";
              sheet[row, col++].Value = "Revenue";
              sheet[row, col++].Value = "% Revenue to loans";
              sheet[row, col++].Value = "Handovers";
              sheet[row, col++].Value = "VAT on revenue";
              sheet[row, col++].Value = "Receipts";
              sheet[row, col++].Value = "Opening";
              sheet[row, col++].Value = "Closing";
              sheet.Rows[1].Height = sheet.Rows[1].Height * 3;

              sheet.Range.FromLTRB(0, row, 17, row).Alignment.WrapText = true;
              sheet.Range.FromLTRB(0, 0, 17, row).Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
              sheet.Range.FromLTRB(0, 0, 17, row).Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
              sheet.Range.FromLTRB(0, 0, 17, row).Font.Bold = true;

              sheet.Range.FromLTRB(0, 0, 17, row).Borders.SetAllBorders(System.Drawing.Color.Black, BorderLineStyle.Thin);

              row++;
              // First date range
              AddRow(sheet, ++row, rows1);

              // Second date range
              AddRow(sheet, ++row, rows2);

              // Variance
              row += 2;
              col = 0;
              sheet[row, col++].Value = "";
              sheet[row, col++].Value = "Variance";
              sheet[row, col++].Value = rows1.Sum(s => s.New_Client) - rows2.Sum(s => s.New_Client);
              sheet[row, col++].Value = rows1.Sum(s => s.New_Loan) - rows2.Sum(s => s.New_Loan);
              var netLoans = rows1.Sum(s => s.NetLoans) - rows2.Sum(s => s.NetLoans);
              sheet[row, col++].Value = (double)(netLoans);
              sheet[row, col++].Value = (double)(rows1.Sum(s => s.NetFees_Interest) - rows2.Sum(s => s.NetFees_Interest));
              sheet[row, col++].Value = (double)(rows1.Sum(s => s.NetInsure) - rows2.Sum(s => s.NetInsure));
              sheet[row, col++].Value = (double)(rows1.Sum(s => s.NuCardFee) - rows2.Sum(s => s.NuCardFee));
              sheet[row, col++].Value = (double)(rows1.Sum(s => s.Vap_Excl) - rows2.Sum(s => s.Vap_Excl));
              sheet[row, col++].Value = (double)(rows1.Sum(s => s.Journal_WrOff) - rows2.Sum(s => s.Journal_WrOff));
              sheet[row, col++].Value = (double)(rows1.Sum(s => s.EarlyDisc) - rows2.Sum(s => s.EarlyDisc));
              var revenue = (rows1.Sum(s => s.Revenue) - rows2.Sum(s => s.Revenue));
              sheet[row, col++].Value = (double)revenue;

              sheet[row, col++].Value = (double)(netLoans > 0 ? revenue / netLoans : 0);
              sheet[row, col++].Value = (double)(rows1.Sum(s => s.HandOver) - rows2.Sum(s => s.HandOver));
              sheet[row, col++].Value = (double)(rows1.Sum(s => s.Vat) - rows2.Sum(s => s.Vat));
              sheet[row, col++].Value = (double)(rows1.Sum(s => s.NetReceipts) - rows2.Sum(s => s.NetReceipts));
              sheet[row, col++].Value = (double)(rows1.Sum(s => s.Open_bal) - rows2.Sum(s => s.Open_bal));
              sheet[row, col++].Value = (double)(rows1.Sum(s => s.Close_bal) - rows2.Sum(s => s.Close_bal));

              sheet.Range.FromLTRB(2, row, 17, row).Borders.TopBorder.LineStyle = BorderLineStyle.Thin;
              sheet.Range.FromLTRB(2, row, 17, row).Borders.BottomBorder.LineStyle = BorderLineStyle.Thin;
              sheet.Range.FromLTRB(1, row, 17, row).Font.Bold = true;

              // Formats
              sheet.Range.FromLTRB(0, 2, 1, row).NumberFormat = "yyyy-MM-dd";
              sheet.Range.FromLTRB(2, 2, 17, row).NumberFormat = "#,##0;-#,##0";
              sheet.Columns[12].NumberFormat = "0.00%";

              #region Print options
              sheet.SetPrintRange(sheet.Range.FromLTRB(0, 0, 17, row));

              sheet.ActiveView.Orientation = PageOrientation.Landscape;
              sheet.ActiveView.PaperKind = System.Drawing.Printing.PaperKind.A4;

              var printOptions = sheet.PrintOptions;
              printOptions.PrintGridlines = false;
              printOptions.FitToPage = true;
              printOptions.FitToHeight = 9999;
              printOptions.FitToWidth = 1;
              #endregion

              #region Col widths
              var range = sheet.Range[$"A:R"];
              range.AutoFitColumns();
              for (var i = 0; i <= 17; i++)
              {
                sheet.Columns[i].Width = sheet.Columns[i].Width * 1.4;
              }
              #endregion

              #region Finish off borders
              sheet.Range.FromLTRB(0, 2, 0, row).Borders.LeftBorder.LineStyle = BorderLineStyle.Thin;
              sheet.Range.FromLTRB(0, row, 1, row).Borders.BottomBorder.LineStyle = BorderLineStyle.Thin;
              sheet.Range.FromLTRB(2, 0, 2, row).Borders.LeftBorder.LineStyle = BorderLineStyle.Thin;
              sheet.Range.FromLTRB(5, 0, 5, row).Borders.LeftBorder.LineStyle = BorderLineStyle.Thin;
              sheet.Range.FromLTRB(13, 0, 13, row).Borders.LeftBorder.LineStyle = BorderLineStyle.Thin;
              sheet.Range.FromLTRB(17, 0, 17, row).Borders.RightBorder.LineStyle = BorderLineStyle.Thin;
              #endregion

              sheet.ScrollTo(0, 0);
              workbook.EndUpdate();

              var tempFile = Path.Combine(Path.GetTempPath(), $"Audit_Trail_{DateTime.Today:yyyy_MM_dd}.xlsx");
              workbook.SaveDocument(tempFile);
              Utils.EMailUtils.SendEMail(_log, "Month-to-month audit trail", "Please find attached the Audit Trail report", recipients, tempFile);
              File.Delete(tempFile);
            }
          }
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "Audit_Trail");
      }
    }


    private void AddRow(Worksheet sheet, int row, List<AuditTrailItem> rows1)
    {
      var col = 0;
      sheet[row, col++].Value = rows1.Min(s => s.From_Date);
      sheet[row, col++].Value = rows1.Max(s => s.To_Date);
      sheet[row, col++].Value = rows1.Sum(s => s.New_Client);
      sheet[row, col++].Value = rows1.Sum(s => s.New_Loan);

      var netLoans = rows1.Sum(s => s.NetLoans);
      sheet[row, col++].Value = (double)netLoans;

      sheet[row, col++].Value = (double)rows1.Sum(s => s.NetFees_Interest);
      sheet[row, col++].Value = (double)rows1.Sum(s => s.NetInsure);
      sheet[row, col++].Value = (double)rows1.Sum(s => s.NuCardFee);
      sheet[row, col++].Value = (double)rows1.Sum(s => s.Vap_Excl);
      sheet[row, col++].Value = (double)rows1.Sum(s => s.Journal_WrOff);
      sheet[row, col++].Value = (double)rows1.Sum(s => s.EarlyDisc);

      var revenue = rows1.Sum(s => s.Revenue);
      sheet[row, col++].Value = (double)revenue;

      sheet[row, col++].Value = (double)(netLoans > 0 ? revenue / netLoans : 0);

      sheet[row, col++].Value = (double)rows1.Sum(s => s.HandOver);
      sheet[row, col++].Value = (double)rows1.Sum(s => s.Vat);
      sheet[row, col++].Value = (double)rows1.Sum(s => s.NetReceipts);
      sheet[row, col++].Value = (double)rows1.Sum(s => s.Open_bal);
      sheet[row, col++].Value = (double)rows1.Sum(s => s.Close_bal);
    }


    /// <summary>
    /// Dapper class for data
    /// </summary>
    class AuditTrailItem
    {
      public string Br_Name;
      public DateTime From_Date;
      public DateTime To_Date;
      public int New_Client;
      public int New_Loan;
      public decimal NetLoans;
      public decimal NetFees_Interest;
      public decimal NetInsure;
      public decimal NuCardFee;
      public decimal Vap_Excl;
      public decimal Journal_WrOff;
      public decimal EarlyDisc;
      public decimal Revenue;
      public decimal RevenueToLoans;
      public decimal HandOver;
      public decimal Vat;
      public decimal NetReceipts;
      public decimal Open_bal;
      public decimal Close_bal;
    }


    private static List<string> GetRecipients()
    {
      using (var uow = new UnitOfWork())
      {
        return uow.Query<Config>()
          .Where(s => s.DataType == Enumerators.Config.Alerting.MonthToMonthAudit.ToInt())
          .Select(s => s.DataValue)
          .ToList();
      }
    }


    private const string QUERY_AUDIT = @"
SELECT au.brnum as Br_Num, br.sysname as Br_Name, 
(select MIN(au3.aud_date) from company.audit as au3 where au3.aud_date >= @StartDate and au3.aud_date <= @EndDate and au3.brnum=au.brnum) as From_Date, 
(select MAX(au3.aud_date) from company.audit as au3 where au3.aud_date >= @StartDate and au3.aud_date <= @EndDate and au3.brnum=au.brnum) as To_Date, 
sum(COALESCE(au.aud_newcli,0)) as New_Client, 
sum(COALESCE(au.aud_newlon,0)) as New_Loan, 
sum(COALESCE(au.aud_cheque,0)-COALESCE(au.aud_canc,0)+COALESCE(au.aud_cancrg,0)) as NetLoans, 
sum(COALESCE(au.aud_cash,0)+COALESCE(au.aud_int,0)-COALESCE(au.aud_cancrg,0) - COALESCE(au.aud_crgvat,0)+COALESCE(au.aud_canvat,0) - COALESCE(au.aud_prem,0)+COALESCE(au.aud_canpre,0)-COALESCE(au.aud_policy,0)+COALESCE(au.aud_canpol,0)    ) as NetFees_Interest,
sum(COALESCE(au.aud_prem,0)-COALESCE(au.aud_canpre,0)+COALESCE(au.aud_policy,0)-COALESCE(au.aud_canpol,0)) as NetInsure,
sum(COALESCE(au.aud_nucfee,0)) as NuCardFee, 
sum(COALESCE(au.aud_vapexc,0)) as Vap_Excl, 
sum(COALESCE(au.aud_jnldr,0)-COALESCE(au.aud_jnlcr,0)+COALESCE(au.aud_extra,0) + COALESCE(au.aud_woff,0)*-1 + COALESCE(au.aud_disc,0)*-1 ) as Journal_WrOff ,
sum(COALESCE(au.aud_early,0)*-1) as EarlyDisc, 
sum(COALESCE(au.aud_cash,0)+COALESCE(au.aud_int,0) + COALESCE(au.aud_nucfee,0) +  COALESCE(au.aud_vapexc,0) +  COALESCE(au.aud_jnldr,0)-COALESCE (au.aud_jnlcr,0)+COALESCE(au.aud_extra,0) + COALESCE(au.aud_woff,0)*-1 + COALESCE(au.aud_disc,0)*-1 + COALESCE(au.aud_early,0)*-1 - COALESCE(au.aud_cancrg,0) -   COALESCE(au.aud_crgvat,0) + COALESCE(au.aud_canvat,0) ) as Revenue ,

( SELECT CASE WHEN sum(COALESCE(au.aud_cheque,0)-COALESCE(au.aud_canc,0)+COALESCE(au.aud_cancrg,0))  = 0 THEN 0 ELSE
  sum(COALESCE(au.aud_cash,0)+COALESCE(au.aud_int,0) + COALESCE(au.aud_nucfee,0) +  COALESCE(au.aud_vapexc,0) +  COALESCE(au.aud_jnldr,0)-COALESCE(au.aud_jnlcr,0)+COALESCE(au.aud_extra,0) + COALESCE(au.aud_woff,0)*-1 + COALESCE(au.aud_disc,0)*-1 + COALESCE(au.aud_early,0)*-1 - COALESCE(au.aud_cancrg,0) - COALESCE(au.aud_crgvat,0) + COALESCE(au.aud_canvat,0) ) 
/  sum(  COALESCE(au.aud_cheque,0)-COALESCE(au.aud_canc,0)+COALESCE(au.aud_cancrg,0) )  * 100 
  END ) as RevenueToLoans,

sum(COALESCE(au.aud_bad,0)*-1) as HandOver, 
sum(COALESCE(au.aud_crgvat,0)-COALESCE(au.aud_canvat,0) + COALESCE(au.aud_vapvat,0)) as Vat ,
sum(COALESCE(au.aud_paid,0)*-1+COALESCE(au.aud_refund,0) ) as NetReceipts, 
(SELECT COALESCE(au2.aud_open,0) from company.audit au2 where au2.brnum = au.brnum and au2.aud_date = (select MIN(au3.aud_date) from company.audit as au3 where au3.aud_date >= @StartDate and au3.aud_date <= @EndDate and au3.brnum=au.brnum) and sr_deleted <> 'T') as Open_bal ,
(SELECT COALESCE(au2.aud_close,0) from company.audit au2 where au2.brnum = au.brnum and au2.aud_date = (select MAX(au3.aud_date) from company.audit as au3 where au3.aud_date >= @StartDate and au3.aud_date <= @EndDate and au3.brnum=au.brnum) and sr_deleted <> 'T') as Close_bal
FROM company.audit au, company.asbranch br
WHERE au.aud_date >= @StartDate 
  AND au.aud_date <= @EndDate
  AND au.brnum = br.brnum
  GROUP BY au.brnum, br.sysname
  ORDER by au.brnum 
";


    private readonly ILogging _log;
    private readonly IConfigSettings _config;

  }
}

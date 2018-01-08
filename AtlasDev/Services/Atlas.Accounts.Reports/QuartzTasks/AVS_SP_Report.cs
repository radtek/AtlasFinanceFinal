using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

using Quartz;
using DevExpress.Xpo;
using DevExpress.Export.Xl;

using Atlas.Domain.Model;
using Atlas.Common.Extensions;
using Atlas.Common.Interface;


namespace Atlas.Accounts.Reports.QuartzTasks
{
  /// <summary>
  /// AVS Service provider automated report- to be run on 1st of the month, to give AVS SP break-down of previous month
  /// </summary>
  [DisallowConcurrentExecution]
  class AVS_SP_Report : IJob
  {
    public AVS_SP_Report(ILogging log, IConfigSettings config)
    {
      _log = log;
      _config = config;
    }
    

    public void Execute(IJobExecutionContext context)
    {
      _log.Information("AVS_SP_Report.Execute starting");
      try
      {
        var avsRecipients = GetAVSRecipients();

        if (avsRecipients.Any())
        {
          var sql =
            "WITH avs_counts AS(SELECT t.\"CompanyId\", t.\"ServiceId\" as service_id, COUNT(*) AS avs_count " +
            "FROM \"AVS_Transaction\" t " +
            "WHERE date_trunc('month', \"CreateDate\") BETWEEN " +
            "          date_trunc('MONTH', current_date - INTERVAL '1 MONTH')::DATE AND " +
            "         (date_trunc('MONTH', current_date - INTERVAL '1 MONTH') + INTERVAL '1 MONTH - 1 DAY')::DATE " +
            "  AND \"ThirdPartyRef\" IS NOT NULL " +
            "GROUP BY t.\"CompanyId\", t.\"ServiceId\") " +

            "SELECT c.\"Name\", b.\"LegacyBranchNum\", " +
            "CASE " +
            "  WHEN avs_counts.service_id = 7 THEN 'CompuScan' " +
            "  WHEN avs_counts.service_id = 8 THEN 'XDS' " +
            "  WHEN avs_counts.service_id = 9 THEN 'Altech' " +
            "  ELSE 'Inactive AVS service' " +
            "END AS SP, avs_counts.avs_count as count " +
            "FROM avs_counts " +
            "JOIN \"CPY_Company\" c on c.\"CompanyId\" = avs_counts.\"CompanyId\" " +
            "JOIN \"BRN_Branch\" b on b.\"Company\" = c.\"CompanyId\" " +
            "ORDER BY b.\"LegacyBranchNum\", avs_counts.service_id";

          var data = new List<Tuple<string, string, string, Int64>>();

          using (var conn = new Npgsql.NpgsqlConnection(_config.GetAtlasCoreConnectionString()))
          {
            conn.Open();

            using (var cmd = conn.CreateCommand())
            {
              cmd.CommandTimeout = 180;
              cmd.CommandText = sql;
              using (var rdr = cmd.ExecuteReader())
              {
                while (rdr.Read())
                {
                  data.Add(new Tuple<string, string, string, Int64>(rdr.GetString(0), rdr.GetString(1), rdr.GetString(2), rdr.GetInt64(3)));
                }
              }
            }
          }

          var tempFile = CreateSpreadsheet2(data);
          _log.Information("AVS_SP_Report.Execute Sending to {@Recipients}", avsRecipients);
          Utils.EMailUtils.SendEMail(_log, "AVS report", "Please find attach AVS report", avsRecipients, tempFile);
          File.Delete(tempFile);
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "Execute()");
      }

      _log.Information("AVS_SP_Report.Execute complete");
    }

    
    /// <summary>
    /// Creates spreadsheet using DevExpress XL Export Library 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private static string CreateSpreadsheet2(List<Tuple<string, string, string, long>> data)
    {
      var tempFile = Path.Combine(Path.GetTempPath(), string.Format("{0}.xlsx", Guid.NewGuid().ToString("N")));
      var exporter = XlExport.CreateExporter(XlDocumentFormat.Xlsx);
      using (var stream = new FileStream(tempFile, FileMode.Create, FileAccess.ReadWrite))
      {
        using (var document = exporter.CreateDocument(stream))
        {
          document.Properties.Title = "Monthly AVS Service provider usage count, per branch, per service provider";
          document.Properties.Subject = "Monthly AVS Service provider usage count";
          document.Properties.Keywords = "AVS";
          document.Properties.Description = "Monthly AVS Service provider usage count, per branch, per service provider";
          document.Properties.Category = "AVS";
          document.Properties.Company = "Atlas Finance (Pty) Ltd.";
          document.Properties.Created = DateTime.Now;
         
          using (var sheet = document.CreateSheet())
          {
            sheet.Name = "AVS SP- counts";

            #region Page/print
            sheet.PageSetup = new XlPageSetup();
            sheet.PageSetup.PaperKind = System.Drawing.Printing.PaperKind.A4;

            sheet.PrintOptions = new XlPrintOptions();            
            sheet.PrintOptions.Headings = true;          
            sheet.PrintOptions.GridLines = true;
            // Center worksheet data on a printed page.
            sheet.PrintOptions.HorizontalCentered = true;
            sheet.PrintOptions.VerticalCentered = true;
            #endregion

            #region Add columns
            using (var column = sheet.CreateColumn())
            {
              column.WidthInPixels = 150;
            }
            using (var column = sheet.CreateColumn())
            {
              column.WidthInPixels = 70;
            }

            using (var column = sheet.CreateColumn())
            {
              column.WidthInPixels = 100;              
            }

            using (var column = sheet.CreateColumn())
            {
              column.WidthInPixels = 100;
              column.Formatting = new XlCellFormatting();
              column.Formatting.NumberFormat = "#,###";
            }
            #endregion

            #region Excel styles
            var cellFormatting = new XlCellFormatting() { Font = new XlFont() };
            cellFormatting.Font.Name = "Tahoma";
            cellFormatting.Font.SchemeStyle = XlFontSchemeStyles.None;

            var headerRowFormatting = new XlCellFormatting();
            headerRowFormatting.CopyFrom(cellFormatting);
            headerRowFormatting.Font.Bold = true;
            headerRowFormatting.Font.Color = XlColor.FromTheme(XlThemeColor.Light1, 0.0);
            headerRowFormatting.Fill = XlFill.SolidFill(XlColor.FromTheme(XlThemeColor.Accent2, 0.0));
            #endregion

            #region Header row
            using (var row = sheet.CreateRow())
            {
              using (var cell = row.CreateCell())
              {
                cell.Value = "Name";
                cell.ApplyFormatting(headerRowFormatting);
              }
              using (var cell = row.CreateCell())
              {
                cell.Value = "Code";
                cell.ApplyFormatting(headerRowFormatting);
              }
              using (var cell = row.CreateCell())
              {
                cell.Value = "Provider";
                cell.ApplyFormatting(headerRowFormatting);
              }
              using (var cell = row.CreateCell())
              {
                cell.Value = "Count";
                cell.ApplyFormatting(headerRowFormatting);
              }
            }
            #endregion

            #region Data
            foreach (var row in data)
            {
              using (var excelRow = sheet.CreateRow())
              {
                using (var cell = excelRow.CreateCell())
                {
                  cell.Value = row.Item1;
                  cell.ApplyFormatting(cellFormatting);
                }
                using (var cell = excelRow.CreateCell())
                {
                  cell.Value = row.Item2;
                  cell.ApplyFormatting(cellFormatting);
                }
                using (var cell = excelRow.CreateCell())
                {
                  cell.Value = row.Item3;
                  cell.ApplyFormatting(cellFormatting);
                }
                using (var cell = excelRow.CreateCell())
                {
                  cell.Value = row.Item4;
                  cell.ApplyFormatting(cellFormatting);
                }
              }
            }
            #endregion
                       
            #region Total row
            var totalRowFormatting = new XlCellFormatting();
            totalRowFormatting.CopyFrom(cellFormatting);
            totalRowFormatting.Font.Bold = true;
            totalRowFormatting.Fill = XlFill.SolidFill(XlColor.FromTheme(XlThemeColor.Accent5, 0.6));

            using (var row = sheet.CreateRow())
            {
              using (var cell = row.CreateCell())
              {
                cell.ApplyFormatting(totalRowFormatting);
              }
              using (var cell = row.CreateCell())
              {
                cell.ApplyFormatting(totalRowFormatting);
              }

              using (var cell = row.CreateCell())
              {
                cell.Value = "Total";
                cell.ApplyFormatting(totalRowFormatting);
                cell.ApplyFormatting(XlCellAlignment.FromHV(XlHorizontalAlignment.Right, XlVerticalAlignment.Center));
              }
              using (var cell = row.CreateCell())
              {
                // Add values in the cell range C2 through C9 using the SUBTOTAL function. 
                cell.SetFormula(XlFunc.Subtotal(XlCellRange.FromLTRB(3, 1, 3, data.Count), XlSummary.Sum, true));
                cell.ApplyFormatting(totalRowFormatting);
              }
            }
            #endregion

            sheet.AutoFilterRange = XlCellRange.FromLTRB(0, 0, 3, data.Count);
            sheet.PrintArea = sheet.DataRange;
            // Freeze the first row in the worksheet
            sheet.SplitPosition = new XlCellPosition(0, 1);
          }
        }        
      }
      return tempFile;
    }


    private static List<string> GetAVSRecipients()
    {
      using (var uow = new UnitOfWork())
      {
        return uow.Query<Config>()
          .Where(s => s.DataType == Enumerators.Config.Alerting.AVSUsage.ToInt())
          .Select(s => s.DataValue)
          .ToList();
      }
    }



    private readonly ILogging _log;
    private readonly IConfigSettings _config;

  }

}

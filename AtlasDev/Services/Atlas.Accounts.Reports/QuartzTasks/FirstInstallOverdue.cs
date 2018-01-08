using System;
using System.Collections.Generic;
using System.Linq;
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
  class FirstInstallOverdue : IJob
  {
    public FirstInstallOverdue(ILogging log, IConfigSettings config)
    {
      _log = log;
      _config = config;
    }


    public void Execute(IJobExecutionContext context)
    {
      _log.Information("FirstInstallOverdue.Execute() starting");
      try
      {
        var recipients = GetRecipients(DateTime.Now.Hour < 12 ? 
          Enumerators.Config.Alerting.FirstInstalmentOverdueAll : Enumerators.Config.Alerting.FirstInstalmentOverdueSales);
        if (recipients.Any())
        {
          var rows = GetData();

          if (rows.Any())
          {
            var tempFile = CreateSpreadsheet(rows);
            EMailUtils.SendEMail(_log, "First instalment overdue report", "Please see attached Excel report", recipients, tempFile);
            File.Delete(tempFile);
          }
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "");
      }
      _log.Information("FirstInstallOverdue.Execute() completed");
    }


    /// <summary>
    /// Gets overdue data
    /// </summary>
    /// <returns></returns>
    private List<Overdue> GetData()
    {
      // On MOnday, do for Sat/Sun
      var days = DateTime.Today.DayOfWeek == DayOfWeek.Monday ?
        "tr.trdate BETWEEN CURRENT_DATE - INTERVAL '2 days' AND CURRENT_DATE - interval '1 day'" :
        "tr.trdate = CURRENT_DATE - interval '1 day'";

      var sql =
          "SELECT tr.brnum as branchnum, br.sysname as branchname, br.regioncode as regioncode, tr.client, tr.loan, " +
          "ln.loanmeth, ln.payno_orig as LoanTerm, ln.nctrantype as TranType, tr.\"order\" as InstalNum,  " +
          "tr.trdate as DueDate, tr.tramount as Amount, cl.hometel, cl.worktel, cl.cell, cl.firstname, cl.othname, cl.surname, cl.identno, cl.workname " +
          "FROM company.trans tr, company.loans ln, company.asbranch br, company.client cl " +
          $"WHERE tr.trtype = 'R' and CHAR_LENGTH(TRIM(BOTH ' ' FROM COALESCE(tr.trstat, ''))) = 0 " +
          $"  AND {days} " +
          "  AND tr.\"order\" = 1 and tr.seqno = 1 " +
          "  AND ln.nctrantype IN ('USE', 'N/A', 'VAP') " +
          "  AND COALESCE(tr.tramount, 0) > 0 " +
          "  AND tr.brnum = ln.brnum AND tr.client = ln.client and tr.loan = ln.loan " +
          "  AND br.brnum = tr.brnum " +
          "  AND cl.brnum = tr.brnum AND cl.client = tr.client " +
          "  ORDER by tr.brnum, tr.client, tr.loan";

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
        var range = sheet.Range[$"A1:{sheet[currRowNo, _lastCol].GetReferenceA1()}"];
        range.AutoFitColumns();

        //var col = _lastCol - 7;
        //sheet.Columns[col++].WidthInCharacters = 10;
        //sheet.Columns[col++].WidthInCharacters = 30;
        //sheet.Columns[col++].WidthInCharacters = 10;
        //sheet.Columns[col++].WidthInCharacters = 30;
        //sheet.Columns[col++].WidthInCharacters = 30;
        //sheet.Columns[col++].WidthInCharacters = 10;
        //sheet.Columns[col].WidthInCharacters = 30;
        #endregion

        sheet.AutoFilter.Apply(range);
        sheet.FreezeRows(0);

        workbook.EndUpdate();
        workbook.SaveDocument(filename, DocumentFormat.Xlsx);
      }

      return filename;
    }


    private static List<string> GetRecipients(Enumerators.Config.Alerting alertType)
    {
      using (var uow = new UnitOfWork())
      {
        return uow.Query<Config>()
          .Where(s => s.DataType == alertType.ToInt())
          .Select(s => s.DataValue)
          .ToList();
      }
    }


    private static void AddHeader(int currRowNo, Worksheet sheet)
    {
      var col = 0;
      sheet.Cells[currRowNo, col++].Value = "BNum";
      sheet.Cells[currRowNo, col++].Value = "Branch";
      sheet.Cells[currRowNo, col++].Value = "Region";
      sheet.Cells[currRowNo, col++].Value = "Client";
      sheet.Cells[currRowNo, col++].Value = "Loan";
      sheet.Cells[currRowNo, col++].Value = "Period";
      sheet.Cells[currRowNo, col++].Value = "Term";
      sheet.Cells[currRowNo, col++].Value = "Instalment";
      sheet.Cells[currRowNo, col++].Value = "Due date";
      sheet.Cells[currRowNo, col++].Value = "Amount";

      sheet.Cells[currRowNo, col++].Value = "ID";
      sheet.Cells[currRowNo, col++].Value = "First";
      sheet.Cells[currRowNo, col++].Value = "Other";
      sheet.Cells[currRowNo, col++].Value = "Surname";
      sheet.Cells[currRowNo, col++].Value = "Cell";
      sheet.Cells[currRowNo, col++].Value = "Work";
      sheet.Cells[currRowNo, col++].Value = "Home";
      sheet.Cells[currRowNo, col++].Value = "Company";

      sheet.Cells[currRowNo, col++].Value = "Date contacted";
      sheet.Cells[currRowNo, col++].Value = "Number contacted";
      sheet.Cells[currRowNo, col++].Value = "Reason for no payment";
      sheet.Cells[currRowNo, col++].Value = "Confirmed paydate";
      sheet.Cells[currRowNo, col++].Value = "Number client contacted on";
      sheet.Cells[currRowNo, col].Value = "Rearranged payment date";

      _lastCol = col;
      sheet.Range[$"{sheet[currRowNo, 0].GetReferenceA1()}:{sheet[currRowNo, col].GetReferenceA1()}"].Font.Bold = true;
    }


    private static void AddRow(int rowNo, Worksheet sheet, ExcelRow row)
    {
      var col = 0;
      sheet.Cells[rowNo, col++].Value = row.BranchNum;
      sheet.Cells[rowNo, col++].Value = row.BranchName;
      sheet.Cells[rowNo, col++].Value = row.Region.Substring(1);
      sheet.Cells[rowNo, col++].Value = row.Client.ToString("00000");
      sheet.Cells[rowNo, col++].Value = row.Loan.ToString("0000");
      sheet.Cells[rowNo, col++].Value = row.Period;
      sheet.Cells[rowNo, col++].Value = row.Term;
      sheet.Cells[rowNo, col++].Value = row.InstalNum;
      sheet.Cells[rowNo, col++].Value = row.DueDate;
      sheet.Cells[rowNo, col++].Value = (float)row.Amount;

      sheet.Cells[rowNo, col++].Value = row.Identno;
      sheet.Cells[rowNo, col++].Value = row.FirstName;
      sheet.Cells[rowNo, col++].Value = row.OthName;
      sheet.Cells[rowNo, col++].Value = row.Surname;
      sheet.Cells[rowNo, col++].Value = row.Cell;
      sheet.Cells[rowNo, col++].Value = row.WorkTel;
      sheet.Cells[rowNo, col++].Value = row.HomeTel;
      sheet.Cells[rowNo, col++].Value = row.WorkName;
    }


    private static ExcelRow MapOverdue(Overdue row)
    {
      return new ExcelRow
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
        TransType = row.TranType,

        HomeTel = row.HomeTel,
        WorkTel = row.WorkTel,
        Cell = row.Cell,
        FirstName = row.FirstName,
        OthName = row.OthName,
        Surname = row.Surname,
        Identno = row.Identno,
        WorkName = row.WorkName
      };
    }


    private class Overdue
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

      public string HomeTel { get; set; }
      public string WorkTel { get; set; }
      public string Cell { get; set; }
      public string FirstName { get; set; }
      public string OthName { get; set; }
      public string Surname { get; set; }
      public string Identno { get; set; }
      public string WorkName { get; set; }
    }


    private class ExcelRow
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

      public string HomeTel { get; set; }
      public string WorkTel { get; set; }
      public string Cell { get; set; }
      public string FirstName { get; set; }
      public string OthName { get; set; }
      public string Surname { get; set; }
      public string Identno { get; set; }
      public string WorkName { get; set; }
    }


    private readonly IConfigSettings _config;

    private readonly ILogging _log;

    private static int _lastCol = 0;

  }
}

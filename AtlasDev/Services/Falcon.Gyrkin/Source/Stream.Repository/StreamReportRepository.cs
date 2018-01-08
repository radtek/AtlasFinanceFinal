using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Common.Extensions;
using Atlas.Common.Utils;
using Atlas.Domain.Model;
using Atlas.Reporting.DTO;
using DevExpress.Spreadsheet;
using DevExpress.Xpo;
using Falcon.Common.Interfaces.Services;
using Newtonsoft.Json;
using Serilog;
using Stream.Framework.Repository;
using Stream.Framework.Structures;
using Stream.Framework.Structures.Reports;
using Stream.Structures.Models.Reports;

namespace Stream.Repository
{
  public class StreamReportRepository : IStreamReportRepository
  {
    #region injected objs

    private readonly IStreamRepository _streamRepository;
    private readonly ILogger _logger;
    private readonly IConfigService _configService;

    #endregion

    #region private classes

    private class QueryString
    {
      public QueryString()
      {
        WhereClauses = new List<String>();
        SelectColumns = new List<string>();
      }

      public List<string> WhereClauses { get; set; }
      public List<string> SelectColumns { get; set; }
    }

    #endregion

    #region private enum

    private enum SpecifiedColumn
    {
      [Description("{'WhereClauses':['1 = 1']}")]
      Cases,
      [Description("{'WhereClauses':['1 = 1']}")]
      Accounts,
      [Description("{'WhereClauses':['1 = 1']}")]
      Debtors,
      [Description("{'WhereClauses':['CSE.\"WorkableCase\" = false']}")]
      SystemClosedCases,
      [Description("{'WhereClauses':['CSE.\"WorkableCase\" IS NULL OR CSE.\"WorkableCase\" = true']}")]
      WorkableCases,
      [Description("{'WhereClauses':['CSE.\"CaseStatusId\" IN (1,2,3) AND CST.\"CompleteDate\" IS NULL AND (ALC.\"TransferredOut\" IS NULL OR ALC.\"TransferredOut\" = false)']}")]
      CurrentAccounts,
      [Description("{'WhereClauses':['CSE.\"CaseStatusId\" IN (1,2,3) AND CST.\"CompleteDate\" IS NULL AND (ALC.\"TransferredOut\" IS NULL OR ALC.\"TransferredOut\" = false)']}")]
      CurrentClients,
      [Description("{'WhereClauses':['CSE.\"CaseStatusId\" IN (1,2,3) AND CST.\"CompleteDate\" IS NULL AND (ALC.\"TransferredOut\" IS NULL OR ALC.\"TransferredOut\" = false)']}")]
      CurrentCases,

      [Description("{'WhereClauses':['CST.\"CreateDate\" BETWEEN \\'{0}\\' AND \\'{1}\\' AND CSA.\"ActionTypeId\" = CASE WHEN CST.\"StreamId\" = 2 THEN 2 ELSE 3 END AND CST.\"StreamId\" IN (2, 3)']}")]
      PtpPtcObtained,
      [Description("{'WhereClauses':['CST.\"CreateDate\" BETWEEN \\'{0}\\' AND \\'{1}\\' AND CSA.\"ActionTypeId\" = CASE WHEN CST.\"StreamId\" = 2 THEN 2 ELSE 3 END AND CST.\"StreamId\" IN (5, 7)']}")]
      PtpPtcBroken,
      [Description("{'WhereClauses':['CST.\"CompleteDate\" BETWEEN \\'{0}\\' AND \\'{1}\\' AND CSA.\"ActionTypeId\" = CASE WHEN CST.\"StreamId\" = 2 THEN 2 ELSE 3 END AND CST.\"StreamId\" IN (2, 3) AND CSA.\"IsSuccess\"']}")]
      PtpPtcSuccessful,
      [Description("{'WhereClauses':['CSA.\"ActionDate\" BETWEEN \\'{0}\\' AND \\'{1}\\' AND CST.\"StreamId\" = 4']}")]
      FollowUps,

      [Description("{'WhereClauses':['CAL.\"NoActionCount\" > 0']}")]
      NoActionCount,
      [Description("{'WhereClauses':['CST.\"EscalationId\"> 1']}")]
      Escalations,
      [Description("{'WhereClauses':['CAL.\"TransferredIn\" = true']}")]
      TransferredIn,
      [Description("{'WhereClauses':['CAL.\"TransferredOut\" = true']}")]
      TransferredOut,
      [Description("{'WhereClauses':['CAL.\"SMSCount\" > 0']}")]
      SMSCount
    }

    private enum StreamPerformanceReportQueryGroup
    {
      [Description("\"RegionId\", \"Region\"")]
      Region,
      [Description("\"BranchId\", \"Branch\"")]
      Branch,
    }

    #endregion

    #region constructors

    public StreamReportRepository(IStreamRepository streamRepository, IConfigService configService, ILogger logger)
    {
      _streamRepository = streamRepository;
      _logger = logger;
      _configService = configService;
    }

    #endregion

    #region public methods

    public byte[] GetPerformanceReport(Framework.Enumerators.Stream.GroupType groupType, DateTime startDate,
      DateTime endDate, long[] branchIds = null)
    {
      try
      {
        byte[] file;
        Dictionary<long, long> branches;
        using (var uow = new UnitOfWork())
        {
          var branchesQuery = new XPQuery<BRN_Branch>(uow).Where(b => b.Region != null).AsQueryable();
          if (branchIds != null && branchIds.Length > 0) // added branch count check
          {
            branchesQuery = branchesQuery.Where(b => branchIds.Contains(b.BranchId)).AsQueryable();
          }
          branches = branchesQuery.ToList().ToDictionary(b => b.BranchId, b => b.Region.RegionId);
        }

        if (groupType == 0) // added groupType check
        {
          groupType = Framework.Enumerators.Stream.GroupType.Collections;
        }

        var streamPerformanceReportQueryGroup = branches.Select(b => b.Value).Distinct().Count() > 1 ? StreamPerformanceReportQueryGroup.Region : StreamPerformanceReportQueryGroup.Branch;

        string query;
        switch (streamPerformanceReportQueryGroup)
        {
          case StreamPerformanceReportQueryGroup.Branch:
            query = Atlas.Reporting.Properties.Resources.STR_PerformanceReportUser;
            break;
          case StreamPerformanceReportQueryGroup.Region:
            query = Atlas.Reporting.Properties.Resources.STR_PerformanceReport;
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }

        var sqlPerformanceQuery = string.Format(query,
          startDate.ToString("yyyy-MM-dd"), endDate.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss"),
         string.Join(",", branches.Keys), (int)groupType);
        var rawSqlUtil = new RawSql();

        var performanceReportResults = new List<STR_Performance>();
        var performReportResultTask = Task.Run(() =>
          performanceReportResults = rawSqlUtil.ExecuteObject<STR_Performance>(
          sqlPerformanceQuery, _configService.AtlasCoreConnection).OrderBy(b => b.Branch).ToList());

        using (var workbook = new Workbook())
        {
          workbook.Unit = DevExpress.Office.DocumentUnit.Point;

          if (streamPerformanceReportQueryGroup == StreamPerformanceReportQueryGroup.Region)
          {
            var sqlPerformanceBranchesQuery = string.Format(Atlas.Reporting.Properties.Resources.STR_PerformanceReportUser,
              startDate.ToString("yyyy-MM-dd"), endDate.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss"),
             string.Join(",", branches.Keys), (int)groupType);

            var performReportBranchResult = rawSqlUtil.ExecuteObject<STR_Performance>(sqlPerformanceBranchesQuery, _configService.AtlasCoreConnection).OrderBy(b => b.Branch);

            var branchKeys = performReportBranchResult.Select(b => b.BranchId).Distinct();
            foreach (var branchId in branchKeys)
            {
              var branchReport = performReportBranchResult.Where(b => b.BranchId == branchId).ToList();
              var strPerformance = branchReport.FirstOrDefault();
              if (strPerformance != null)
              {
                var worksheetBranch = workbook.Worksheets.Add(strPerformance.Branch);
                branchReport.Add(TotalPerformanceReport(branchReport));

                branchReport = CalcHitRatePercentage(branchReport);
                PopulatePerformanceWorksheet(ref worksheetBranch, branchReport, startDate, endDate, groupType);
              }
            }
          }

          if (streamPerformanceReportQueryGroup == StreamPerformanceReportQueryGroup.Branch)
          {
            var sqlPerformanceDailyQuery = string.Format(Atlas.Reporting.Properties.Resources.STR_PerformanceDaily,
              startDate.ToString("yyyy-MM-dd"), endDate.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss"), string.Join(",", branches.Keys),
              groupType.ToInt());
            var performanceDailyReportResults = rawSqlUtil.ExecuteObject<STR_PerformanceDaily>(
              sqlPerformanceDailyQuery, _configService.AtlasCoreConnection).OrderBy(d => d.Date).ThenBy(b => b.AllocatedUser).ToList();
            performanceDailyReportResults = UserDailyPerformanceReportFillInBlanks(performanceDailyReportResults, startDate, endDate);

            if (performReportResultTask.Status != TaskStatus.RanToCompletion)
            {
              performReportResultTask.Wait();
            }
            var branch = performanceReportResults.FirstOrDefault();
            if (branch != null)
            {
              var worksheetUser = workbook.Worksheets.Add(branch.Branch.Replace("/", "_"));
              PopulatePerformanceDailyWorksheet(ref worksheetUser, performanceDailyReportResults, groupType);
            }
          }

          if (performReportResultTask.Status != TaskStatus.RanToCompletion)
          {
            performReportResultTask.Wait();
          }

          var worksheet = workbook.Worksheets[0];
          worksheet.Name = "Consolidated";
          performanceReportResults.Add(TotalPerformanceReport(performanceReportResults));
          performanceReportResults = CalcHitRatePercentage(performanceReportResults);

          PopulatePerformanceWorksheet(ref worksheet, performanceReportResults, startDate, endDate, groupType);

          workbook.EndUpdate();
          file = workbook.SaveDocument(DocumentFormat.Xlsx);
        }
        return file;
      }
      catch (Exception exception)
      {
        _logger.Error(
          string.Format("[Stream Get Performance Report] startDate: {0}, endDate: {1}, branchIds: {2} - {3} - {4}",
            startDate, endDate, branchIds, exception.Message, exception.StackTrace));
        throw;
      }
    }

    public byte[] GetDetailReport(Framework.Enumerators.Stream.GroupType groupType, long branchId, int[] streamIds, int[] caseStatusIds)
    {
      var rawSqlUtil = new Atlas.Common.Utils.RawSql();
      var query = string.Format(Atlas.Reporting.Properties.Resources.STR_Detail, string.Join(",", caseStatusIds),
        groupType.ToInt(), branchId, string.Join(",", streamIds)); 
      var details = rawSqlUtil.ExecuteObject<STR_Detail>(query, _configService.AtlasCoreConnection);

      using (var workbook = new Workbook())
      {
        workbook.Unit = DevExpress.Office.DocumentUnit.Point;

        var worksheet = workbook.Worksheets[0];
        worksheet.Name = "Cases";
        // add to work book
        PopulateDetailExportWorksheet(ref worksheet, details);

        workbook.EndUpdate();
        return workbook.SaveDocument(DocumentFormat.Xlsx);
      }
    }

    private List<STR_Performance> CalcHitRatePercentage(List<STR_Performance> report)
    {
      foreach (var reportLine in report)
      {
        if (reportLine.CurrentCases > 0)
        {
          reportLine.PtpPtcHitRate = reportLine.PtpPtcObtained/(float) reportLine.CurrentCases;
          reportLine.PtpPtcHitRateSuccessful = reportLine.PtpPtcSuccessful/(float) reportLine.CurrentCases;
        }
      }
      return report;
    }

    private List<PerformanceSummary> CalcHitRatePercentage(List<PerformanceSummary> report)
    {
      foreach (var reportLine in report)
      {
        if (reportLine.CurrentCases > 0)
        {
          reportLine.PtpPtcHitRate =
            (float) Math.Round((reportLine.PtpPtcObtained/(decimal) reportLine.CurrentCases)*100, 2);
        }

        if (reportLine.PtpPtcObtained > 0)
        {
          reportLine.PtpPtcHitRateSuccessful =
            (float) Math.Round((reportLine.PtpPtcSuccessful/(decimal) reportLine.PtpPtcObtained)*100, 2);
        }
      }
      return report;
    }

    #region Report Drilldown methods

    public List<IPerformanceSummary> GetOverview(Framework.Enumerators.Stream.GroupType groupType, DateTime startDate,
      DateTime endDate, long regionId, int categoryId, int subCategoryId, long[] branchIds = null,
      int drillDownLevel = 1)
    {
      Dictionary<long, long> branches;
      using (var uow = new UnitOfWork())
      {
        var branchesQuery = new XPQuery<BRN_Branch>(uow).Where(b => b.Region != null).AsQueryable();
        if (branchIds != null && branchIds.Length > 0) // added branch count check
        {
          branchesQuery = branchesQuery.Where(b => branchIds.Contains(b.BranchId)).AsQueryable();
        }
        if (regionId > 0)
        {
          branchesQuery = branchesQuery.Where(b => b.Region.RegionId == regionId).AsQueryable();
        }
        branches = branchesQuery.ToList().ToDictionary(b => b.BranchId, b => b.Region.RegionId);
      }

      if (groupType == 0) // added groupType check
      {
        groupType = Framework.Enumerators.Stream.GroupType.Collections;
      }

      var groupingQueryString = string.Empty;
      switch (drillDownLevel)
      {
        case 1:
          groupingQueryString = StreamPerformanceReportQueryGroup.Region.ToStringEnum();
          break;
        case 2:
          groupingQueryString = string.Format("{0}, {1}",
            StreamPerformanceReportQueryGroup.Region.ToStringEnum(),
            StreamPerformanceReportQueryGroup.Branch.ToStringEnum());
          break;
        case 3:
          groupingQueryString = string.Format("{0}, {1}",
            StreamPerformanceReportQueryGroup.Region.ToStringEnum(),
            StreamPerformanceReportQueryGroup.Branch.ToStringEnum());
          break;
      }

      string query;
      switch (drillDownLevel)
      {
        case 1:
          query = Atlas.Reporting.Properties.Resources.STR_PerformanceReport;
          break;
        case 2:
          query = Atlas.Reporting.Properties.Resources.STR_PerformanceReportUser;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }


      var sqlPerformanceQuery = string.Format(query,
        startDate.ToString("yyyy-MM-dd"), endDate.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss"),
        string.Join(",", branches.Keys), groupType.ToInt());
      var rawSqlUtil = new Atlas.Common.Utils.RawSql();
      // TODO: cast from STR_Performance to PerformanceSummary
      var performanceReportResults = rawSqlUtil.ExecuteObject<PerformanceSummary>(
        sqlPerformanceQuery, _configService.AtlasCoreConnection);

      performanceReportResults = CalcHitRatePercentage(performanceReportResults);

      performanceReportResults =
        performanceReportResults.OrderBy(p => p.IsTotal ? 999 : 0)
          .ThenBy(p => p.Region)
          .ThenBy(p => p.Branch)
          .ThenBy(p => p.AllocatedUser)
          .ThenBy(p => p.Category)
          .ThenBy(p => p.SubCategory)
          .ToList();

      return new List<IPerformanceSummary>(performanceReportResults);
    }

    public List<IAccountStreamAction> GetAccounts(Framework.Enumerators.Stream.GroupType groupType, DateTime startDate,
      DateTime endDate, int categoryId, long[] branchIds, long allocatedUserId, string column)
    {
      var lstCategoryId = new List<int>();
      if (categoryId == 0)
      {
        lstCategoryId.AddRange(EnumUtil.GetValues<Framework.Enumerators.Category.Type>().Select(value => (int)value));
      }
      else
      {
        lstCategoryId.Add(categoryId);
      }

      var specifiedColumn = (SpecifiedColumn)Enum.Parse(typeof(SpecifiedColumn), column);
      // get caseIds from detailed query
      var rawSqlUtil = new Atlas.Common.Utils.RawSql();
      string sqlPerformanceQuery;
      switch (specifiedColumn)
      {
        case SpecifiedColumn.Accounts:
        case SpecifiedColumn.Debtors:
        case SpecifiedColumn.Cases:
        case SpecifiedColumn.SystemClosedCases:
        case SpecifiedColumn.WorkableCases:
        case SpecifiedColumn.CurrentAccounts:
        case SpecifiedColumn.CurrentCases:
        case SpecifiedColumn.CurrentClients:
        case SpecifiedColumn.PtpPtcObtained:
        case SpecifiedColumn.PtpPtcBroken:
        case SpecifiedColumn.PtpPtcSuccessful:
        case SpecifiedColumn.FollowUps:
        case SpecifiedColumn.NoActionCount:
        case SpecifiedColumn.SMSCount:
        case SpecifiedColumn.TransferredIn:
        case SpecifiedColumn.TransferredOut:
        case SpecifiedColumn.Escalations:
          var specifiedColumnDescription = specifiedColumn.ToStringEnum().Replace("\\\"", "\"");
          var queryString = JsonConvert.DeserializeObject<QueryString>(specifiedColumnDescription);
          var whereClause = BuildWhereClause(queryString);

          whereClause = string.Format(whereClause, startDate.ToString("yyyy-MM-dd"),
            endDate.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss"),
            (int)groupType,
            string.Join(",", branchIds),
            string.Join(",", lstCategoryId),
            allocatedUserId);

          sqlPerformanceQuery = string.Format(Atlas.Reporting.Properties.Resources.STR_PerformanceReportDetail,
            startDate.ToString("yyyy-MM-dd"),
            endDate.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss"),
            string.Join(",", branchIds),
            (int)groupType,
            string.Join(",", lstCategoryId),
            allocatedUserId,
            whereClause);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      var caseIds = rawSqlUtil.ExecuteObject(sqlPerformanceQuery, _configService.AtlasCoreConnection).ToArray();

      return _streamRepository.GetAccountStreamActions(caseIds);
    }

    private string BuildWhereClause(QueryString queryString)
    {
      return string.Join(" AND ", queryString.WhereClauses);
    }

    public byte[] GetAccountsExport(Framework.Enumerators.Stream.GroupType groupType, DateTime startDate,
      DateTime endDate, long regionId, int categoryId, long[] branchIds, long allocatedUserId, string column)
    {
      try
      {
        var accounts = GetAccounts(groupType, startDate, endDate, categoryId, branchIds, allocatedUserId, column);

        using (var workbook = new Workbook())
        {
          workbook.Unit = DevExpress.Office.DocumentUnit.Point;

          var accountExport = accounts.Select(account => new STR_AccountsExport()
          {
            AccountReference = account.AccountReference,
            AllocatedUser = account.AllocatedUserFullName,
            CaseId = account.CaseId,
            ClientName = account.DebtorFullName,
            IdNumber = account.DebtorIdNumber,
            Priority = account.Priority,
            Status = account.CaseStatus
          }).ToList();

          var worksheet = workbook.Worksheets[0];
          worksheet.Name = "Cases";
          // add to work book
          PopulateAccountExportWorksheet(ref worksheet, accountExport);

          workbook.EndUpdate();
          return workbook.SaveDocument(DocumentFormat.Xlsx);
        }
      }
      catch (Exception exception)
      {
        _logger.Error(
          string.Format("[Stream Get Performance Report] startDate: {0}, endDate: {1}, branchIds: {2} - {3} - {4}",
            startDate, endDate, branchIds, exception.Message, exception.StackTrace));
        throw;
      }
    }

    #endregion

    #endregion

    #region private methods

    #region excel

    private void PopulateAccountExportWorksheet(ref Worksheet worksheet, List<STR_AccountsExport> accountsExport)
    {
      var header = BuildAccountsExportHeader();
      worksheet.Import(header, 0, 0, false);
      // bold text
      for (var j = 0; j < header.Count; j++)
      {
        worksheet.Rows[0].Font.Bold = true;
        worksheet.Rows[0].FillColor = System.Drawing.Color.LightSteelBlue;
        worksheet.Rows[0].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
        worksheet.Rows[0].Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
        worksheet.Rows[0].Alignment.WrapText = true;
      }

      var colIndex = 0;
      for (var i = 1; i <= accountsExport.Count; i++)
      {
        colIndex = 0;
        worksheet.Cells[i, colIndex].Value = (float)accountsExport[i - 1].CaseId;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].ColumnWidthInCharacters = 30;
        worksheet.Cells[i, colIndex++].Value = accountsExport[i - 1].AccountReference;
        worksheet.Cells[i, colIndex].ColumnWidthInCharacters = 50;
        worksheet.Cells[i, colIndex++].Value = accountsExport[i - 1].ClientName;
        worksheet.Cells[i, colIndex].ColumnWidthInCharacters = 10;
        worksheet.Cells[i, colIndex++].Value = accountsExport[i - 1].IdNumber;
        worksheet.Cells[i, colIndex++].Value = accountsExport[i - 1].Priority;
        worksheet.Cells[i, colIndex].ColumnWidthInCharacters = 20;
        worksheet.Cells[i, colIndex++].Value = accountsExport[i - 1].Status;
        worksheet.Cells[i, colIndex].ColumnWidthInCharacters = 30;
        worksheet.Cells[i, colIndex++].Value = accountsExport[i - 1].AllocatedUser;
      }

      for (var i = 1; i <= accountsExport.Count; i++)
      {
        for (var j = 0; j < colIndex; j++)
        {
          worksheet.Cells[i, j].Borders.SetAllBorders(System.Drawing.Color.Black, BorderLineStyle.Thin);
        }
      }
    }

    private void PopulateDetailExportWorksheet(ref Worksheet worksheet, List<STR_Detail> details)
    {
      var header = BuildDetailExportHeader();
      worksheet.Import(header, 0, 0, false);
      // bold text
      for (var j = 0; j < header.Count; j++)
      {
        worksheet.Rows[0].Font.Bold = true;
        worksheet.Rows[0].FillColor = System.Drawing.Color.LightSteelBlue;
        worksheet.Rows[0].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
        worksheet.Rows[0].Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
        worksheet.Rows[0].Alignment.WrapText = true;
      }

      var colIndex = 0;
      for (var i = 1; i <= details.Count; i++)
      {
        colIndex = 0;
        worksheet.Cells[i, colIndex].ColumnWidthInCharacters = 15;
        worksheet.Cells[i, colIndex].Value = details[i - 1].CaseNo;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].ColumnWidthInCharacters = 20;
        worksheet.Cells[i, colIndex++].Value = details[i - 1].CaseStatus;
        worksheet.Cells[i, colIndex].ColumnWidthInCharacters = 20;
        worksheet.Cells[i, colIndex++].Value = details[i - 1].Stream;
        worksheet.Cells[i, colIndex].ColumnWidthInCharacters = 37;
        worksheet.Cells[i, colIndex++].Value = details[i - 1].Category;
        worksheet.Cells[i, colIndex].ColumnWidthInCharacters = 20;
        worksheet.Cells[i, colIndex++].Value = details[i - 1].IdNumber;
        worksheet.Cells[i, colIndex].ColumnWidthInCharacters = 30;
        worksheet.Cells[i, colIndex++].Value = details[i - 1].FirstName;
        worksheet.Cells[i, colIndex].ColumnWidthInCharacters = 30;
        worksheet.Cells[i, colIndex++].Value = details[i - 1].LastName;
      }

      for (var i = 1; i <= details.Count; i++)
      {
        for (var j = 0; j < colIndex; j++)
        {
          worksheet.Cells[i, j].Borders.SetAllBorders(System.Drawing.Color.Black, BorderLineStyle.Thin);
        }
      }
    }

    private STR_Performance TotalPerformanceReport(List<STR_Performance> consolidatedReport)
    {
      var total = new STR_Performance
      {
        IsTotal = true
      };

      consolidatedReport.ForEach(reportLine =>
      {
        if (!reportLine.IsTotal)
        {
          //total.Accounts += reportLine.Accounts;
          //total.Debtors += reportLine.Debtors;
          total.NoActionCount += reportLine.NoActionCount;
          total.NotInterested += reportLine.NotInterested;
          total.SMSCount += reportLine.SMSCount;
          total.TransferredIn += reportLine.TransferredIn;
          total.TransferredOut += reportLine.TransferredOut;
          total.Escalations += reportLine.Escalations;
          //total.Cases += reportLine.Cases;
          total.SystemClosedCases += reportLine.SystemClosedCases;
          total.ForceClosedCases += reportLine.ForceClosedCases;
          //total.WorkableCases += reportLine.WorkableCases;
          total.CurrentAccounts += reportLine.CurrentAccounts;
          total.CurrentClients += reportLine.CurrentClients;
          total.CurrentCases += reportLine.CurrentCases;
          total.FollowUps += reportLine.FollowUps;
          total.PtpPtcBroken += reportLine.PtpPtcBroken;
          total.PtpPtcHitRate += reportLine.PtpPtcHitRate;
          total.PtpPtcHitRateSuccessful += reportLine.PtpPtcHitRateSuccessful;
          total.PtpPtcObtained += reportLine.PtpPtcObtained;
          total.PtpPtcSuccessful += reportLine.PtpPtcSuccessful;
        }
      });

      if (consolidatedReport.Count > 0)
      {
        total.PtpPtcHitRate = total.PtpPtcHitRate / consolidatedReport.Count;
        total.PtpPtcHitRateSuccessful = total.PtpPtcHitRateSuccessful / consolidatedReport.Count;
      }

      return total;
    }

    private void PopulatePerformanceWorksheet(ref Worksheet worksheet, List<STR_Performance> performanceReport,
      DateTime rangeStartDate, DateTime rangeEndDate, Framework.Enumerators.Stream.GroupType groupType)
    {
      performanceReport =
        performanceReport.OrderBy(p => p.IsTotal ? 999 : 0)
          .ThenBy(p => p.Region)
          .ThenBy(p => p.Branch)
          .ThenBy(p => p.AllocatedUser)
          .ThenBy(p => p.Category)
          .ThenBy(p => p.SubCategory)
          .ToList();

      // build Header
      worksheet.Cells[0, 0].Value = string.Format("Stream Performance report for {0} - {1}",
        rangeStartDate.ToString("dd MMM yyyy"), rangeEndDate.ToString("dd MMM yyyy"));
      worksheet.Rows[0].Font.Bold = true;
      worksheet.Rows[0].FillColor = System.Drawing.Color.LightSteelBlue;
      worksheet.Rows[0].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
      worksheet.Rows[0].Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
      worksheet.Rows[0].Borders.SetAllBorders(System.Drawing.Color.Black, BorderLineStyle.Thin);
      worksheet.MergeCells(worksheet.Range["A1:G1"]);

      var header = BuildPerformanceReportHeader(groupType);
      worksheet.Import(header, 1, 0, false);
      worksheet.Rows[1].Height = 100;
      worksheet.Rows[1].Font.Bold = true;
      worksheet.Rows[1].FillColor = System.Drawing.Color.LightSteelBlue;
      worksheet.Rows[1].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
      worksheet.Rows[1].Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
      worksheet.Rows[1].Alignment.WrapText = true;
      worksheet.Rows[1].Borders.SetAllBorders(System.Drawing.Color.Black, BorderLineStyle.Thin);

      for (var i = 2; i < (performanceReport.Count + 2); i++)
      {
        var colIndex = 0;
        worksheet.Cells[i, colIndex].Value = performanceReport[i - 2].Region;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 14;
        worksheet.Cells[i, colIndex].Value = performanceReport[i - 2].Branch;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 14;
        worksheet.Cells[i, colIndex].Value = performanceReport[i - 2].AllocatedUser;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 14;
        worksheet.Cells[i, colIndex].Value = performanceReport[i - 2].Category;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 14;
        //worksheet.Cells[i, colIndex].Value = performanceReport[i - 2].SubCategory;
        //worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 14;
        //worksheet.Cells[i, colIndex].Value = (float)performanceReport[i - 2].Accounts;
        //worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        //worksheet.Cells[i, colIndex].Value = (float)performanceReport[i - 2].Debtors;
        //worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        //worksheet.Cells[i, colIndex].Value = (float)performanceReport[i - 2].Cases;
        //worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = (float)performanceReport[i - 2].SystemClosedCases;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = (float)performanceReport[i - 2].ForceClosedCases;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        //worksheet.Cells[i, colIndex].Value = (float)performanceReport[i - 2].WorkableCases; // requested by phillip: 1 June 2016
        //worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = (float)performanceReport[i - 2].CurrentAccounts;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = (float)performanceReport[i - 2].CurrentClients;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = (float)performanceReport[i - 2].CurrentCases;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = (float)performanceReport[i - 2].PtpPtcObtained;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = (float)performanceReport[i - 2].PtpPtcBroken;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = (float)performanceReport[i - 2].PtpPtcSuccessful;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = (float)performanceReport[i - 2].FollowUps;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = (float)performanceReport[i - 2].NoActionCount;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = (float)performanceReport[i - 2].NotInterested;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = performanceReport[i - 2].PtpPtcHitRate;
        worksheet.Cells[i, colIndex++].NumberFormat = "0.00%";
        worksheet.Cells[i, colIndex].Value = performanceReport[i - 2].PtpPtcHitRateSuccessful;
        worksheet.Cells[i, colIndex++].NumberFormat = "0.00%";
        worksheet.Cells[i, colIndex].Value = (float)performanceReport[i - 2].Escalations;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = (float)performanceReport[i - 2].TransferredIn;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = (float)performanceReport[i - 2].TransferredOut;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = (float)performanceReport[i - 2].SMSCount;
        worksheet.Cells[i, colIndex].NumberFormat = "#####";

        if (performanceReport[i - 2].IsTotal)
        {
          // italic text
          worksheet.Rows[i].Font.Italic = true;
          worksheet.Rows[i].Font.Color = System.Drawing.Color.DodgerBlue;
          worksheet.Rows[i].Font.Bold = true;
          worksheet.Rows[i].FillColor = System.Drawing.Color.LightSteelBlue;
        }
        worksheet.Rows[i].Borders.SetAllBorders(System.Drawing.Color.Black, BorderLineStyle.Thin);
      }
    }

    private List<string> BuildPerformanceReportHeader(Framework.Enumerators.Stream.GroupType groupType)
    {
      return BuildHeader<STR_Performance>(groupType);
    }

    private static List<string> BuildHeader<T>(Framework.Enumerators.Stream.GroupType groupType)
    {
      var header = new List<string>();

      foreach (var property in typeof(T).GetProperties())
      {
        var descriptionAttributes =
          (DescriptionAttribute[])property.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (descriptionAttributes.Length > 0)
        {
          var tempHeader = descriptionAttributes[0].Description;
          if (groupType == Framework.Enumerators.Stream.GroupType.Sales)
          {
            tempHeader = tempHeader.Replace("PTP", "PTC");
          }
          header.Add(tempHeader);
        }
      }

      return header;
    }

    private List<string> BuildAccountsExportHeader()
    {
      var header = new List<string>
      {
        GetDescription(typeof (STR_AccountsExport), "CaseId"),
        GetDescription(typeof (STR_AccountsExport), "AccountReference"),
        GetDescription(typeof (STR_AccountsExport), "ClientName"),
        GetDescription(typeof (STR_AccountsExport), "IdNumber"),
        GetDescription(typeof (STR_AccountsExport), "Priority"),
        GetDescription(typeof (STR_AccountsExport), "Status"),
        GetDescription(typeof (STR_AccountsExport), "AllocatedUser")
      };


      return header;
    }

    private List<string> BuildDetailExportHeader()
    {
      var header = new List<string>
      {
        GetDescription(typeof (STR_Detail), "CaseNo"),
        GetDescription(typeof (STR_Detail), "CaseStatus"),
        GetDescription(typeof (STR_Detail), "Stream"),
        GetDescription(typeof (STR_Detail), "Category"),
        GetDescription(typeof (STR_Detail), "IdNumber"),
        GetDescription(typeof (STR_Detail), "FirstName"),
        GetDescription(typeof (STR_Detail), "LastName")
      };


      return header;
    }

    private void PopulatePerformanceDailyWorksheet(ref Worksheet worksheet,
      List<STR_PerformanceDaily> performanceDailyReport, Framework.Enumerators.Stream.GroupType groupType)
    {
      var header = BuildPerformanceReportDailyHeader(groupType);
      worksheet.Import(header, 0, 0, false);
      // bold text
      for (var j = 0; j < header.Count; j++)
      {
        worksheet.Rows[0].Font.Bold = true;
        worksheet.Rows[0].FillColor = System.Drawing.Color.LightSteelBlue;
        worksheet.Rows[0].Height = 100;
        worksheet.Rows[0].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
        worksheet.Rows[0].Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
        worksheet.Rows[0].Alignment.WrapText = true;
      }

      var colIndex = 0;
      for (var i = 1; i <= performanceDailyReport.Count; i++)
      {
        colIndex = 0;
        worksheet.Cells[i, colIndex++].Value = performanceDailyReport[i - 1].Date.ToString("dd");
        worksheet.Cells[i, colIndex++].Value = performanceDailyReport[i - 1].AllocatedUser;
        worksheet.Cells[i, colIndex].Value = (float)performanceDailyReport[i - 1].PtpPtcObtained;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = (float)performanceDailyReport[i - 1].FollowUps;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = (float)performanceDailyReport[i - 1].NoActionCount;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = (float)performanceDailyReport[i - 1].Escalations;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
      }

      for (var i = 0; i <= performanceDailyReport.Count; i++)
      {
        for (var j = 0; j < colIndex; j++)
        {
          worksheet.Cells[i, j].Borders.SetAllBorders(System.Drawing.Color.Black, BorderLineStyle.Thin);
        }
      }
    }

    private List<string> BuildPerformanceReportDailyHeader(Framework.Enumerators.Stream.GroupType groupType)
    {
      var header = new List<string>
      {
        GetDescription(typeof (STR_PerformanceDaily), "Date"),
        GetDescription(typeof (STR_PerformanceDaily), "AllocatedUser"),
        string.Format(GetDescription(typeof (STR_PerformanceDaily), "PtpPtcObtained"),
          groupType == Framework.Enumerators.Stream.GroupType.Collections ? "PTPs" : "PTCs"),
        GetDescription(typeof (STR_PerformanceDaily), "FollowUps"),
        GetDescription(typeof (STR_PerformanceDaily), "NoActionCount"),
        GetDescription(typeof (STR_PerformanceDaily), "Escalations")
      };

      return header;
    }

    private string GetDescription(Type type, string propertyName)
    {
      var descriptions =
        (DescriptionAttribute[])
          type.GetProperty(propertyName).GetCustomAttributes(typeof(DescriptionAttribute), false);

      if (descriptions.Length == 0)
      {
        return string.Empty;
      }
      return descriptions[0].Description;
    }

    private List<STR_PerformanceDaily> UserDailyPerformanceReportFillInBlanks(List<STR_PerformanceDaily> report,
      DateTime startDate, DateTime endDate)
    {
      var dailyReport = new List<STR_PerformanceDaily>();
      for (var day = 0; day <= (endDate.Date - startDate.Date).TotalDays; day++)
      {
        var reportDate = startDate.Date.AddDays(day);

        var daily = report.FirstOrDefault(r => r.Date == reportDate.Date) ?? new STR_PerformanceDaily
        {
          Date = reportDate,
          Escalations = 0,
          FollowUps = 0,
          PtpPtcObtained = 0,
          NoActionCount = 0
        };

        dailyReport.Add(daily);
      }
      return dailyReport.OrderBy(d => d.Date).ToList();
    }

    #endregion

    #endregion
  }
}
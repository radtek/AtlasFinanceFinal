using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Atlas.Ass.Structures;
using DevExpress.Spreadsheet;
using DevExpress.Xpo;
using Serilog;
using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Falcon.Common.Structures.Report.Ass;
using Falcon.Service.Core;

namespace Falcon.Service.Business.Reporting
{
  public class CiExtract
  {
    private const string ExcelFmtCurrency = "#,##0;-#,##0";
    private const string ExcelFmtPercent = "0.00%";

    private List<Tuple<string, decimal, decimal, float>> _branch80PercLessTarget =
      new List<Tuple<string, decimal, decimal, float>>();

    private PossibleHandoversFile GetPossibleHandoverInfo(string branchName, string branchNumber)
    {
      var fileLine = new PossibleHandoversFile();
      var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
      var endDate = startDate.AddMonths(1).AddDays(-1);

      var loansFlagged =
        RedisConnection.GetObjectFromString<List<LoansFlagged>>(string.Format(AssReporting.REDIS_KEY_LOANS_FLAGGED,
          branchNumber)) ?? new List<LoansFlagged>();
      var possibleHandovers =
        RedisConnection.GetObjectFromString<List<PossibleHandover>>(
          string.Format(AssReporting.REDIS_KEY_POSSIBLE_HANDOVERS, branchNumber)) ?? new List<PossibleHandover>();
      var arrears =
        RedisConnection.GetObjectFromString<List<Arrears>>(string.Format(AssReporting.REDIS_KEY_ARREARS, branchNumber)) ??
        new List<Arrears>();
      var collections =
        RedisConnection.GetObjectFromString<List<Collections>>(string.Format(AssReporting.REDIS_KEY_COLLECTIONS,
          branchNumber)) ?? new List<Collections>();
      var debtorsBook =
        RedisConnection.GetObjectFromString<List<DebtorsBook>>(string.Format(AssReporting.REDIS_KEY_DEBTORS_BOOK,
          branchNumber)) ?? new List<DebtorsBook>();
      var target =
        RedisConnection.GetObjectFromString<TAR_HandoverTargetDTO>(string.Format(AssReporting.REDIS_KEY_BUDGETS,
          branchNumber, startDate.ToString("ddMMyyyy"), endDate.ToString("ddMMyyyy"))) ?? new TAR_HandoverTargetDTO();

      fileLine.ActualArrears = arrears.Sum(p => p.ArrearsValue);
      fileLine.Branch = branchName;
      if (collections.Sum(c => c.ReceivablePast) != 0)
        fileLine.CollectionsPrevMonth = collections.Sum(c => c.ReceivedPast) /
                                        collections.Sum(c => c.ReceivablePast);
      if (collections.Count > 0)
        fileLine.OldestArrearDate = collections.Where(p => p.OldestArrearDate.HasValue).Min(p => p.OldestArrearDate);
      if (collections.Sum(c => c.ReceivableThisMonth) != 0)
        fileLine.CollectionsThisMonth = collections.Sum(c => c.ReceivedThisMonth) /
                                        collections.Sum(c => c.ReceivableThisMonth);
      fileLine.DebtorBook = debtorsBook.Sum(d => d.DebtorsBookValue);
      fileLine.HandoverBudget = target.HandoverBudget;
      fileLine.HandOverComputer = possibleHandovers.Sum(p => p.PossibleHandOvers);
      fileLine.HandoverValueNextMonth = possibleHandovers.Sum(p => p.NextPossibleHandOvers);
      fileLine.NextMonthHandoverForBreakEven = fileLine.HandoverValueNextMonth - fileLine.HandoverBudget;
      fileLine.ShortFall = fileLine.HandoverBudget - fileLine.HandOverComputer;
      fileLine.ArrearsTarget = fileLine.DebtorBook * (Decimal)target.ArrearTarget;
      fileLine.FlaggedLoans = loansFlagged.Sum(f => f.NoOfLoans);
      fileLine.FlaggedLoansOverdue = loansFlagged.Sum(f => f.OverdueValue);

      if (fileLine.DebtorBook != 0)
        fileLine.PercentToDebtorsBook = (float)fileLine.ActualArrears / (float)fileLine.DebtorBook;
      if (fileLine.HandoverBudget != 0)
        fileLine.ActualVsBudget = (float)fileLine.HandOverComputer / (float)fileLine.HandoverBudget;

      return fileLine;
    }


    public byte[] ExportCiReport(List<long> branchIds, DateTime startDate, DateTime endDate, ILogger log,
      bool exportPossibleHandovers)
    {
      using (var workbook = new Workbook())
      {
        try
        {
          var consolidatedCiInfo = new List<CiFile>();
          var consolidatedCiScores = new List<CiScoreBand>();
          var consolidatedPossibleHandoverInfo = new List<PossibleHandoversFile>();
          var ciBranchSummaries = new List<CIBranchSummary>();
          var branchSyncData = new List<Tuple<DateTime, string, long>>();
          var regions = GetRegionNamesByBranchIds(branchIds);
          var idx = 1;

          foreach (var region in regions)
          {
            var regionConsolidationCiInfo = new List<CiFile>();
            var regionConsolidationCiScores = new List<CiScoreBand>();
            var regionConsolidationPossibleHandoverInfo = new List<PossibleHandoversFile>();
            var branches = GetLegacyBranchNumbers(region.Key, branchIds);

            #region Branch Sync Data

            branchSyncData.AddRange(GetBranchSyncData(branches.Select(b => b.Value.Item1).ToArray()));

            #endregion

            foreach (var branch in branches)
            {
              #region Ci Info

              var ciInfo = GetCiInfo(branch.Value.Item1, branch.Key, startDate, endDate);
              var ciScores = GetScoreBands(branch.Value.Item1, startDate, endDate);
              consolidatedCiInfo = Consolidate(consolidatedCiInfo, ciInfo);
              consolidatedCiScores = Consolidate(consolidatedCiScores, ciScores);
              regionConsolidationCiInfo = Consolidate(regionConsolidationCiInfo, ciInfo);
              regionConsolidationCiScores = Consolidate(regionConsolidationCiScores, ciScores);

              #endregion

              #region Possible Handover

              var possibleHandoverInfo = GetPossibleHandoverInfo(branch.Value.Item2, branch.Value.Item1);
              regionConsolidationPossibleHandoverInfo.Add(possibleHandoverInfo);

              #endregion

              var branchTotal = TotalConsolidation(ciInfo);
              // add closed branches only
              if (!branch.Value.Item3)
              {
                // brace for keith, and because im starting to like the brace for single line if statements
                Determine80PercLessBranch(branchTotal, branch.Value.Item2);
              }
              ciBranchSummaries.Add(new CIBranchSummary
              {
                BranchName = branch.Value.Item2,
                BranchId = branch.Key,
                Cheque = branchTotal.Cheque,
                Collections = branchTotal.Collections,
                HandoverTotal = branchTotal.HandoverTotal
              });

              if (regions.Count == 1)
              {
                ciInfo.Add(TotalConsolidation(ciInfo));
                ciScores.Add(TotalConsolidation(ciScores));
                ciInfo = FinaliseConsolidation(ciInfo);

                var branchWorksheet =
                  workbook.Worksheets.Add(
                    branch.Value.Item2.Replace(':', ' ')
                      .Replace('\\', ' ')
                      .Replace('/', ' ')
                      .Replace('?', ' ')
                      .Replace('[', '(')
                      .Replace(']', ')'));

                PopulateCiReportWorksheet(branchWorksheet, ciInfo, startDate, endDate, ciScores, null);
              }
            }

            #region finalize Region

            #region Ci Info

            regionConsolidationCiInfo.Add(TotalConsolidation(regionConsolidationCiInfo));
            regionConsolidationCiScores.Add(TotalConsolidation(regionConsolidationCiScores));
            regionConsolidationCiInfo = FinaliseConsolidation(regionConsolidationCiInfo);

            #endregion

            #region Possible Handover

            var totalRegion = TotalConsolidation(region.Value, regionConsolidationPossibleHandoverInfo);
            consolidatedPossibleHandoverInfo.Add(totalRegion);
            regionConsolidationPossibleHandoverInfo.Add(totalRegion);

            #endregion

            #endregion

            // add to workbook
            var regionWorksheet = workbook.Worksheets.Insert(idx++,
              region.Value.Replace(':', ' ')
                .Replace('\\', ' ')
                .Replace('/', ' ')
                .Replace('?', ' ')
                .Replace('[', '(')
                .Replace(']', ')'));

            PopulateCiReportWorksheet(regionWorksheet, regionConsolidationCiInfo, startDate, endDate,
              regionConsolidationCiScores, exportPossibleHandovers ? regionConsolidationPossibleHandoverInfo : null);
          }

          if (regions.Count == 1)
          {
            workbook.Worksheets.RemoveAt(0);
          }
          else
          {
            workbook.Worksheets[0].Name = "Consolidated";

            #region finalize Region

            #region CI Info

            consolidatedCiInfo.Add(TotalConsolidation(consolidatedCiInfo));
            consolidatedCiScores.Add(TotalConsolidation(consolidatedCiScores));
            consolidatedCiInfo = FinaliseConsolidation(consolidatedCiInfo);

            #endregion

            #region Possible Handover

            consolidatedPossibleHandoverInfo.Add(TotalConsolidation("Total", consolidatedPossibleHandoverInfo));

            #endregion

            #endregion

            var consolidatedWorksheet = workbook.Worksheets[0];
            PopulateCiReportWorksheet(consolidatedWorksheet, consolidatedCiInfo, startDate, endDate,
              consolidatedCiScores, exportPossibleHandovers ? consolidatedPossibleHandoverInfo : null);
          }

          // closed region
          var syncBranchWorksheet = regions.ContainsKey(14) && workbook.Worksheets.Count >= 1
            ? workbook.Worksheets.Insert(workbook.Worksheets.Count - 1, "Branch Update Sync")
            : workbook.Worksheets.Add("Branch Update Sync");
          PopulateLastSyncUpdateWorksheet(syncBranchWorksheet, branchSyncData);

          // branch summary
          var branchSummaryWorksheet = workbook.Worksheets.Add("Branch Summary");
          PopulateCiBranchSummaryWorksheet(branchSummaryWorksheet, ciBranchSummaries);

          // 80 % Less Target
          var branch80PercLessTargetWorksheet = workbook.Worksheets.Insert(0, "Below 80%");
          Populate80PercLessWorkSheet(branch80PercLessTargetWorksheet);
          workbook.Worksheets.ActiveWorksheet = workbook.Worksheets[0];

          workbook.EndUpdate();
          return workbook.SaveDocument(DocumentFormat.Xlsx);
        }
        catch (Exception ex)
        {
          log.Error(string.Format("CI REPORT GET CI EXPORT: {0} - {1}", ex.Message, ex.StackTrace));
          return null;
        }
      }
    }

    private void PopulateCiBranchSummaryWorksheet(Worksheet worksheet, List<CIBranchSummary> branchSummaries)
    {
      worksheet.Import(BuildBranchSummaryHeader(), 0, 0, false);

      for (var i = 1; i <= branchSummaries.Count; i++)
      {
        var colIndex = 0;

        worksheet.Cells[i, colIndex].Value = branchSummaries[i - 1].BranchName;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 14;

        worksheet.Cells[i, colIndex].Value = (float)branchSummaries[i - 1].Cheque;
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, colIndex].Font.Color = Color.Black;
        worksheet.Cells[i, colIndex].FillColor = Color.LightGray;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;

        worksheet.Cells[i, colIndex].Value = (float)branchSummaries[i - 1].HandoverTotal;
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, colIndex].Font.Color = Color.Black;
        worksheet.Cells[i, colIndex].FillColor = Color.LightBlue;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;

        worksheet.Cells[i, colIndex].Value = (float)branchSummaries[i - 1].Collections;
        worksheet.Cells[i, colIndex].Font.Color = Color.Black;
        worksheet.Cells[i, colIndex].FillColor = ColorTranslator.FromHtml("#D4E9B9");
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, colIndex].ColumnWidthInCharacters = 15;
      }
    }

    private void PopulateCiReportWorksheet(Worksheet worksheet, List<CiFile> ciInfo, DateTime rangeStartDate,
      DateTime rangeEndDate,
      List<CiScoreBand> ciScores, List<PossibleHandoversFile> possibleHandoverInfo)
    {
      var ciFile = ciInfo.FirstOrDefault(p => p.PayNo == 0);
      if (ciFile != null)
        worksheet.Import(BuildCiHeader(ciFile.CompuscanProducts.Keys.ToList()), 0, 0, false);
      else
        throw new Exception("Cannot build CI header");

      for (var i = 1; i <= ciInfo.Count; i++)
      {
        var colIndex = 0;
        worksheet.Cells[i, colIndex].Value = ciInfo[i - 1].LoanMeth;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 14;
        worksheet.Cells[i, colIndex].Value = ciInfo[i - 1].PayNo;
        worksheet.Cells[i, colIndex].NumberFormat = "00";
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 11;
        worksheet.Cells[i, colIndex++].Value = ciInfo[i - 1].QuantitySFee;

        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].Cheque;
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, colIndex].Font.Color = Color.Black;
        worksheet.Cells[i, colIndex].FillColor = Color.LightGray;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;

        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].Target;
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;

        worksheet.Cells[i, colIndex].Value = ciInfo[i - 1].TargetPercent;
        worksheet.Cells[i, colIndex++].NumberFormat = ExcelFmtPercent;

        //worksheet.Cells[i, colIndex].Value = ciInfo[i - 1].Actual; // i dont understand it, but the want the loan mix twice with a different column name
        worksheet.Cells[i, colIndex].Value = ciInfo[i - 1].LoanMix;
        worksheet.Cells[i, colIndex].Font.Color = Color.Black;
        worksheet.Cells[i, colIndex].FillColor = Color.LightGray;
        worksheet.Cells[i, colIndex++].NumberFormat = ExcelFmtPercent;

        //worksheet.Cells[i, colIndex].Value = ciInfo[i - 1].Deviation;
        worksheet.Cells[i, colIndex].Value = ciInfo[i - 1].LoanMix - ciInfo[i - 1].TargetPercent;
        worksheet.Cells[i, colIndex].Font.Color = Color.Black;
        worksheet.Cells[i, colIndex].FillColor = Color.Moccasin;
        worksheet.Cells[i, colIndex++].NumberFormat = ExcelFmtPercent;

        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].ChargesExclVAT;
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].ChargesVAT;
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].TotalCharges;
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[i, colIndex].Value = ciInfo[i - 1].ChargesPercent;
        worksheet.Cells[i, colIndex++].NumberFormat = ExcelFmtPercent;

        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].CreditLife;
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[i, colIndex].Value = ciInfo[i - 1].CreditLifePercent;
        worksheet.Cells[i, colIndex++].NumberFormat = ExcelFmtPercent;

        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].LoanFeeExclVAT;
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].LoanFeeVAT;
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].LoanFeeInclVAT;
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[i, colIndex].Value = ciInfo[i - 1].LoanFeePercent;
        worksheet.Cells[i, colIndex++].NumberFormat = ExcelFmtPercent;

        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].FuneralAddOn;
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].AgeAddOn;
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].VAPExcl;
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].VAPVAT;
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].VAPIncl;
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].TotalAddOn;
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[i, colIndex].Value = ciInfo[i - 1].AddOnPercent;
        worksheet.Cells[i, colIndex++].NumberFormat = ExcelFmtPercent;

        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].TotFeeExcl;
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].TotFeeVAT;
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].TotFeeIncl;
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[i, colIndex].Value = ciInfo[i - 1].TotFeePercent;
        worksheet.Cells[i, colIndex++].NumberFormat = ExcelFmtPercent;

        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].HandoverTotal;
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, colIndex].Font.Color = Color.Black;
        worksheet.Cells[i, colIndex].FillColor = Color.LightBlue;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[i, colIndex].Value = ciInfo[i - 1].HandoverPercent;
        worksheet.Cells[i, colIndex++].NumberFormat = ExcelFmtPercent;
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].Loans;
        worksheet.Cells[i, colIndex].Font.Color = Color.Black;
        worksheet.Cells[i, colIndex].FillColor = Color.LightGray;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = ciInfo[i - 1].LoanMix;
        worksheet.Cells[i, colIndex].Font.Color = Color.Black;
        worksheet.Cells[i, colIndex].FillColor = Color.LightGray;
        worksheet.Cells[i, colIndex++].NumberFormat = ExcelFmtPercent;
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].VapLinkedLoans;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].VapDeniedByConWithAuth;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].VapDeniedByConWithOutAuth;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].VapExcludedLoans;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].Collections;
        worksheet.Cells[i, colIndex].Font.Color = Color.Black;
        worksheet.Cells[i, colIndex].FillColor = ColorTranslator.FromHtml("#D4E9B9");
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].RolledValue;
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[i, colIndex].Value = ciInfo[i - 1].RolledPercentage;
        worksheet.Cells[i, colIndex].Font.Color = Color.Black;
        worksheet.Cells[i, colIndex].FillColor = Color.LightGray;
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtPercent;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].Refunds;
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].ChequeClientsNew;
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].HandoverNoOfLoans;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].HandoverNoOfClients;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].ClientBranch;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].ClientSalesRep;
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].TotalNewClients;
        worksheet.Cells[i, colIndex].Font.Color = Color.Black;
        worksheet.Cells[i, colIndex].FillColor = ColorTranslator.FromHtml("#F0D59C");
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].ExistingClients;
        worksheet.Cells[i, colIndex].Font.Color = Color.Black;
        worksheet.Cells[i, colIndex].FillColor = ColorTranslator.FromHtml("#F0D59C");
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].RevivedClients;
        worksheet.Cells[i, colIndex].Font.Color = Color.Black;
        worksheet.Cells[i, colIndex].FillColor = ColorTranslator.FromHtml("#F0D59C");
        worksheet.Cells[i, colIndex++].NumberFormat = "#####";
        worksheet.Cells[i, colIndex].Value = ciInfo[i - 1].NewClientMix;
        worksheet.Cells[i, colIndex++].NumberFormat = ExcelFmtPercent;
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].AverageLoan;
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].NewLoanAverage;
        worksheet.Cells[i, colIndex].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].ReswipeBankChange;
        worksheet.Cells[i, colIndex].NumberFormat = "#####";
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].ReswipeTermChange;
        worksheet.Cells[i, colIndex].NumberFormat = "#####";
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[i, colIndex].Value = (float)ciInfo[i - 1].ReswipeInstalmentChange;
        worksheet.Cells[i, colIndex].NumberFormat = "#####";
        worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;

        foreach (var p in ciInfo[i - 1].CompuscanProducts)
        {
          worksheet.Cells[i, colIndex].Value = (float)p.Value;
          worksheet.Cells[i, colIndex].NumberFormat = "#####";
          worksheet.Cells[i, colIndex++].ColumnWidthInCharacters = 15;
        }

        if (i == ciInfo.Count)
        {
          for (var j = 0; j <= colIndex; j++)
            worksheet.Cells[i, j].Font.Bold = true;
          worksheet.Rows.Insert(0);
          worksheet.Rows.Insert(1);
          worksheet.Cells[0, 0].Value = string.Format("Report extracted on {0} - For Period {1} - {2}",
            DateTime.Now.ToString("dd MMM yyyy HH:mm:ss"), rangeStartDate.ToString("dd MMM yyyy"),
            rangeEndDate.ToString("dd MMM yyyy"));
          worksheet.MergeCells(worksheet.Range["A1:H1"]);
          for (var h = 0; h <= (ciInfo.Count + 2); h++)
            for (var k = 0; k < colIndex; k++)
              worksheet.Cells[h, k].Borders.SetAllBorders(Color.Black, BorderLineStyle.Thin);

          worksheet.Cells[12, 2].Value = "Variance :";
          worksheet.Cells[12, 2].Font.Bold = true;
          worksheet.Cells[12, 2].Borders.LeftBorder.Color = Color.Black;
          worksheet.Cells[12, 2].Borders.BottomBorder.Color = Color.Black;

          worksheet.Cells[12, 3].Value = (float)(ciInfo[i - 1].Cheque - ciInfo[i - 1].Target);
          worksheet.Cells[12, 3].Font.Bold = true;
          worksheet.Cells[12, 3].NumberFormat = ExcelFmtCurrency;
          worksheet.Cells[12, 3].FillColor = (ciInfo[i - 1].Target - ciInfo[i - 1].Cheque) < 0
            ? Color.LightGreen
            : Color.IndianRed;
          worksheet.Cells[12, 3].Borders.LeftBorder.Color = Color.Black;
          worksheet.Cells[12, 3].Borders.BottomBorder.Color = Color.Black;

          worksheet.Cells[12, 4].Value = ciInfo[i - 1].Target != 0
            ? (float)(ciInfo[i - 1].Cheque / ciInfo[i - 1].Target)
            : 0;
          worksheet.Cells[12, 4].NumberFormat = ExcelFmtPercent;
          worksheet.Cells[12, 4].FillColor = ciInfo[i - 1].Cheque > ciInfo[i - 1].Target
            ? Color.LightGreen
            : Color.IndianRed;
          worksheet.Cells[12, 4].Font.Bold = true;
          worksheet.Cells[12, 4].Font.UnderlineType = UnderlineType.Double;
          worksheet.Cells[12, 4].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Right;
          worksheet.Cells[12, 4].Borders.RightBorder.Color = Color.Black;
          worksheet.Cells[12, 4].Borders.BottomBorder.Color = Color.Black;
          worksheet.Rows[12].RowHeight = 100;
        }
      }

      // A16
      var ciScoreStartRow = 15;
      worksheet.Import(BuildCiScoreHeader(), ciScoreStartRow, 0, false);
      for (var j = 0; j < 5; j++)
      {
        worksheet.Cells[ciScoreStartRow, j].Font.Bold = true;
        worksheet.Cells[ciScoreStartRow, j].Borders.SetAllBorders(Color.Black, BorderLineStyle.Medium);
      }

      ciScoreStartRow++;
      for (var i = 0; i < ciScores.Count; i++)
      {
        var colIndex = 0;
        worksheet.Cells[ciScoreStartRow, colIndex].Value = ciScores[i].PayNo;
        worksheet.Cells[ciScoreStartRow, colIndex].NumberFormat = "00";
        worksheet.Cells[ciScoreStartRow, colIndex++].ColumnWidthInCharacters = 11;
        worksheet.Cells[ciScoreStartRow, colIndex].Value = (float)ciScores[i].Weekly615;
        worksheet.Cells[ciScoreStartRow, colIndex].NumberFormat = "#####";
        worksheet.Cells[ciScoreStartRow, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[ciScoreStartRow, colIndex].Value = (float)ciScores[i].BiWeekly615;
        worksheet.Cells[ciScoreStartRow, colIndex].NumberFormat = "#####";
        worksheet.Cells[ciScoreStartRow, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[ciScoreStartRow, colIndex].Value = (float)ciScores[i].Monthly615;

        worksheet.Cells[ciScoreStartRow, colIndex].NumberFormat = "#####";
        worksheet.Cells[ciScoreStartRow, colIndex++].ColumnWidthInCharacters = 15;
        worksheet.Cells[ciScoreStartRow, colIndex].Value = (float)ciScores[i].Total615;
        worksheet.Cells[ciScoreStartRow, colIndex].NumberFormat = "#####";
        worksheet.Cells[ciScoreStartRow, colIndex++].ColumnWidthInCharacters = 15;

        for (var j = 0; j < colIndex; j++)
          worksheet.Cells[ciScoreStartRow, j].Borders.SetAllBorders(Color.Black,
            (i + 1) == ciScores.Count ? BorderLineStyle.Medium : BorderLineStyle.Thin);
        if ((i + 1) == ciScores.Count)
        {
          for (var j = 0; j < colIndex; j++)
            worksheet.Cells[ciScoreStartRow, j].Font.Bold = true;
        }

        ciScoreStartRow++;
      }

      if (possibleHandoverInfo != null)
      {
        worksheet.Cells["A30"].Value = string.Format("Possible Handovers for: {0}",
          DateTime.Today.ToString("dd MMM yyyy"));
        worksheet.MergeCells(worksheet.Range["A30:F30"]);
        worksheet.Cells["A31"].Value = "Sales";
        worksheet.Cells["A31"].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
        worksheet.Cells["C31"].Value = "Possible Handovers";
        worksheet.Cells["C31"].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
        worksheet.MergeCells(worksheet.Range["B31:F31"]);
        worksheet.Cells["I31"].Value = "Arrears";
        worksheet.Cells["I31"].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
        worksheet.MergeCells(worksheet.Range["H31:K31"]);
        worksheet.Cells["M31"].Value = "Collections % to date";
        worksheet.Cells["M31"].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
        worksheet.MergeCells(worksheet.Range["M31:N31"]);
        worksheet.Cells["O31"].Value = "Flagged";
        worksheet.Cells["O31"].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
        worksheet.MergeCells(worksheet.Range["O31:P31"]);
        var consolidatedRange = worksheet.Range["A30:P33"];
        consolidatedRange.Borders.SetAllBorders(Color.Black, BorderLineStyle.Medium);
        consolidatedRange.Font.Bold = true;

        var startRow = 31;
        worksheet.MergeCells(worksheet.Range["A32:B33"]);
        worksheet.Cells["A32"].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
        worksheet.Cells["A32"].Alignment.WrapText = true;
        worksheet.MergeCells(worksheet.Range["C32:C33"]);
        worksheet.Cells["C32"].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
        worksheet.Cells["C32"].Alignment.WrapText = true;
        worksheet.MergeCells(worksheet.Range["D32:D33"]);
        worksheet.Cells["D32"].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
        worksheet.Cells["D32"].Alignment.WrapText = true;
        worksheet.MergeCells(worksheet.Range["E32:E33"]);
        worksheet.Cells["E32"].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
        worksheet.Cells["E32"].Alignment.WrapText = true;
        worksheet.MergeCells(worksheet.Range["F32:F33"]);
        worksheet.Cells["F32"].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
        worksheet.Cells["F32"].Alignment.WrapText = true;
        worksheet.MergeCells(worksheet.Range["G32:G33"]);
        worksheet.Cells["G32"].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
        worksheet.Cells["G32"].Alignment.WrapText = true;
        worksheet.MergeCells(worksheet.Range["H32:H33"]);
        worksheet.Cells["H32"].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
        worksheet.Cells["H32"].Alignment.WrapText = true;
        worksheet.MergeCells(worksheet.Range["I32:I33"]);
        worksheet.Cells["I32"].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
        worksheet.Cells["I32"].Alignment.WrapText = true;
        worksheet.MergeCells(worksheet.Range["J32:J33"]);
        worksheet.Cells["J32"].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
        worksheet.Cells["J32"].Alignment.WrapText = true;
        worksheet.MergeCells(worksheet.Range["K32:K33"]);
        worksheet.Cells["K32"].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
        worksheet.Cells["K32"].Alignment.WrapText = true;
        worksheet.MergeCells(worksheet.Range["L32:L33"]);
        worksheet.Cells["L32"].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
        worksheet.Cells["L32"].Alignment.WrapText = true;
        worksheet.MergeCells(worksheet.Range["M32:M33"]);
        worksheet.Cells["M32"].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
        worksheet.Cells["M32"].Alignment.WrapText = true;
        worksheet.MergeCells(worksheet.Range["N32:N33"]);
        worksheet.Cells["N32"].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
        worksheet.Cells["N32"].Alignment.WrapText = true;
        worksheet.MergeCells(worksheet.Range["O32:O33"]);
        worksheet.Cells["O32"].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
        worksheet.Cells["O32"].Alignment.WrapText = true;
        worksheet.MergeCells(worksheet.Range["P32:P33"]);
        worksheet.Cells["P32"].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
        worksheet.Cells["P32"].Alignment.WrapText = true;
        for (var m = 0; m < possibleHandoverInfo.Count; m++)
        {
          worksheet.MergeCells(worksheet.Range[string.Format("A{0}:B{0}", 34 + m)]);
        }
        worksheet.Import(
          BuildPossibleHandoversHeader(
            possibleHandoverInfo.Where(p => p.OldestArrearDate.HasValue).Min(p => p.OldestArrearDate)), startRow++, 0,
          false);
        for (var i = 1; i <= possibleHandoverInfo.Count; i++)
        {
          startRow++;
          var colIndex = 0;
          worksheet.Cells[startRow, colIndex++].Value = possibleHandoverInfo[i - 1].Branch;
          worksheet.Cells[startRow, ++colIndex].Value = (float)possibleHandoverInfo[i - 1].HandOverComputer;
          worksheet.Cells[startRow, colIndex].NumberFormat = ExcelFmtCurrency;
          worksheet.Cells[startRow, colIndex++].ColumnWidthInCharacters = 15;


          worksheet.Cells[startRow, colIndex].Value = (float)possibleHandoverInfo[i - 1].HandoverBudget;
          worksheet.Cells[startRow, colIndex].NumberFormat = ExcelFmtCurrency;
          worksheet.Cells[startRow, colIndex++].ColumnWidthInCharacters = 15;
          worksheet.Cells[startRow, colIndex].Value = (float)possibleHandoverInfo[i - 1].ShortFall;
          worksheet.Cells[startRow, colIndex].NumberFormat = ExcelFmtCurrency;
          worksheet.Cells[startRow, colIndex++].ColumnWidthInCharacters = 15;
          worksheet.Cells[startRow, colIndex].Value = possibleHandoverInfo[i - 1].ActualVsBudget;
          worksheet.Cells[startRow, colIndex].NumberFormat = ExcelFmtPercent;
          worksheet.Cells[startRow, colIndex].ColumnWidthInCharacters = 15;
          worksheet.Cells[startRow, colIndex++].FillColor = Color.Pink;
          worksheet.Cells[startRow, colIndex].Value = (float)possibleHandoverInfo[i - 1].HandoverValueNextMonth;
          worksheet.Cells[startRow, colIndex].NumberFormat = ExcelFmtCurrency;
          worksheet.Cells[startRow, colIndex++].ColumnWidthInCharacters = 15;
          worksheet.Cells[startRow, colIndex].Value = (float)possibleHandoverInfo[i - 1].NextMonthHandoverForBreakEven;
          worksheet.Cells[startRow, colIndex].NumberFormat = ExcelFmtCurrency;
          worksheet.Cells[startRow, colIndex].ColumnWidthInCharacters = 15;
          worksheet.Cells[startRow, colIndex++].FillColor = Color.Yellow;
          worksheet.Cells[startRow, colIndex].Value = (float)possibleHandoverInfo[i - 1].DebtorBook;
          worksheet.Cells[startRow, colIndex].NumberFormat = ExcelFmtCurrency;
          worksheet.Cells[startRow, colIndex++].ColumnWidthInCharacters = 15;
          worksheet.Cells[startRow, colIndex].Value = (float)possibleHandoverInfo[i - 1].ActualArrears;
          worksheet.Cells[startRow, colIndex].NumberFormat = ExcelFmtCurrency;
          worksheet.Cells[startRow, colIndex++].ColumnWidthInCharacters = 15;
          worksheet.Cells[startRow, colIndex].Value = (float)possibleHandoverInfo[i - 1].ArrearsTarget;
          worksheet.Cells[startRow, colIndex].NumberFormat = ExcelFmtCurrency;
          worksheet.Cells[startRow, colIndex++].ColumnWidthInCharacters = 15;
          worksheet.Cells[startRow, colIndex].Value = possibleHandoverInfo[i - 1].PercentToDebtorsBook;
          worksheet.Cells[startRow, colIndex].NumberFormat = ExcelFmtPercent;
          worksheet.Cells[startRow, colIndex++].ColumnWidthInCharacters = 15;
          worksheet.Cells[startRow, colIndex].Value = (float)possibleHandoverInfo[i - 1].CollectionsThisMonth;
          worksheet.Cells[startRow, colIndex].NumberFormat = ExcelFmtPercent;
          worksheet.Cells[startRow, colIndex++].ColumnWidthInCharacters = 15;
          worksheet.Cells[startRow, colIndex].Value = (float)possibleHandoverInfo[i - 1].CollectionsPrevMonth;
          worksheet.Cells[startRow, colIndex].NumberFormat = ExcelFmtPercent;
          worksheet.Cells[startRow, colIndex++].ColumnWidthInCharacters = 15;
          worksheet.Cells[startRow, colIndex].Value = possibleHandoverInfo[i - 1].FlaggedLoans;
          worksheet.Cells[startRow, colIndex++].ColumnWidthInCharacters = 11;
          worksheet.Cells[startRow, colIndex].Value = (float)possibleHandoverInfo[i - 1].FlaggedLoansOverdue;
          worksheet.Cells[startRow, colIndex].NumberFormat = ExcelFmtCurrency;
          worksheet.Cells[startRow, colIndex++].ColumnWidthInCharacters = 15;

          for (var j = 0; j < colIndex; j++)
            worksheet.Cells[startRow, j].Borders.SetAllBorders(Color.Black,
              i == possibleHandoverInfo.Count ? BorderLineStyle.Medium : BorderLineStyle.Thin);

          if (i == possibleHandoverInfo.Count)
          {
            for (var j = 0; j < colIndex; j++)
              worksheet.Cells[startRow, j].Font.Bold = true;
            worksheet.Rows.Insert(startRow);
          }
        }
      }
    }

    private void Determine80PercLessBranch(CiFile ciInfo, string branchName)
    {
      var variance = ciInfo.Target != 0 ? (float)(ciInfo.Cheque / ciInfo.Target) : 0;
      if (variance < 0.80)
      {
        _branch80PercLessTarget.Add(new Tuple<string, decimal, decimal, float>(branchName, ciInfo.Target,
          ciInfo.Cheque, variance));
      }
    }


    private void PopulateLastSyncUpdateWorksheet(Worksheet worksheet,
      List<Tuple<DateTime, string, long>> branchSyncData)
    {
      worksheet.Import(BuildBranchSyncHeader(), 0, 0, false);
      branchSyncData = branchSyncData.OrderBy(b => b.Item1).ToList();
      for (var i = 1; i <= branchSyncData.Count; i++)
      {
        worksheet.Cells[i, 0].Value = branchSyncData[i - 1].Item2;
        worksheet.Cells[i, 0].ColumnWidthInCharacters = 14;
        worksheet.Cells[i, 1].Value = branchSyncData[i - 1].Item1;
        worksheet.Cells[i, 1].NumberFormat = "dd/MM/yyyy HH:mm:ss";
        worksheet.Cells[i, 1].ColumnWidthInCharacters = 18;
        if (branchSyncData[i - 1].Item3 >= 100)
        {
          worksheet.Cells[i, 0].Font.Color = Color.Red;
          worksheet.Cells[i, 0].Font.Bold = true;
          worksheet.Cells[i, 1].Font.Color = Color.Red;
          worksheet.Cells[i, 1].Font.Bold = true;
        }
      }
    }


    /// <summary>
    /// Adding new excel work sheet to CI Report with populating the list of branches which are achieved less than 80% to targets
    /// Dt: 24.06.2015
    /// </summary>
    private void Populate80PercLessWorkSheet(Worksheet worksheet)
    {
      worksheet.Import(Build80PercLessHeader(), 0, 0, false);
      _branch80PercLessTarget = _branch80PercLessTarget.OrderBy(b => b.Item4).ToList();

      for (var i = 1; i <= _branch80PercLessTarget.Count; i++)
      {
        worksheet.Cells[i, 0].Value = _branch80PercLessTarget[i - 1].Item1;
        worksheet.Cells[i, 0].ColumnWidthInCharacters = 14;
        worksheet.Cells[i, 1].Value = (float)_branch80PercLessTarget[i - 1].Item2;
        worksheet.Cells[i, 1].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, 2].Value = (float)_branch80PercLessTarget[i - 1].Item3;
        worksheet.Cells[i, 2].NumberFormat = ExcelFmtCurrency;
        worksheet.Cells[i, 3].Value = _branch80PercLessTarget[i - 1].Item4;
        worksheet.Cells[i, 3].NumberFormat = ExcelFmtPercent;
        worksheet.Cells[i, 4].Value =
          (float)((_branch80PercLessTarget[i - 1].Item2 - _branch80PercLessTarget[i - 1].Item3) / 3300);
        worksheet.Cells[i, 4].NumberFormat = ExcelFmtCurrency;
      }
    }


    private List<string> BuildCiScoreHeader()
    {
      return new List<string>
      {
        GetDescription(typeof (CiScoreBand), "PayNo"),
        GetDescription(typeof (CiScoreBand), "Weekly615"),
        GetDescription(typeof (CiScoreBand), "BiWeekly615"),
        GetDescription(typeof (CiScoreBand), "Monthly615"),
        GetDescription(typeof (CiScoreBand), "Total615")
      };
    }

    private List<string> BuildBranchSummaryHeader()
    {
      return new List<string>
      {
        GetDescription(typeof (CIBranchSummary), "BranchName"),
        GetDescription(typeof (CIBranchSummary), "Cheque"),
        GetDescription(typeof (CIBranchSummary), "HandoverTotal"),
        GetDescription(typeof (CIBranchSummary), "Collections")
      };
    }

    private List<string> BuildCiHeader(List<string> additionalHeaders)
    {
      var header = new List<string>
      {
        GetDescription(typeof (CiFile), "LoanMeth"),
        GetDescription(typeof (CiFile), "PayNo"),
        GetDescription(typeof (CiFile), "QuantitySFee"),
        GetDescription(typeof (CiFile), "Cheque"),
        GetDescription(typeof (CiFile), "Target"),
        GetDescription(typeof (CiFile), "TargetPercent"),
        GetDescription(typeof (CiFile), "Actual"),
        GetDescription(typeof (CiFile), "Deviation"),
        GetDescription(typeof (CiFile), "ChargesExclVAT"),
        GetDescription(typeof (CiFile), "ChargesVAT"),
        GetDescription(typeof (CiFile), "TotalCharges"),
        GetDescription(typeof (CiFile), "ChargesPercent"),
        GetDescription(typeof (CiFile), "CreditLife"),
        GetDescription(typeof (CiFile), "CreditLifePercent"),
        GetDescription(typeof (CiFile), "LoanFeeExclVAT"),
        GetDescription(typeof (CiFile), "LoanFeeVAT"),
        GetDescription(typeof (CiFile), "LoanFeeInclVAT"),
        GetDescription(typeof (CiFile), "LoanFeePercent"),
        GetDescription(typeof (CiFile), "FuneralAddOn"),
        GetDescription(typeof (CiFile), "AgeAddOn"),
        GetDescription(typeof (CiFile), "VAPExcl"),
        GetDescription(typeof (CiFile), "VAPVAT"),
        GetDescription(typeof (CiFile), "VAPIncl"),
        GetDescription(typeof (CiFile), "TotalAddOn"),
        GetDescription(typeof (CiFile), "AddOnPercent"),
        GetDescription(typeof (CiFile), "TotFeeExcl"),
        GetDescription(typeof (CiFile), "TotFeeVAT"),
        GetDescription(typeof (CiFile), "TotFeeIncl"),
        GetDescription(typeof (CiFile), "TotFeePercent"),

        GetDescription(typeof (CiFile), "HandoverTotal"),
        GetDescription(typeof (CiFile), "HandoverPercent"),
        GetDescription(typeof (CiFile), "Loans"),
        GetDescription(typeof (CiFile), "LoanMix"),
        GetDescription(typeof (CiFile), "VapLinkedLoans"),
        GetDescription(typeof (CiFile), "VapDeniedByConWithAuth"),
        GetDescription(typeof (CiFile), "VapDeniedByConWithOutAuth"),
        GetDescription(typeof (CiFile), "VapExcludedLoans"),
        GetDescription(typeof (CiFile), "Collections"),
        GetDescription(typeof (CiFile), "RolledValue"),
        GetDescription(typeof (CiFile), "RolledPercentage"),
        GetDescription(typeof (CiFile), "Refunds"),
        GetDescription(typeof (CiFile), "ChequeClientsNew"),
        GetDescription(typeof (CiFile), "HandoverNoOfLoans"),
        GetDescription(typeof (CiFile), "HandoverNoOfClients"),
        GetDescription(typeof (CiFile), "ClientBranch"),
        GetDescription(typeof (CiFile), "ClientSalesRep"),
        GetDescription(typeof (CiFile), "TotalNewClients"),
        GetDescription(typeof (CiFile), "ExistingClients"),
        GetDescription(typeof (CiFile), "RevivedClients"),
        GetDescription(typeof (CiFile), "NewClientMix"),
        GetDescription(typeof (CiFile), "AverageLoan"),
        GetDescription(typeof (CiFile), "NewLoanAverage"),
        GetDescription(typeof (CiFile), "ReswipeBankChange"),
        GetDescription(typeof (CiFile), "ReswipeTermChange"),
        GetDescription(typeof (CiFile), "ReswipeInstalmentChange")
      };
      // today's target
      header.AddRange(additionalHeaders);
      return header;
    }


    private List<string> BuildPossibleHandoversHeader(DateTime? oldestArrearDate)
    {
      var header = new List<string>
      {
        GetDescription(typeof (PossibleHandoversFile), "Branch"),
        string.Empty,
        GetDescription(typeof (PossibleHandoversFile), "HandOverComputer"),
        GetDescription(typeof (PossibleHandoversFile), "HandoverBudget"),
        GetDescription(typeof (PossibleHandoversFile), "ShortFall"),
        GetDescription(typeof (PossibleHandoversFile), "ActualVsBudget"),
        GetDescription(typeof (PossibleHandoversFile), "HandoverValueNextMonth"),
        GetDescription(typeof (PossibleHandoversFile), "NextMonthHandoverForBreakEven"),
        GetDescription(typeof (PossibleHandoversFile), "DebtorBook"),
        GetDescription(typeof (PossibleHandoversFile), "ActualArrears"),
        GetDescription(typeof (PossibleHandoversFile), "ArrearsTarget"),
        GetDescription(typeof (PossibleHandoversFile), "PercentToDebtorsBook"),
        GetDescription(typeof (PossibleHandoversFile), "CollectionsThisMonth"),
        string.Format(GetDescription(typeof (PossibleHandoversFile), "CollectionsPrevMonth"),
          (oldestArrearDate.HasValue ? oldestArrearDate.Value.ToString("dd MMM yyyy") : "N/A")),
        GetDescription(typeof (PossibleHandoversFile), "FlaggedLoans"),
        GetDescription(typeof (PossibleHandoversFile), "FlaggedLoansOverdue")
      };
      return header;
    }


    private CiFile TotalConsolidation(List<CiFile> consolidated)
    {
      var total = new CiFile
      {
        LoanMeth = "M",
        PayNo = 0,
        QuantitySFee = string.Empty,
        Cheque = consolidated.Sum(p => p.Cheque),
        Target = consolidated.Sum(p => p.Target),
        TargetPercent = consolidated.Sum(t => t.TargetPercent),
        ClientBranch = consolidated.Sum(p => p.ClientBranch),
        ClientSalesRep = consolidated.Sum(p => p.ClientSalesRep),
        HandoverNoOfLoans = consolidated.Sum(p => p.HandoverNoOfLoans),
        HandoverNoOfClients = consolidated.Sum(p => p.HandoverNoOfClients),
        Collections = consolidated.Sum(p => p.Collections),
        RolledValue = consolidated.Sum(p => p.RolledValue),
        VapLinkedLoans = consolidated.Sum(p => p.VapLinkedLoans),
        VapDeniedByConWithAuth = consolidated.Sum(p => p.VapDeniedByConWithAuth),
        VapDeniedByConWithOutAuth = consolidated.Sum(p => p.VapDeniedByConWithOutAuth),
        VapExcludedLoans = consolidated.Sum(p => p.VapExcludedLoans)
      };

      if (total.Collections != 0)
        total.RolledPercentage = (float)(total.RolledValue / total.Collections);
      total.ExistingClients = consolidated.Sum(p => p.ExistingClients);
      total.HandoverTotal = consolidated.Sum(p => p.HandoverTotal);
      total.ChargesExclVAT = consolidated.Sum(p => p.ChargesExclVAT);
      total.ChargesVAT = consolidated.Sum(p => p.ChargesVAT);
      total.TotalCharges = consolidated.Sum(p => p.TotalCharges);
      total.CreditLife = consolidated.Sum(p => p.CreditLife);
      total.LoanFeeExclVAT = consolidated.Sum(p => p.LoanFeeExclVAT);
      total.LoanFeeVAT = consolidated.Sum(p => p.LoanFeeVAT);
      total.LoanFeeInclVAT = consolidated.Sum(p => p.LoanFeeInclVAT);
      total.FuneralAddOn = consolidated.Sum(p => p.FuneralAddOn);
      total.AgeAddOn = consolidated.Sum(p => p.AgeAddOn);
      total.VAPExcl = consolidated.Sum(p => p.VAPExcl);
      total.VAPVAT = consolidated.Sum(p => p.VAPVAT);
      total.VAPIncl = consolidated.Sum(p => p.VAPIncl);
      total.TotalAddOn = consolidated.Sum(p => p.TotalAddOn);
      total.TotFeeExcl = consolidated.Sum(p => p.TotFeeExcl);
      total.TotFeeVAT = consolidated.Sum(p => p.TotFeeVAT);
      total.TotFeeIncl = consolidated.Sum(p => p.TotFeeIncl);
      total.Loans = consolidated.Sum(p => p.Loans);
      total.ChequeClientsNew = consolidated.Sum(p => p.ChequeClientsNew);
      total.Refunds = consolidated.Sum(p => p.Refunds);
      total.RevivedClients = consolidated.Sum(p => p.RevivedClients);
      total.TotalNewClients = consolidated.Sum(p => p.TotalNewClients);
      total.ReswipeBankChange = consolidated.Sum(p => p.ReswipeBankChange);
      total.ReswipeTermChange = consolidated.Sum(p => p.ReswipeTermChange);
      total.ReswipeInstalmentChange = consolidated.Sum(p => p.ReswipeInstalmentChange);
      if (total.Cheque != 0)
      {
        total.ChargesPercent = (float)(total.ChargesExclVAT / total.Cheque);
        total.CreditLifePercent = (float)(total.CreditLife / total.Cheque);
        total.LoanFeePercent = (float)(total.LoanFeeExclVAT / total.Cheque);
        total.AddOnPercent = (float)((total.FuneralAddOn + total.AgeAddOn + total.VAPExcl) / total.Cheque);
        total.TotFeePercent = (float)(total.TotFeeExcl / total.Cheque);
      }

      if (consolidated.Sum(p => p.Cheque) != 0)
        total.Actual = consolidated.Sum(p => p.Actual);
      total.Deviation = total.Actual - total.TargetPercent;
      if (consolidated.Sum(p => p.Loans) != 0)
        total.LoanMix = consolidated.Sum(p => p.LoanMix);
      if (consolidated.Sum(p => p.ChequeClientsNew) != 0)
        total.NewClientMix = (float)(total.ChequeClientsNew / consolidated.Sum(p => p.ChequeClientsNew));

      if (total.TotalNewClients != 0)
        total.NewLoanAverage = total.ChequeClientsNew / total.TotalNewClients;

      if ((total.Cheque + total.CreditLife) != 0)
        total.HandoverPercent = (float)(total.HandoverTotal / (total.Cheque + total.CreditLife));
      if (total.Loans != 0)
        total.AverageLoan = total.AverageLoan / total.Loans;
      var consolidatedCiFile = consolidated.FirstOrDefault(p => p.PayNo == 1);
      if (consolidatedCiFile != null)
        total.CompuscanProducts = consolidatedCiFile.CompuscanProducts;

      return total;
    }


    private CiScoreBand TotalConsolidation(List<CiScoreBand> consolidated)
    {
      return new CiScoreBand
      {
        PayNo = 0,
        Weekly615 = consolidated.Sum(p => p.Weekly615),
        BiWeekly615 = consolidated.Sum(p => p.BiWeekly615),
        Monthly615 = consolidated.Sum(p => p.Monthly615),
        Total615 = consolidated.Sum(p => p.Total615)
      };
    }


    private PossibleHandoversFile TotalConsolidation(string regionName, List<PossibleHandoversFile> consolidated)
    {
      var totalLine = new PossibleHandoversFile
      {
        Branch = regionName
      };
      totalLine.ActualArrears = consolidated.Sum(p => p.ActualArrears);
      totalLine.ArrearsTarget = consolidated.Sum(p => p.ArrearsTarget);
      totalLine.CollectionsPrevMonth = consolidated.Average(p => p.CollectionsPrevMonth);
      totalLine.OldestArrearDate = consolidated.Min(p => p.OldestArrearDate);
      totalLine.CollectionsThisMonth = consolidated.Average(p => p.CollectionsThisMonth);
      totalLine.DebtorBook = consolidated.Sum(p => p.DebtorBook);
      totalLine.FlaggedLoans = consolidated.Sum(p => p.FlaggedLoans);
      totalLine.FlaggedLoansOverdue = consolidated.Sum(p => p.FlaggedLoansOverdue);
      totalLine.HandoverBudget = consolidated.Sum(p => p.HandoverBudget);
      totalLine.HandOverComputer = consolidated.Sum(p => p.HandOverComputer);
      totalLine.HandoverValueNextMonth = consolidated.Sum(p => p.HandoverValueNextMonth);
      totalLine.NextMonthHandoverForBreakEven = consolidated.Sum(p => p.NextMonthHandoverForBreakEven);
      totalLine.ShortFall = consolidated.Sum(p => p.ShortFall);

      if (totalLine.HandoverBudget != 0)
        totalLine.ActualVsBudget = (float)totalLine.HandOverComputer / (float)totalLine.HandoverBudget;
      if (totalLine.DebtorBook != 0)
        totalLine.PercentToDebtorsBook = (float)totalLine.ActualArrears / (float)totalLine.DebtorBook;

      return totalLine;
    }


    private List<CiFile> Consolidate(List<CiFile> consolidated, List<CiFile> newInfo)
    {
      var newCondolidated = new List<CiFile>();
      var paynos = new[] { 1, 2, 3, 4, 5, 6, 12, 24 };

      foreach (var payno in paynos)
      {
        var payNoNewInfo = newInfo.FirstOrDefault(p => p.PayNo == payno);
        if (payNoNewInfo == null)
          continue;

        var payNoConsolidated = consolidated.FirstOrDefault(p => p.PayNo == payno) ?? new CiFile
        {
          LoanMeth = "M",
          PayNo = payno,
          QuantitySFee = string.Empty
        };

        payNoConsolidated.Cheque = payNoConsolidated.Cheque + payNoNewInfo.Cheque;
        payNoConsolidated.Target = payNoConsolidated.Target + payNoNewInfo.Target;
        payNoConsolidated.TargetPercent = payNoNewInfo.TargetPercent;
        payNoConsolidated.ClientBranch = payNoConsolidated.ClientBranch + payNoNewInfo.ClientBranch;
        payNoConsolidated.ClientSalesRep = payNoConsolidated.ClientSalesRep + payNoNewInfo.ClientSalesRep;
        payNoConsolidated.HandoverNoOfLoans = payNoConsolidated.HandoverNoOfLoans + payNoNewInfo.HandoverNoOfLoans;
        payNoConsolidated.HandoverNoOfClients = payNoConsolidated.HandoverNoOfClients + payNoNewInfo.HandoverNoOfClients;
        payNoConsolidated.Collections = payNoConsolidated.Collections + payNoNewInfo.Collections;
        payNoConsolidated.RolledValue = payNoConsolidated.RolledValue + payNoNewInfo.RolledValue;
        payNoConsolidated.VapLinkedLoans = payNoConsolidated.VapLinkedLoans + payNoNewInfo.VapLinkedLoans;
        payNoConsolidated.VapDeniedByConWithAuth = payNoConsolidated.VapDeniedByConWithAuth +
                                                   payNoNewInfo.VapDeniedByConWithAuth;
        payNoConsolidated.VapDeniedByConWithOutAuth = payNoConsolidated.VapDeniedByConWithOutAuth +
                                                      payNoNewInfo.VapDeniedByConWithOutAuth;
        payNoConsolidated.VapExcludedLoans = payNoConsolidated.VapExcludedLoans + payNoNewInfo.VapExcludedLoans;
        if (payNoConsolidated.Collections != 0)
          payNoConsolidated.RolledPercentage = (float)(payNoConsolidated.RolledValue / payNoConsolidated.Collections);
        payNoConsolidated.ExistingClients = payNoConsolidated.ExistingClients + payNoNewInfo.ExistingClients;
        payNoConsolidated.HandoverTotal = payNoConsolidated.HandoverTotal + payNoNewInfo.HandoverTotal;
        payNoConsolidated.ChargesExclVAT = payNoConsolidated.ChargesExclVAT + payNoNewInfo.ChargesExclVAT;
        payNoConsolidated.ChargesVAT = payNoConsolidated.ChargesVAT + payNoNewInfo.ChargesVAT;
        payNoConsolidated.TotalCharges = payNoConsolidated.TotalCharges + payNoNewInfo.TotalCharges;
        payNoConsolidated.CreditLife = payNoConsolidated.CreditLife + payNoNewInfo.CreditLife;
        payNoConsolidated.LoanFeeExclVAT = payNoConsolidated.LoanFeeExclVAT + payNoNewInfo.LoanFeeExclVAT;
        payNoConsolidated.LoanFeeVAT = payNoConsolidated.LoanFeeVAT + payNoNewInfo.LoanFeeVAT;
        payNoConsolidated.LoanFeeInclVAT = payNoConsolidated.LoanFeeInclVAT + payNoNewInfo.LoanFeeInclVAT;
        payNoConsolidated.FuneralAddOn = payNoConsolidated.FuneralAddOn + payNoNewInfo.FuneralAddOn;
        payNoConsolidated.AgeAddOn = payNoConsolidated.AgeAddOn + payNoNewInfo.AgeAddOn;
        payNoConsolidated.VAPExcl = payNoConsolidated.VAPExcl + payNoNewInfo.VAPExcl;
        payNoConsolidated.VAPVAT = payNoConsolidated.VAPVAT + payNoNewInfo.VAPVAT;
        payNoConsolidated.VAPIncl = payNoConsolidated.VAPIncl + payNoNewInfo.VAPIncl;
        payNoConsolidated.TotalAddOn = payNoConsolidated.TotalAddOn + payNoNewInfo.TotalAddOn;
        payNoConsolidated.TotFeeExcl = payNoConsolidated.TotFeeExcl + payNoNewInfo.TotFeeExcl;
        payNoConsolidated.TotFeeVAT = payNoConsolidated.TotFeeVAT + payNoNewInfo.TotFeeVAT;
        payNoConsolidated.TotFeeIncl = payNoConsolidated.TotFeeIncl + payNoNewInfo.TotFeeIncl;
        payNoConsolidated.Loans = payNoConsolidated.Loans + payNoNewInfo.Loans;
        payNoConsolidated.ChequeClientsNew = payNoConsolidated.ChequeClientsNew + payNoNewInfo.ChequeClientsNew;
        payNoConsolidated.Refunds = payNoConsolidated.Refunds + payNoNewInfo.Refunds;
        payNoConsolidated.ReswipeBankChange = payNoConsolidated.ReswipeBankChange + payNoNewInfo.ReswipeBankChange;
        payNoConsolidated.ReswipeTermChange = payNoConsolidated.ReswipeTermChange + payNoNewInfo.ReswipeTermChange;
        payNoConsolidated.ReswipeInstalmentChange = payNoConsolidated.ReswipeInstalmentChange +
                                                    payNoNewInfo.ReswipeInstalmentChange;
        payNoConsolidated.RevivedClients = payNoConsolidated.RevivedClients + payNoNewInfo.RevivedClients;
        payNoConsolidated.TotalNewClients = payNoConsolidated.TotalNewClients + payNoNewInfo.TotalNewClients;


        if (payNoConsolidated.Cheque != 0)
        {
          payNoConsolidated.ChargesPercent = (float)(payNoConsolidated.ChargesExclVAT / payNoConsolidated.Cheque);
          payNoConsolidated.CreditLifePercent = (float)(payNoConsolidated.CreditLife / payNoConsolidated.Cheque);
          payNoConsolidated.LoanFeePercent = (float)(payNoConsolidated.LoanFeeExclVAT / payNoConsolidated.Cheque);
          payNoConsolidated.AddOnPercent =
            (float)
              ((payNoConsolidated.FuneralAddOn + payNoConsolidated.AgeAddOn + payNoConsolidated.VAPExcl) /
               payNoConsolidated.Cheque);
          payNoConsolidated.TotFeePercent = (float)(payNoConsolidated.TotFeeExcl / payNoConsolidated.Cheque);
        }

        if (payNoConsolidated.TotalNewClients != 0)
          payNoConsolidated.NewLoanAverage = payNoConsolidated.ChequeClientsNew / payNoConsolidated.TotalNewClients;

        if ((payNoConsolidated.Cheque + payNoConsolidated.CreditLife) != 0)
          payNoConsolidated.HandoverPercent =
            (float)
              (payNoConsolidated.HandoverTotal /
               (payNoConsolidated.Cheque + payNoConsolidated.CreditLife));
        if (payNoConsolidated.Loans != 0)
          payNoConsolidated.AverageLoan = payNoConsolidated.AverageLoan / payNoConsolidated.Loans;

        if (payNoConsolidated.CompuscanProducts != null)
        {
          for (var i = 0; i < payNoConsolidated.CompuscanProducts.Keys.Count; i++)
          {
            var keys = payNoConsolidated.CompuscanProducts.Keys.ToArray();
            if (payNoNewInfo.CompuscanProducts.ContainsKey(keys[i]))
              payNoConsolidated.CompuscanProducts[keys[i]] = payNoConsolidated.CompuscanProducts[keys[i]] +
                                                             payNoNewInfo.CompuscanProducts[keys[i]];
          }
        }
        if (payNoNewInfo.CompuscanProducts != null)
        {
          foreach (var key in payNoNewInfo.CompuscanProducts.Keys)
          {
            if (payNoConsolidated.CompuscanProducts == null)
              payNoConsolidated.CompuscanProducts = new Dictionary<string, int>();
            if (!payNoConsolidated.CompuscanProducts.ContainsKey(key))
              payNoConsolidated.CompuscanProducts.Add(key, payNoNewInfo.CompuscanProducts[key]);
          }
        }
        if (payNoConsolidated.CompuscanProducts == null)
          payNoConsolidated.CompuscanProducts = new Dictionary<string, int>();
        newCondolidated.Add(payNoConsolidated);
      }

      return newCondolidated;
    }


    private List<CiScoreBand> Consolidate(List<CiScoreBand> consolidated, List<CiScoreBand> newInfo)
    {
      var newCondolidated = new List<CiScoreBand>();

      var paynos = new[] { 1, 2, 3, 4, 5, 6, 12, 24 };

      foreach (var payno in paynos)
      {
        var payNoNewInfo = newInfo.FirstOrDefault(p => p.PayNo == payno);
        if (payNoNewInfo == null)
          continue;

        var payNoConsolidated = consolidated.FirstOrDefault(p => p.PayNo == payno) ?? new CiScoreBand
        {
          PayNo = payno
        };

        payNoConsolidated.BiWeekly615 = payNoConsolidated.BiWeekly615 + payNoNewInfo.BiWeekly615;
        payNoConsolidated.Monthly615 = payNoConsolidated.Monthly615 + payNoNewInfo.Monthly615;
        payNoConsolidated.Weekly615 = payNoConsolidated.Weekly615 + payNoNewInfo.Weekly615;
        payNoConsolidated.Total615 = payNoConsolidated.Total615 + payNoNewInfo.Total615;

        newCondolidated.Add(payNoConsolidated);
      }

      return newCondolidated;
    }


    private List<CiFile> FinaliseConsolidation(List<CiFile> consolidated)
    {
      foreach (var c in consolidated)
      {
        if (consolidated.Sum(d => d.Cheque) != 0)
          c.Actual = (float)c.Cheque / (float)consolidated.Where(p => p.PayNo > 0).Sum(d => d.Cheque);
        c.Deviation = c.Actual - c.TargetPercent;
        if (consolidated.Sum(d => d.Loans) != 0)
          c.LoanMix = (float)c.Loans / (float)consolidated.Where(p => p.PayNo > 0).Sum(d => d.Loans);
        if (consolidated.Sum(p => p.TotalNewClients) != 0)
          c.NewClientMix = (c.TotalNewClients / (float)consolidated.Where(p => p.PayNo > 0).Sum(p => p.TotalNewClients));
        if (c.TotalNewClients != 0)
          c.NewLoanAverage = c.ChequeClientsNew / c.TotalNewClients;
        if (c.Loans != 0)
          c.AverageLoan = c.Cheque / c.Loans;
        if (c.CompuscanProducts == null)
        {
          c.CompuscanProducts = new Dictionary<string, int>();
        }
        else if (c.CompuscanProducts.Count > 0)
        {
          c.CompuscanProducts = c.CompuscanProducts.Reverse().ToDictionary(k => k.Key, v => v.Value);
        }
      }

      return consolidated;
    }


    private List<CiScoreBand> GetScoreBands(string branch, DateTime startDate, DateTime endDate)
    {
      var paynos = new[] { 1, 2, 3, 4, 5, 6, 12, 24 };

      var fileLines = new List<CiScoreBand>();
      var basicLoan =
        RedisConnection.GetObjectFromString<List<BasicLoan>>(string.Format(AssReporting.REDIS_KEY_BASIC_LOAN, branch,
          startDate.ToString("ddMMyyyy"), endDate.ToString("ddMMyyyy"))) ?? new List<BasicLoan>();

      foreach (var payno in paynos)
      {
        var fileLine = new CiScoreBand
        {
          BiWeekly615 = basicLoan.Where(p => p.PayNo == payno).Sum(p => p.ScoreAbove615BiWeekly),
          Monthly615 = basicLoan.Where(p => p.PayNo == payno).Sum(p => p.ScoreAbove615Monthly),
          PayNo = payno,
          Weekly615 = basicLoan.Where(p => p.PayNo == payno).Sum(p => p.ScoreAbove615Weekly)
        };

        fileLine.Total615 = fileLine.BiWeekly615 + fileLine.Weekly615 + fileLine.Monthly615;

        fileLines.Add(fileLine);
      }

      return fileLines;
    }


    private List<CiFile> GetCiInfo(string branch, long branchId, DateTime startDate, DateTime endDate)
    {
      var paynos = new[] { 1, 2, 3, 4, 5, 6, 12, 24 };
      var fileLines = new List<CiFile>();
      var clientLoanInfo =
        RedisConnection.GetObjectFromString<List<ClientLoanInfo>>(string.Format(AssReporting.REDIS_KEY_CLIENT_LOAN,
          branch, startDate.ToString("ddMMyyyy"), endDate.ToString("ddMMyyyy"))) ?? new List<ClientLoanInfo>();
      var vap =
        RedisConnection.GetObjectFromString<List<VAP>>(string.Format(AssReporting.REDIS_KEY_VAP, branch,
          startDate.ToString("ddMMyyyy"), endDate.ToString("ddMMyyyy"))) ?? new List<VAP>();
      var basicLoan =
        RedisConnection.GetObjectFromString<List<BasicLoan>>(string.Format(AssReporting.REDIS_KEY_BASIC_LOAN, branch,
          startDate.ToString("ddMMyyyy"), endDate.ToString("ddMMyyyy"))) ?? new List<BasicLoan>();
      var collectionRefund =
        RedisConnection.GetObjectFromString<List<CollectionRefund>>(
          string.Format(AssReporting.REDIS_KEY_COLLECTION_REFUND, branch, startDate.ToString("ddMMyyyy"),
            endDate.ToString("ddMMyyyy"))) ?? new List<CollectionRefund>();
      var handoverInfo =
        RedisConnection.GetObjectFromString<List<HandoverInfo>>(string.Format(AssReporting.REDIS_KEY_HANDOVER_INFO,
          branch, startDate.ToString("ddMMyyyy"), endDate.ToString("ddMMyyyy"))) ?? new List<HandoverInfo>();
      var reswipeInfo =
        RedisConnection.GetObjectFromString<List<Reswipes>>(string.Format(AssReporting.REDIS_KEY_RESWIPE_INFO, branch,
          startDate.ToString("ddMMyyyy"), endDate.ToString("ddMMyyyy"))) ?? new List<Reswipes>();
      var rolledAccounts =
        RedisConnection.GetObjectFromString<List<RolledAccounts>>(string.Format(AssReporting.REDIS_KEY_ROLLED_ACCOUNTS,
          branch, startDate.ToString("ddMMyyyy"), endDate.ToString("ddMMyyyy"))) ?? new List<RolledAccounts>();

      var chequeLoanMixTargets = GetChequeLoanMixTargets();
      var dailyPhasingPercent = GetDailyPhasingPercent(endDate, startDate.Date != endDate.Date);
      var monthCiTargets = GetMonthCiTargets(startDate, new[] { branchId });

      foreach (var payno in paynos)
      {
        var fileLine = new CiFile
        {
          LegacyBranchNumber = branch
        };

        // compuscan products
        if (payno == 1)
          fileLine.CompuscanProducts =
            RedisConnection.GetObjectFromString<Dictionary<string, int>>(
              string.Format(AssReporting.REDIS_KEY_COMPUSCAN_PRODUCTS, branch, startDate.ToString("ddMMyyyy"),
                endDate.ToString("ddMMyyyy"))) ?? new Dictionary<string, int>();

        if (basicLoan.Where(b => b.PayNo == payno).Sum(b => b.Quantity) != 0)
          fileLine.AverageLoan = basicLoan.Where(b => b.PayNo == payno).Sum(b => b.Cheque) /
                                 basicLoan.Where(b => b.PayNo == payno).Sum(b => b.Quantity);
        fileLine.Cheque = basicLoan.Where(p => p.PayNo == payno).Sum(p => p.Cheque);
        fileLine.Target = GetTargetValue((PayNo)payno, new[] { branchId }, dailyPhasingPercent, monthCiTargets,
          chequeLoanMixTargets);

        fileLine.TargetPercent = chequeLoanMixTargets[(PayNo)payno];
        fileLine.ClientBranch = basicLoan.Where(p => p.PayNo == payno).Sum(p => p.BranchLoans);
        fileLine.ClientSalesRep = basicLoan.Where(p => p.PayNo == payno).Sum(p => p.SalesRepLoans);
        fileLine.HandoverNoOfLoans = handoverInfo.Where(p => p.PayNo == payno).Sum(p => p.Quantity);
        fileLine.HandoverNoOfClients = handoverInfo.Where(p => p.PayNo == payno).Sum(p => p.ClientQuantity);
        fileLine.Collections = collectionRefund.Where(p => p.PayNo == payno).Sum(p => p.Collections);
        fileLine.RolledValue = rolledAccounts.Where(p => p.PayNo == payno).Sum(p => p.RollbackValue);
        fileLine.VapLinkedLoans = vap.Where(p => p.PayNo == payno).Sum(p => p.VapLinkedLoans);
        fileLine.VapDeniedByConWithAuth = vap.Where(p => p.PayNo == payno).Sum(p => p.VapDeniedByConWithAuth);
        fileLine.VapDeniedByConWithOutAuth = vap.Where(p => p.PayNo == payno).Sum(p => p.VapDeniedByConWithOutAuth);
        fileLine.VapExcludedLoans = vap.Where(p => p.PayNo == payno).Sum(p => p.VapExcludedLoans);
        if (fileLine.Collections != 0)
          fileLine.RolledPercentage = (float)(fileLine.RolledValue / fileLine.Collections);
        fileLine.ExistingClients = clientLoanInfo.Where(p => p.PayNo == payno).Sum(p => p.ExistingClientCount);
        if (basicLoan.Where(p => p.PayNo == payno).Sum(p => p.Cheque + p.LoanFeeInclVAT) != 0)
          fileLine.HandoverPercent =
            (float)
              (handoverInfo.Where(p => p.PayNo == payno).Sum(p => p.Amount) /
               basicLoan.Where(p => p.PayNo == payno).Sum(p => p.Cheque + p.LoanFeeInclVAT));
        fileLine.HandoverTotal = handoverInfo.Where(p => p.PayNo == payno).Sum(p => p.Amount);
        fileLine.ChargesExclVAT = basicLoan.Where(p => p.PayNo == payno).Sum(p => p.ChargesExclVAT);
        fileLine.ChargesVAT = basicLoan.Where(p => p.PayNo == payno).Sum(p => p.ChargesVAT);
        fileLine.TotalCharges = basicLoan.Where(p => p.PayNo == payno).Sum(p => p.TotalCharges);
        fileLine.CreditLife = basicLoan.Where(p => p.PayNo == payno).Sum(p => p.CreditLife);
        fileLine.LoanFeeExclVAT = basicLoan.Where(p => p.PayNo == payno).Sum(p => p.LoanFeeExclVAT);
        fileLine.LoanFeeVAT = basicLoan.Where(p => p.PayNo == payno).Sum(p => p.LoanFeeVAT);
        fileLine.LoanFeeInclVAT = basicLoan.Where(p => p.PayNo == payno).Sum(p => p.LoanFeeInclVAT);
        fileLine.FuneralAddOn = basicLoan.Where(p => p.PayNo == payno).Sum(p => p.FuneralAddOn);
        fileLine.AgeAddOn = basicLoan.Where(p => p.PayNo == payno).Sum(p => p.AgeAddOn);
        fileLine.VAPExcl = basicLoan.Where(p => p.PayNo == payno).Sum(p => p.VAPExcl);
        fileLine.VAPVAT = basicLoan.Where(p => p.PayNo == payno).Sum(p => p.VAPVAT);
        fileLine.VAPIncl = basicLoan.Where(p => p.PayNo == payno).Sum(p => p.VAPIncl);
        fileLine.TotalAddOn = basicLoan.Where(p => p.PayNo == payno).Sum(p => p.TotalAddOn);
        fileLine.TotFeeExcl = basicLoan.Where(p => p.PayNo == payno).Sum(p => p.TotFeeExcl);
        fileLine.TotFeeVAT = basicLoan.Where(p => p.PayNo == payno).Sum(p => p.TotFeeVAT);
        fileLine.TotFeeIncl = basicLoan.Where(p => p.PayNo == payno).Sum(p => p.TotFeeIncl);
        if (basicLoan.Where(p => p.PayNo == payno).Sum(p => p.Cheque) != 0)
        {
          fileLine.ChargesPercent =
            (float)
              (basicLoan.Where(p => p.PayNo == payno).Sum(p => p.ChargesExclVAT) /
               basicLoan.Where(p => p.PayNo == payno).Sum(p => p.Cheque));
          fileLine.CreditLifePercent =
            (float)
              (basicLoan.Where(p => p.PayNo == payno).Sum(p => p.CreditLife) /
               basicLoan.Where(p => p.PayNo == payno).Sum(p => p.Cheque));
          fileLine.LoanFeePercent =
            (float)
              (basicLoan.Where(p => p.PayNo == payno).Sum(p => p.LoanFeeExclVAT) /
               basicLoan.Where(p => p.PayNo == payno).Sum(p => p.Cheque));
          fileLine.AddOnPercent =
            (float)
              ((basicLoan.Where(p => p.PayNo == payno).Sum(p => p.FuneralAddOn) +
                basicLoan.Where(p => p.PayNo == payno).Sum(p => p.AgeAddOn) +
                basicLoan.Where(p => p.PayNo == payno).Sum(p => p.VAPExcl)) /
               basicLoan.Where(p => p.PayNo == payno).Sum(p => p.Cheque));
          fileLine.TotFeePercent =
            (float)
              (basicLoan.Where(p => p.PayNo == payno).Sum(p => p.TotFeeExcl) /
               basicLoan.Where(p => p.PayNo == payno).Sum(p => p.Cheque));
        }

        fileLine.LoanMeth = "M";
        fileLine.Loans = basicLoan.Where(p => p.PayNo == payno).Sum(p => p.Quantity);
        fileLine.PayNo = payno;
        fileLine.ChequeClientsNew = clientLoanInfo.Where(p => p.PayNo == payno).Sum(p => p.NewClientAmount);
        fileLine.QuantitySFee = string.Empty; // blank in excel
        fileLine.Refunds = collectionRefund.Where(p => p.PayNo == payno).Sum(p => p.Refunds);
        fileLine.RevivedClients = clientLoanInfo.Where(p => p.PayNo == payno).Sum(p => p.RevivedClientCount);
        if (fileLine.RevivedClients > 0)
        {
          fileLine.AverageRevivedClientsAmount =
            clientLoanInfo.Where(p => p.PayNo == payno).Sum(p => p.RevivedClientAmount)/fileLine.RevivedClients;
        }
        fileLine.TotalNewClients = clientLoanInfo.Where(p => p.PayNo == payno).Sum(p => p.NewClientQuantity);
        fileLine.ReswipeBankChange = reswipeInfo.Where(p => p.PayNo == payno).Sum(p => p.BankChange);
        fileLine.ReswipeTermChange = reswipeInfo.Where(p => p.PayNo == payno).Sum(p => p.LoanTermChange);
        fileLine.ReswipeInstalmentChange = reswipeInfo.Where(p => p.PayNo == payno).Sum(p => p.InstalmentChange);

        if (basicLoan.Sum(p => p.Quantity) != 0)
        {
          var totalQuantity = basicLoan.Sum(p => p.Quantity);
          fileLine.LoanMix = ((float)fileLine.Loans / totalQuantity);
        }

        if (basicLoan.Sum(p => p.Cheque) != 0)
        {
          var totalCheque = basicLoan.Sum(p => p.Cheque);
          fileLine.Actual = ((float)fileLine.Cheque / (float)totalCheque);
        }
        fileLine.Deviation = fileLine.Actual - fileLine.TargetPercent;

        if (clientLoanInfo.Sum(p => p.NewClientQuantity) != 0)
          fileLine.NewClientMix = (clientLoanInfo.Where(p => p.PayNo == payno).Sum(p => p.NewClientQuantity) /
                                   (float)clientLoanInfo.Sum(p => p.NewClientQuantity));
        if (fileLine.TotalNewClients != 0)
          fileLine.NewLoanAverage = fileLine.ChequeClientsNew / fileLine.TotalNewClients;

        fileLines.Add(fileLine);
      }

      return fileLines;
    }


    private List<string> BuildBranchSyncHeader()
    {
      return new List<string> { "Branch", "Last Updated" };
    }


    private List<string> Build80PercLessHeader()
    {
      return new List<string> { "Branch", "Target", "Achieved", "Achieved %", "No. of Loans Required" };
    }


    private List<Tuple<DateTime, string, long>> GetBranchSyncData(string[] branchNos)
    {
      var lastUpdatedBranches = new List<Tuple<DateTime, string, long>>();
      for (var i = 0; i < branchNos.Length; i++)
      {
        var lastUpdatedBranch =
          RedisConnection.GetObjectFromString<Tuple<DateTime, string, long>>(
            string.Format(AssReporting.REDIS_KEY_BRANCH_LAST_SYNC_DATE, branchNos[i]));
        if (lastUpdatedBranch != null)
          lastUpdatedBranches.Add(lastUpdatedBranch);
      }
      return lastUpdatedBranches;
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


    private Dictionary<long, string> GetRegionNamesByBranchIds(List<long> branchIds)
    {
      using (var uow = new UnitOfWork())
      {
        return
          new XPQuery<BRN_Branch>(uow).Where(b => branchIds.Contains(b.BranchId))
            .Select(b => new { b.Region.RegionId, b.Region.Description })
            .Distinct().OrderBy(t => t.RegionId)
            .ToDictionary(b => b.RegionId, b => b.Description);
      }
    }


    private Dictionary<long, Tuple<string, string, bool>> GetLegacyBranchNumbers(long regionId, List<long> branchIds)
    {
      using (var uow = new UnitOfWork())
      {
        return
          new XPQuery<BRN_Branch>(uow).Where(b => b.Region.RegionId == regionId && branchIds.Contains(b.BranchId))
            .Select(b => new { b.BranchId, b.LegacyBranchNum, b.Company.Name, b.IsClosed })
            .ToDictionary(b => b.BranchId, b => new Tuple<string, string, bool>(b.LegacyBranchNum, b.Name, b.IsClosed));
      }
    }


    #region Targets

    private enum PayNo
    {
      PayNo1 = 1,
      PayNo2 = 2,
      PayNo3 = 3,
      PayNo4 = 4,
      PayNo5 = 5,
      PayNo6 = 6,
      PayNo12 = 12,
      PayNo24 = 24,
      Total = 0
    }


    private static decimal GetTargetValue(PayNo payNo, long[] branchIds, float dailyPhasePercent,
      Dictionary<long, decimal> monthlyCiTargets, Dictionary<PayNo, float> chequeLoanMixTargets)
    {
      var branchBudget = monthlyCiTargets.Where(t => branchIds.Contains(t.Key)).Sum(t => t.Value);
      var chequeLoanMixTarget = chequeLoanMixTargets[payNo];

      var totalBudget = branchBudget * (decimal)dailyPhasePercent / 100;

      if (payNo == PayNo.Total)
        return totalBudget;

      return (totalBudget * (decimal)chequeLoanMixTarget);
    }


    private static Dictionary<long, decimal> GetMonthCiTargets(DateTime targetDate, ICollection<long> branchIds)
    {
      using (var uow = new UnitOfWork())
      {
        return new XPQuery<TAR_BranchCIMonthly>(uow).Where(t => branchIds.Contains(t.Branch.BranchId)
                                                                && t.TargetDate.Year == targetDate.Year &&
                                                                t.TargetDate.Month == targetDate.Month)
          .ToDictionary(k => k.Branch.BranchId, v => v.Amount);
      }
    }


    private static float GetDailyPhasingPercent(DateTime targetDate, bool monthToDate)
    {
      using (var uow = new UnitOfWork())
      {
        var targetDay = (targetDate.Month == DateTime.Today.Month && targetDate.Year == DateTime.Today.Year)
          ? DateTime.Today
          : targetDate.Date;

        return new XPQuery<TAR_DailySale>(uow).Where(
          t =>
            !t.DisableDate.HasValue && t.TargetDate.Date.Month == targetDate.Month &&
            t.TargetDate.Date.Year == targetDate.Year &&
            (monthToDate ? t.TargetDate.Date.Day <= targetDay.Day : t.TargetDate.Date.Day == targetDay.Day))
          .Sum(t => t.Percent);
      }
    }


    /// <summary>
    /// This returns the target % 
    /// values will be moved to the DB
    /// </summary>
    /// <returns>
    /// key: payno
    /// value: percentage</returns>
    private static Dictionary<PayNo, float> GetChequeLoanMixTargets()
    {
      // TODO: move to DB
      return new Dictionary<PayNo, float>
      {
        {PayNo.PayNo1, (float) 0.25},
        {PayNo.PayNo2, 0},
        {PayNo.PayNo3, (float) 0.1},
        {PayNo.PayNo4, (float) 0.38},
        {PayNo.PayNo5, 0},
        {PayNo.PayNo6, (float) 0.27},
        {PayNo.PayNo12, 0},
        {PayNo.PayNo24, 0},
        {PayNo.Total, 1}
      };
    }

    #endregion

  }
}
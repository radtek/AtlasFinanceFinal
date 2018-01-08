using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Atlas.Ass.Framework.Repository;
using Atlas.Ass.Repository;
using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using AutoMapper;
using DevExpress.Xpo;
using Falcon.Common.Services;
using Falcon.Service.Business.Reporting;
using Falcon.Service.Core;
using Quartz;
using Serilog;

namespace Falcon.Service.Tasks
{
  [DisallowConcurrentExecution]
  public class AssCiReportPossibleHandoverTask : IJob
  {
    private readonly IAssCiRepository _assCiRepository;
    private static readonly ILogger Log = Serilog.Log.Logger.ForContext<AssCiReportPossibleHandoverTask>();

    private readonly string _connectionString = ConfigurationManager.ConnectionStrings["AssConnection"] != null
      ? ConfigurationManager.ConnectionStrings["AssConnection"].ConnectionString
      : string.Empty;

    public AssCiReportPossibleHandoverTask()
    {
      _assCiRepository = new AssCiRepository(new ConfigService(), Log);
    }

    public void Execute(IJobExecutionContext context)
    {
      if (!string.IsNullOrEmpty(_connectionString))
      {
        Log.Information("[FalconService][Task] {Job} Executing...", context.JobDetail.Key.Name);
        try
        {
          string[] branchNos;
          var allowedRegions = new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };
          var notAllowedBranches = new long[] { 1, 18, 21, 84, 111, 113, 149, 152, 167, 207 };
          //var hoursToRunJobs = new[] { 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };

          // can be removed once regions have been unlinked properly
          using (var uow = new UnitOfWork())
          {
            var branches =
              new XPQuery<BRN_Branch>(uow).Where(
                b => b.BranchId > 1 && !b.IsClosed &&            // Skip HO and closed
                  allowedRegions.Contains(b.Region.RegionId) && 
                  !notAllowedBranches.Contains(b.BranchId))
                .Select(b => new { b.BranchId, b.LegacyBranchNum })
                .ToArray();
            branchNos = branches.Select(b => b.LegacyBranchNum).ToArray();
          }

          var startDate = DateTime.Today;
          var endDate = DateTime.Today;

          var rundaily = bool.Parse(ConfigurationManager.AppSettings["runDaily"] ?? "false");

          if (rundaily)
          {
            RunDailyTargets(context.JobDetail.Key.Name);
          }

          RunHandOverInfoQuery(branchNos, startDate, endDate,context.JobDetail.Key.Name);
          RunCollectionRefundQuery(branchNos, startDate, endDate, context.JobDetail.Key.Name);

          startDate = DateTime.Today.AddDays(-DateTime.Today.Day + 1);
          endDate = startDate.AddMonths(1).AddDays(-1);

          RunHandOverInfoQuery(branchNos, startDate, endDate, context.JobDetail.Key.Name);
          RunCollectionRefundQuery(branchNos, startDate, endDate, context.JobDetail.Key.Name);

          #region Once a day run
          //if (DateTime.Now.Hour == 2)
          {
            if (RedisConnection.GetObjectFromString<DateTime?>(AssReporting.REDIS_KEY_LAST_POSSIBLE_EVERY_HOUR_RUN) == null)
            {
              RunBudgetsQuery(branchNos, context.JobDetail.Key.Name);
              RunPossibleHandoverQuery(branchNos, context.JobDetail.Key.Name);
              RunArrearsQuery(branchNos, context.JobDetail.Key.Name);
              RunCollectionsQuery(branchNos, context.JobDetail.Key.Name);
              RunDebtorsBookQuery(branchNos, context.JobDetail.Key.Name);
              RunLoansFlaggedQuery(branchNos, context.JobDetail.Key.Name);
              RedisConnection.SetStringFromObject<DateTime?>(AssReporting.REDIS_KEY_LAST_POSSIBLE_EVERY_HOUR_RUN, DateTime.Now, new TimeSpan(0, 56, 0));
            }

            if (RedisConnection.GetObjectFromString<DateTime?>(AssReporting.REDIS_KEY_LAST_ONE_TIME_RUN) == null)
            {
              for (var i = 1; i <= 3; i++)
              {
                endDate = startDate.AddDays(-1);
                startDate = startDate.AddMonths(-1);
                RunHandOverInfoQuery(branchNos, startDate, endDate, context.JobDetail.Key.Name, new TimeSpan(23, 45, 0));
                RunCollectionRefundQuery(branchNos, startDate, endDate,context.JobDetail.Key.Name, new TimeSpan(23, 45, 0));
              }
              RedisConnection.SetStringFromObject<DateTime?>(AssReporting.REDIS_KEY_LAST_ONE_TIME_RUN, DateTime.Now, new TimeSpan(2, 0, 0));
            }
          }

          #endregion

          Log.Information("[FalconService][Task] {Job} Completed", context.JobDetail.Key.Name);
        }
        catch (Exception ex)
        {
          Log.Error("[FalconService][Task] Error: {Error}", ex.Message);
        }
      }
    }

    private void RunCollectionRefundQuery(string[] branchNos, DateTime startDate, DateTime endDate, string jobName,
      TimeSpan? expiryTime = null)
    {
      Log.Information(
        string.Format("[FalconService][Task][{2}] - Started Query Run Collection Refund: {0} - {1}",
          startDate.ToString("dd/MM/yyyy"), endDate.ToString("dd/MM/yyyy"), jobName));

      var thisMonthCollectionRefund = _assCiRepository.RunCollectionRefundQuery(branchNos.ToArray(), startDate, endDate);
      foreach (var branchNo in branchNos)
      {
        var thisMonthBranchResult = thisMonthCollectionRefund.Where(b => b.LegacyBranchNumber == branchNo).ToList();
        RedisConnection.SetStringFromObject(
          string.Format(AssReporting.REDIS_KEY_COLLECTION_REFUND, branchNo, startDate.ToString("ddMMyyyy"),
            endDate.ToString("ddMMyyyy")), thisMonthBranchResult, expiryTime ?? new TimeSpan(1, 30, 0));
      }

      Log.Information(
        string.Format("[FalconService][Task][{2}] - Finished Query Run Collection Refund: {0} - {1}",
          startDate.ToString("dd/MM/yyyy"), endDate.ToString("dd/MM/yyyy"),jobName));
    }

    private void RunHandOverInfoQuery(string[] branchNos, DateTime startDate, DateTime endDate, string jobName,
      TimeSpan? expiryTime = null)
    {
      Log.Information(string.Format(
        "[FalconService][Task][{2}] - Started Query Run Handover Info: {0} - {1}",
        startDate.ToString("dd/MM/yyyy"), endDate.ToString("dd/MM/yyyy"), jobName));

      var thisMonthHandoverInfo = _assCiRepository.RunHandoverInfoQuery(branchNos.ToArray(), startDate, endDate);
      foreach (var branchNo in branchNos)
      {
        var thisMonthBranchResult = thisMonthHandoverInfo.Where(b => b.LegacyBranchNumber == branchNo).ToList();
        RedisConnection.SetStringFromObject(
          string.Format(AssReporting.REDIS_KEY_HANDOVER_INFO, branchNo, startDate.ToString("ddMMyyyy"),
            endDate.ToString("ddMMyyyy")), thisMonthBranchResult, expiryTime ?? new TimeSpan(1, 30, 0));
      }

      Log.Information(
        string.Format("[FalconService][Task][{2}] - Finished Query Run Handover Info: {0} - {1}",
          startDate.ToString("dd/MM/yyyy"), endDate.ToString("dd/MM/yyyy"), jobName));
    }

    // TODO: move into target repo
    private static void RunBudgetsQuery(string[] branchNos, string jobName)
    {
      Log.Information(string.Format("[FalconService][Task][{0}] - Started Query Run Getting Budgets", jobName));

      using (var uow = new UnitOfWork())
      {
        var targets =
          new XPQuery<TAR_HandoverTarget>(uow).Where(
            b => branchNos.Contains(b.Branch.LegacyBranchNum) && b.DisableDate == null && b.ActiveDate <= DateTime.Today)
            .ToList();
        var targetsDto = Mapper.Map<List<TAR_HandoverTarget>, List<TAR_HandoverTargetDTO>>(targets);
        foreach (var target in targetsDto)
        {
          RedisConnection.SetStringFromObject(
            string.Format(AssReporting.REDIS_KEY_BUDGETS, target.Branch.LegacyBranchNum,
              target.StartRange.ToString("ddMMyyyy"), target.EndRange.ToString("ddMMyyyy")), target,
            new TimeSpan(24, 0, 0));
        }

        var dailyTarget =
          new XPQuery<TAR_DailySale>(uow).Where(
            b => branchNos.Contains(b.Branch.LegacyBranchNum) && b.DisableDate == null).ToList();
        var dailyTargetDto = Mapper.Map<List<TAR_DailySale>, List<TAR_DailySaleDTO>>(dailyTarget);
        foreach (var target in dailyTargetDto)
        {
          RedisConnection.SetStringFromObject(
            string.Format(AssReporting.REDIS_KEY_DAILY_SALES_TARGET, target.Branch.LegacyBranchNum,
              target.TargetDate.ToString("ddMMyyyy")), target, new TimeSpan(24, 0, 0));
        }
      }

      Log.Information(string.Format("[FalconService][Task][{0}] - Finished Query Run Getting Budgets",jobName));
    }

    // TODO: move into target repo
    private static void RunDailyTargets(string jobName)
    {
      Log.Information(string.Format("[FalconService][Task][{0}] - Started Query Run Daily Targets",jobName));
      using (var uow = new UnitOfWork())
      {
        var targets = new XPQuery<TAR_DailySale>(uow).ToList();
        var targetsDto = Mapper.Map<List<TAR_DailySale>, List<TAR_DailySaleDTO>>(targets);
        foreach (var target in targetsDto)
        {
          RedisConnection.SetStringFromObject(
            string.Format(AssReporting.REDIS_KEY_DAILY_SALES_TARGET, target.TargetDate.Day,
              target.TargetDate.ToString("ddMMyyyy")), target, new TimeSpan(24, 0, 0));
        }


        var branchTargets = new XPQuery<TAR_BranchCIMonthly>(uow).ToList();
        var branchTargetsDto =
          Mapper.Map<List<TAR_BranchCIMonthly>, List<TAR_BranchCIMonthlyDTO>>(branchTargets);

        foreach (var btargets in branchTargetsDto)
        {
          // RedisConnection.SetStringFromObject<TAR_BranchCIMonthlyDTO>(string.Format(AssReporting.REDIS_KEY_BRANCH_TARGET,btargets.Branch.BranchId), btargets, new TimeSpan(24, 0, 0));
          RedisConnection.SetStringFromObject(string.Format(AssReporting.REDIS_KEY_BRANCH_TARGET, btargets.Branch.LegacyBranchNum), btargets, new TimeSpan(24, 0, 0));
        }
      }

      Log.Information(string.Format("[FalconService][Task][{0}] - Finished Query RunDaily Targets", jobName));
    }

    private void RunPossibleHandoverQuery(string[] branchNos, string jobName)
    {
      Log.Information(string.Format("[FalconService][Task][{0}] - Started Query Run Possible Handovers", jobName));

      var thisMonthPossibleHandovers = _assCiRepository.RunPossibleHandoverQuery(branchNos.ToArray());
      foreach (var branchNo in branchNos)
      {
        var thisMonthBranchResult = thisMonthPossibleHandovers.Where(b => b.LegacyBranchNumber == branchNo).ToList();
        RedisConnection.SetStringFromObject(
          string.Format(AssReporting.REDIS_KEY_POSSIBLE_HANDOVERS, branchNo), thisMonthBranchResult,
          new TimeSpan(24, 0, 0));
      }

      Log.Information(string.Format("[FalconService][Task][{0}] - Finished Query Run Possible Handovers", jobName));
    }


    private void RunArrearsQuery(string[] branchNos, string jobName)
    {
      Log.Information(string.Format("[FalconService][Task][{0}] - Started Query Run Arrears", jobName));

      var arrears = _assCiRepository.RunArrearsQuery(branchNos.ToArray());
      foreach (var branchNo in branchNos)
      {
        var thisMonthBranchResult = arrears.Where(b => b.LegacyBranchNumber == branchNo).ToList();
        RedisConnection.SetStringFromObject(
          string.Format(AssReporting.REDIS_KEY_ARREARS, branchNo), thisMonthBranchResult, new TimeSpan(24, 0, 0));
      }

      Log.Information(string.Format("[FalconService][Task][{0}] - Finished Query Run Arrears", jobName));
    }

    private void RunCollectionsQuery(string[] branchNos, string jobName)
    {
      Log.Information(string.Format("[FalconService][Task][{0}] - Started Query Run Collections", jobName));

      var collections = _assCiRepository.RunCollectionsQuery(branchNos.ToArray());
      foreach (var branchNo in branchNos)
      {
        var thisMonthBranchResult = collections.Where(b => b.LegacyBranchNumber == branchNo).ToList();
        RedisConnection.SetStringFromObject(
          string.Format(AssReporting.REDIS_KEY_COLLECTIONS, branchNo), thisMonthBranchResult, new TimeSpan(24, 0, 0));
      }

      Log.Information(string.Format("[FalconService][Task][{0}] - Finished Query Run Collections", jobName));
    }


    private void RunDebtorsBookQuery(string[] branchNos, string jobName)
    {
      Log.Information(string.Format("[FalconService][Task][{0}] - Started Query Run Debtors Book", jobName));

      var debtorsBook = _assCiRepository.RunDebtorsBookQuery(branchNos.ToArray());
      foreach (var branchNo in branchNos)
      {
        var thisMonthBranchResult = debtorsBook.Where(b => b.LegacyBranchNumber == branchNo).ToList();
        RedisConnection.SetStringFromObject(
          string.Format(AssReporting.REDIS_KEY_DEBTORS_BOOK, branchNo), thisMonthBranchResult, new TimeSpan(24, 0, 0));
      }

      Log.Information(string.Format("[FalconService][Task][{0}] - Finished Query Run Debtors Book", jobName));
    }


    private void RunLoansFlaggedQuery(string[] branchNos, string jobName)
    {
      Log.Information(string.Format("[FalconService][Task][{0}] - Started Query Run Loans Flagged", jobName));

      var debtorsBook = _assCiRepository.RunLoansFlaggedQuery(branchNos.ToArray());
      foreach (var branchNo in branchNos)
      {
        var thisMonthBranchResult = debtorsBook.Where(b => b.LegacyBranchNumber == branchNo).ToList();
        RedisConnection.SetStringFromObject(
          string.Format(AssReporting.REDIS_KEY_LOANS_FLAGGED, branchNo), thisMonthBranchResult, new TimeSpan(24, 0, 0));
      }

      Log.Information(string.Format("[FalconService][Task][{0}] - Finished Query Run Loans Flagged", jobName));
    }

  }
}
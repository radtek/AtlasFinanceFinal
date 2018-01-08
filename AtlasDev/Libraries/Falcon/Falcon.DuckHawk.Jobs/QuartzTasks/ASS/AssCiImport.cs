using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Ass.Framework.Repository;
using Falcon.Common.Interfaces.Jobs;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Common.Interfaces.Structures;
using Falcon.Common.Structures.Bureau;
using Falcon.DuckHawk.Jobs.Attributes;
using Falcon.TBR.Bureau.Interfaces;
using Quartz;
using Serilog;
using ICompuscanProducts = Falcon.Common.Interfaces.Structures.ICompuscanProducts;

namespace Falcon.DuckHawk.Jobs.QuartzTasks.ASS
{
  //[DisableJob]
  [DisallowConcurrentExecution]
  [JobName("AssCiImport")]
  [TriggerName("AssCiImport")]
  //[CronExpression("0 0/5 * 1/1 * ? *")] // runs every 5 minutes
  [CronExpression("0 0/1 * 1/1 * ? *")] //run every minute for testing purposes
  public class AssCiImport : IAssCiImportJob
  {
    private readonly ILogger _logger;
    private readonly IAssCiRepository _assCiRepository;
    private readonly IAssCiReportRepository _assCiReportRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IBureauRepository _bureauRepository;

    public AssCiImport(ILogger logger, IAssCiRepository assCiRepository, IAssCiReportRepository assCiReportRepository,
      ICompanyRepository companyRepository, IBureauRepository bureauRepository)
    {
      _logger = logger;
      _assCiRepository = assCiRepository;
      _assCiReportRepository = assCiReportRepository;
      _companyRepository = companyRepository;
      _bureauRepository = bureauRepository;
    }

    public void Execute(IJobExecutionContext context)
    {
      try
      {
        _logger.Information("Started Job [AssCiImport]");

        var branchStartDates = _assCiReportRepository.GetBranchLastImportDate().OrderBy(b => b.LastImportDate);

        var endDate = DateTime.Today;

        _logger.Information("[AssCiImport]: Get All Active branches");
        var branches = _companyRepository.GetActiveBranches();

        foreach (var branchStartDate in branchStartDates.Select(b => b.LastImportDate).Distinct())
        {
          var startDate = branchStartDate;
          var branchesMatchingDate = branchStartDates.Where(b => b.LastImportDate == startDate).Select(b => b.BranchId);
          var startDateBranches =
            branches.Where(
              t =>
                branchesMatchingDate.Contains(t.BranchId))
              .ToList();

          if (startDateBranches.Count == 0)
            continue;

          if (startDate.AddYears(3) < DateTime.Today)
          {
            startDate = DateTime.Today.AddYears(-3); // we going 3 years back :D
          }
          if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CiReportImportDateOverride"]))
          {
            if (!DateTime.TryParse(ConfigurationManager.AppSettings["CiReportImportDateOverride"], out startDate))
            {
              _logger.Warning("[AssCiReport]: Import Date not in the correct format");
            }
          }

          while (startDate <= endDate)
          {
            _logger.Information(string.Format("[AssCiReport]: Getting Branches, {0}, data for {1}",
              string.Join("; ", startDateBranches.Select(b => b.LegacyBranchNum)), startDate.ToString("yyyy-MM-dd")));

            ImportAssCiReport(startDateBranches, startDate);

            startDate = startDate.AddDays(1);
          }
        }

        _logger.Information("Finished Job [AssCiImport]");
      }
      catch (Exception ex)
      {
        _logger.Error(string.Format("Error in Job [AssCiImport]: {0} - {1}", ex.Message, ex.StackTrace));
      }
    }

    private void ImportAssCiReport(ICollection<IBranch> branches, DateTime startDate)
    {
      #region Fetch data from Ass

      var basicLoanInfoTask = Task.Run(() =>
      {
        _logger.Information("[AssCiImport]: RunBasicInfoQuery started");
        var result = _assCiRepository.RunBasicInfoQuery(branches.Select(b => b.LegacyBranchNum).Distinct().ToList(), startDate,
          startDate);
        _logger.Information("[AssCiImport]: RunBasicInfoQuery end");
        return result;
      });

      // 30secs
      var clientInfoTask = Task.Run(() =>
      {
        _logger.Information("[AssCiImport]: RunClientInfoQuery started");
        var result = _assCiRepository.RunClientInfoQuery(branches.Select(b => b.LegacyBranchNum).Distinct().ToList(), startDate,
          startDate);
        _logger.Information("[AssCiImport]: RunClientInfoQuery end");
        return result;
      });

      var collectionRefundsTask = Task.Run(() =>
      {
        _logger.Information("[AssCiImport]: RunCollectionRefundQuery started");
        var result = _assCiRepository.RunCollectionRefundQuery(branches.Select(b => b.LegacyBranchNum).Distinct().ToList(), startDate,
          startDate);
        _logger.Information("[AssCiImport]: RunCollectionRefundQuery end");
        return result;
      });

      var vapTask = Task.Run(() =>
      {
        _logger.Information("[AssCiImport]: RunVapQuery started");
        var result = _assCiRepository.RunVapQuery(branches.Select(b => b.LegacyBranchNum).Distinct().ToList(), startDate,
        startDate);
        _logger.Information("[AssCiImport]: RunVapQuery end");
        return result;
      });

      var reswipesTask = Task.Run(() =>
      {
        _logger.Information("[AssCiImport]: RunReswipesQuery started");
        var result = _assCiRepository.RunReswipesQuery(branches.Select(b => b.LegacyBranchNum).Distinct().ToList(),
        startDate, startDate);
        _logger.Information("[AssCiImport]: RunReswipesQuery end");
        return result;
      });

      var rolledAccountTask = Task.Run(() =>
      {
        _logger.Information("[AssCiImport]: RunRolledAccountsQuery started");
        var result = _assCiRepository.RunRolledAccountsQuery(branches.Select(b => b.LegacyBranchNum).Distinct().ToList(), startDate,
          startDate);
        _logger.Information("[AssCiImport]: RunRolledAccountsQuery end");
        return result;
      });

      var bureauProductsTask = Task.Run(() =>
      {
        _logger.Information("[AssCiImport]: Bureau Products started");
        var bureauProducts = _bureauRepository.GetCompuscanProductsSummary(branches.Select(b => b.BranchId).Distinct().ToList(), startDate);
        var compuscanProducts = new List<ICompuscanProducts>();
        compuscanProducts.AddRange(bureauProducts.Select(
            b => new CompuscanProducts
            {
              BranchId = b.BranchId,
              Date = b.Date,
              Declined = b.Declined,
              OneMonth = b.OneMonth,
              OneMThin = b.OneMThin,
              OneMCapped = b.OneMCapped,
              TwoToFourMonths = b.TwoToFourMonths,
              FiveToSixMonths = b.FiveToSixMonths,
              TwelveMonths = b.TwelveMonths
            }).ToList());

        _logger.Information("[AssCiImport]: Bureau Products end");
        return compuscanProducts;
      });

      #endregion

      #region Persist into DB

      _assCiReportRepository.ImportCiReport(branches, startDate, basicLoanInfoTask.GetAwaiter().GetResult(), collectionRefundsTask.GetAwaiter().GetResult(), clientInfoTask.GetAwaiter().GetResult(),
        reswipesTask.GetAwaiter().GetResult(), rolledAccountTask.GetAwaiter().GetResult(), vapTask.GetAwaiter().GetResult());

      _assCiReportRepository.ImportCiReportScore(branches, startDate, basicLoanInfoTask.GetAwaiter().GetResult());

      _assCiReportRepository.ImportCiReportBureauProducts(branches, startDate, bureauProductsTask.GetAwaiter().GetResult());

      #endregion
    }
  }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Ass.Framework.Repository;
using Atlas.Ass.Framework.Structures;
using Falcon.Common.Interfaces.Jobs;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Common.Interfaces.Structures;
using Falcon.DuckHawk.Jobs.Attributes;
using Quartz;
using Serilog;

namespace Falcon.DuckHawk.Jobs.QuartzTasks.ASS
{
  //[DisableJob]
  [DisallowConcurrentExecution]
  [JobName("AssCiImportPossibleHandover")]
  [TriggerName("AssCiImportPossibleHandover")]
  //[CronExpression("0 0/10 * 1/1 * ? *")] // runs every 10 minutes
  [CronExpression("0 0/1 * 1/1 * ? *")] //run every minute for testing purposes
  public class AssCiImportPossibleHandover : IAssCiImportPossibleHandoverJob
  {
    private readonly ILogger _logger;
    private readonly IAssCiRepository _assCiRepository;
    private readonly IAssCiReportRepository _assCiReportRepository;
    private readonly ICompanyRepository _companyRepository;

    public AssCiImportPossibleHandover(ILogger logger, IAssCiRepository assCiRepository, IAssCiReportRepository assCiReportRepository,
      ICompanyRepository companyRepository)
    {
      _logger = logger;
      _assCiRepository = assCiRepository;
      _assCiReportRepository = assCiReportRepository;
      _companyRepository = companyRepository;
    }

    public void Execute(IJobExecutionContext context)
    {
      try
      {
        _logger.Information("Started Job [AssCiImportPossibleHandover]");

        var branchStartDates = _assCiReportRepository.GetBranchLastImportDate().OrderBy(b => b.LastImportDate);

        var endDate = DateTime.Today;

        _logger.Information("[AssCiImportPossibleHandover]: Get All Active branches");
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
              _logger.Warning("[AssCiImportPossibleHandover]: Import Date not in the correct format");
            }
          }

          while (startDate <= endDate)
          {
            _logger.Information(string.Format("[AssCiImportPossibleHandover]: Getting Branches, {0}, data for {1}",
              string.Join("; ", startDateBranches.Select(b => b.LegacyBranchNum)), startDate.ToString("yyyy-MM-dd")));

            ImportAssCiReport(startDateBranches, startDate);

            startDate = startDate.AddDays(1);
          }
        }

        _logger.Information("Finished Job [AssCiImportPossibleHandover]");
      }
      catch (Exception ex)
      {
        _logger.Error(string.Format("Error in Job [AssCiImportPossibleHandover]: {0} - {1}", ex.Message, ex.StackTrace));
      }
    }

    private void ImportAssCiReport(ICollection<IBranch> branches, DateTime startDate)
    {
      #region Fetch data from Ass

      var handOverInfosTask = Task.Run(() =>
      {
        _logger.Information("[AssCiImportPossibleHandover]: RunHandoverInfoQuery started");
          ICollection<IHandoverInfo_New> result = new List<IHandoverInfo_New>();
          try
          {
              result = _assCiRepository.RunNewHandoverInfoQuery(branches.Select(b => b.LegacyBranchNum).Distinct().ToList(),
                  startDate.AddMonths(-3),
                  startDate);
              _logger.Information("[AssCiImportPossibleHandover]: RunHandoverInfoQuery end");
          }
          catch (Exception exception)
            {
                _logger.Error(string.Format("Error in Job [AssCiImportPossibleHandover]: RunHandoverInfoQuery {0} - {1}", exception.Message, exception.StackTrace));
            }
        return result;
      });

      if (startDate.Date == DateTime.Today.Date)
      {
        // 30 secs
        var possibleHandoverTask = Task.Run(() =>
        {
          _logger.Information("[AssCiImportPossibleHandover]: RunPossibleHandoverQuery started");
            ICollection<IPossibleHandover> result = new List<IPossibleHandover>();
            try
            {
                result = _assCiRepository.RunPossibleHandoverQuery(branches.Select(b => b.LegacyBranchNum).Distinct().ToList());
                _logger.Information("[AssCiImportPossibleHandover]: RunPossibleHandoverQuery end");
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Error in Job [AssCiImportPossibleHandover]: RunPossibleHandoverQuery {0} - {1}", exception.Message, exception.StackTrace));
            }
          return result;
        });

        // 30 secs
        var arrearsTask = Task.Run(() =>
        {
          _logger.Information("[AssCiImportPossibleHandover]: RunArrearsQuery started");
            ICollection<IArrears> result = new List<IArrears>();
            try
            {
                result = _assCiRepository.RunArrearsQuery(branches.Select(b => b.LegacyBranchNum).Distinct().ToList());
                _logger.Information("[AssCiImportPossibleHandover]: RunArrearsQuery end");
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Error in Job [AssCiImportPossibleHandover]: RunArrearsQuery {0} - {1}", exception.Message, exception.StackTrace));
            }
          return result;
        });
        
        var collectionsTask = Task.Run(() =>
        {
          _logger.Information("[AssCiImportPossibleHandover]: RunCollectionsQuery started");
            ICollection<ICollections> result =new List<ICollections>();
            try
            {
                result = _assCiRepository.RunCollectionsQuery(branches.Select(b => b.LegacyBranchNum).Distinct().ToList());
                _logger.Information("[AssCiImportPossibleHandover]: RunCollectionsQuery end");
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Error in Job [AssCiImportPossibleHandover]: RunCollectionsQuery {0} - {1}", exception.Message, exception.StackTrace));
            }
          return result;
        });

        var debtorsBookTask = Task.Run(() =>
        {
          _logger.Information("[AssCiImportPossibleHandover]: RunDebtorsBookQuery started");
            ICollection<IDebtorsBook> result = new List<IDebtorsBook>();
            try
            {
                result = _assCiRepository.RunDebtorsBookQuery(branches.Select(b => b.LegacyBranchNum).Distinct().ToList());
                _logger.Information("[AssCiImportPossibleHandover]: RunDebtorsBookQuery end");
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Error in Job [AssCiImportPossibleHandover]: RunDebtorsBookQuery {0} - {1}", exception.Message, exception.StackTrace));
            }
          return result;
        });

        var loansFlaggedTask = Task.Run(() =>
        {
            _logger.Information("[AssCiImportPossibleHandover]: RunLoansFlaggedQuery started");
            ICollection<ILoansFlagged> result =new List<ILoansFlagged>();
            try
            {
                result = _assCiRepository.RunLoansFlaggedQuery(branches.Select(b => b.LegacyBranchNum).Distinct().ToList());
                _logger.Information("[AssCiImportPossibleHandover]: RunLoansFlaggedQuery end");
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Error in Job [AssCiImportPossibleHandover]: RunLoansFlaggedQuery {0} - {1}", exception.Message, exception.StackTrace));
            }
          return result;
        });

        _assCiReportRepository.ImportCiReportPossibleHandover(branches, startDate, possibleHandoverTask.GetAwaiter().GetResult(), arrearsTask.GetAwaiter().GetResult(),
          debtorsBookTask.GetAwaiter().GetResult(), loansFlaggedTask.GetAwaiter().GetResult(), collectionsTask.GetAwaiter().GetResult());
      }

      #endregion

      #region Persist into DB

      _assCiReportRepository.ImportCiReportHandoverInfo(branches, handOverInfosTask.GetAwaiter().GetResult());

      #endregion
    }
  }
}
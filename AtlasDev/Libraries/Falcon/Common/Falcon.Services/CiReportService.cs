using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Atlas.Common.Extensions;
using Atlas.Reporting.DTO;
using AutoMapper;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Common.Interfaces.Services;
using Falcon.Common.Interfaces.Structures;
using Falcon.Exporter.CiReport.Infrastructure.Interfaces;
using Falcon.Exporter.CiReport.Infrastructure.Interfaces.Models;
using Falcon.Exporter.CiReport.Infrastructure.Structures.Models;
using Falcon.Exporter.CiReport.Infrastructure.Structures.Reports;
using Serilog;

namespace Falcon.Common.Services
{
  public class CiReportService : ICiReportService
  {
    private float _meanLimit = 80;

    private readonly IAssCiReportRepository _assCiReportRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly ILogger _logger;

    public CiReportService(IAssCiReportRepository assCiReportRepository, ICompanyRepository companyRepository,
      ILogger logger)
    {
      _assCiReportRepository = assCiReportRepository;
      _companyRepository = companyRepository;
      _logger = logger;

      Mapper.CreateMap<CiReport, ASS_CiReportSales>().ReverseMap();
      Mapper.CreateMap<PossibleHandover, ASS_CiReportPossibleHandovers>().ReverseMap();
      Mapper.CreateMap<CiReportScore, ASS_CiReportCompuscanScores>().ReverseMap();
      Mapper.CreateMap<CiReportBranchSyncStatus, ASS_CiReportBranchSyncStatus>().ReverseMap();
      Mapper.CreateMap<CiReportBranchSummary, ASS_CiReportBranchSummary>().ReverseMap();
      Mapper.CreateMap<CiReportLowMean, ASS_CiReportLowMean>().ReverseMap();
      Mapper.CreateMap<VarianceStatus, ASS_CiReportLowMean>().ReverseMap();
    }

    public byte[] GetCiReport(DateTime startDate, DateTime endDate, List<long> branchIds)
    {
      try
      {
        _logger.Debug("GetCiReport entry point");
        var ciReportVersion =
          _assCiReportRepository.GetCiReportVersion(startDate, endDate).OrderBy(r => r.VersionDate).FirstOrDefault();
        if (ciReportVersion == null)
        {
          throw new Exception("Cannot find report Version for specified date range");
        }

        var ciReportExporter = ActivateExporter(ciReportVersion.ExporterLocation,
          ciReportVersion.ExporterLocationAbsoluteClassName);

        if (ciReportExporter == null)
        {
          throw new Exception("Exporter not found");
        }

        _logger.Debug("GetCiReport exporter activated");
        var branches = _companyRepository.GetBranchesByIds(branchIds);

        var ciReportGrouping = branches.Select(r => r.RegionId).Distinct().Count() == 1
          ? CiReportGrouping.Branch
          : CiReportGrouping.Region;
        _logger.Debug(" GetCiReport branches received");

        var sales = new List<ASS_CiReportSales>();
        var reportLowMean = new List<ASS_CiReportLowMean>();
        var scores = new List<ASS_CiReportCompuscanScores>();
        var branchSummaries = new List<ASS_CiReportBranchSummary>();
        var lastBranchSyncStatuses = new List<CiReportBranchSyncStatus>();
        var branchSyncTask = Task.Run(() =>
        {
          lastBranchSyncStatuses =
            Mapper.Map<List<ASS_CiReportBranchSyncStatus>, List<CiReportBranchSyncStatus>>(
              GetBranchSyncStatuses(branchIds).ToList());
        });


        var salesTask = Task.Run(() =>
        sales = _assCiReportRepository.GetCiReportSales(branchIds, startDate, endDate, ciReportGrouping));

        var reportLowMeanTask = Task.Run(() =>
        reportLowMean = _assCiReportRepository.GetCiReportLowMeans(branchIds, startDate, endDate, _meanLimit));

        var scoresTask = Task.Run(() => scores = _assCiReportRepository.GetCiReportScores(branchIds, startDate, endDate,
          ciReportGrouping));

        var branchSummariesTask = Task.Run(() => branchSummaries = _assCiReportRepository.GetCiReportBranchSummaries(branchIds, startDate, endDate));
        Task<List<ASS_CiReportPossibleHandovers>> possibleHandoversTask = null;

        var possibleHandovers = new List<ASS_CiReportPossibleHandovers>();
        if (ciReportGrouping != CiReportGrouping.Branch)
        {
          // if the endDate is end of current month, then default to today, since we cannot see into the future
          possibleHandoversTask = Task.Run(() =>
          possibleHandovers = _assCiReportRepository.GetCiReportPossibleHandovers(branches,
            endDate >= DateTime.Today ? DateTime.Today : endDate, ciReportGrouping));

        }

        salesTask.Wait();
        reportLowMeanTask.Wait();
        scoresTask.Wait();
        branchSummariesTask.Wait();
        if (possibleHandoversTask != null)
        {
          possibleHandoversTask.Wait();
        }
        _logger.Debug("GetCiReport waiting for branch tasks");

        var worksheets = GetLoop(branches, ciReportGrouping).Select(item => new ExportCiReportModelWorksheet
        {
          Order = item.Key,
          Description = item.Value,
          CiReportScores =
            Mapper.Map<List<ASS_CiReportCompuscanScores>, List<CiReportScore>>(
              scores.Where(b => b.Id == item.Key).OrderBy(r => r.PayNo == 0 ? 999 : r.PayNo).ToList()),
          PossibleHandovers =
            Mapper.Map<List<ASS_CiReportPossibleHandovers>, List<PossibleHandover>>(
              possibleHandovers.Where(b => b.ParentId == item.Key).ToList()),
          CiReports =
            Mapper.Map<List<ASS_CiReportSales>, List<CiReport>>(
              sales.Where(b => b.Id == item.Key).OrderBy(r => r.PayNo == 0 ? 999 : r.PayNo).ToList()),
          VarianceStatus = new VarianceStatus
          {

            AchievedPercent =
              sales.Where(b => b.Id == item.Key).Sum(c => c.Target) > 0
                ? sales.Where(b => b.Id == item.Key).Sum(c => c.Cheque) /
                  sales.Where(b => b.Id == item.Key).Sum(c => c.Target)
                : 0,
            Variance =
              sales.Where(b => b.PayNo == 0 && b.Id == item.Key).Sum(c => c.Cheque) -
              sales.Where(b => b.PayNo == 0 && b.Id == item.Key).Sum(c => c.Target)
          }
        }).Cast<IExportCiReportModelWorksheet>().ToList();

        _logger.Debug("GetCiReport starting consolidation section");
        // consolidation
        var consolidatedCiReportGrouping = (CiReportGrouping)(ciReportGrouping.ToInt() + 1);

        salesTask =
            Task.Run(() => sales =
              _assCiReportRepository.GetCiReportSales(branchIds, startDate, endDate, consolidatedCiReportGrouping));

        scoresTask = Task.Run(() =>
          scores = _assCiReportRepository.GetCiReportScores(branchIds, startDate, endDate,
            consolidatedCiReportGrouping));

        possibleHandoversTask = Task.Run(() =>
        possibleHandovers = _assCiReportRepository.GetCiReportPossibleHandovers(branches,
          endDate >= DateTime.Today ? DateTime.Today : endDate, consolidatedCiReportGrouping).ToList());

        if (consolidatedCiReportGrouping == CiReportGrouping.None)
        {
          possibleHandovers = possibleHandovers.OrderBy(b => b.Id == 0 ? 999 : b.Id).ToList();
        }

        salesTask.Wait();
        scoresTask.Wait();
        possibleHandoversTask.Wait();
        _logger.Debug("GetCiReport waiting for consolidation tasks");
        var consolidatedModelWorkSheet = new ExportCiReportModelWorksheet
        {
          Order = 0,
          Description = "Consolidated",
          CiReportScores =
            Mapper.Map<List<ASS_CiReportCompuscanScores>, List<CiReportScore>>(
              scores.OrderBy(r => r.PayNo == 0 ? 999 : r.PayNo).ToList()),
          PossibleHandovers =
            Mapper.Map<List<ASS_CiReportPossibleHandovers>, List<PossibleHandover>>(
              possibleHandovers.ToList()),
          CiReports =
            Mapper.Map<List<ASS_CiReportSales>, List<CiReport>>(
              sales.OrderBy(r => r.PayNo == 0 ? 999 : r.PayNo).ToList()),
          VarianceStatus = new VarianceStatus
          {
            AchievedPercent = sales.Sum(c => c.Target) > 0 ? sales.Sum(c => c.Cheque) / sales.Sum(c => c.Target) : 0,
            Variance = sales.Where(a => a.PayNo == 0).Sum(c => c.Cheque) - sales.Where(a => a.PayNo == 0).Sum(c => c.Target)
          }
        };
        worksheets.Add(consolidatedModelWorkSheet);

        _logger.Debug("GetCiReport started branch sync status");
        branchSyncTask.Wait();
        // Branch Sync Status
        var branchSyncStatusWorksheet = new ExportCiReportModelWorksheet
        {
          Order = 99998, //make sure its the second last sheet
          Description = "Branch Sync Status",
          LastBranchSyncStatuses = lastBranchSyncStatuses
        };
        worksheets.Add(branchSyncStatusWorksheet);

        // Branch Summary
        _logger.Debug("GetCiReport started branch summary");
        var branchSummaryWorksheet = new ExportCiReportModelWorksheet
        {
          Order = 99999, //make sure its the last sheet
          Description = "Branch Summary",
          CiReportBranchSummaries =
            Mapper.Map<List<ASS_CiReportBranchSummary>, List<CiReportBranchSummary>>(branchSummaries)
        };
        worksheets.Add(branchSummaryWorksheet);

        _logger.Debug("GetCiReport started low mean");

        // Low Mean
        var lowMeanWorksheet = new ExportCiReportModelWorksheet
        {
          Order = -2,
          Description = string.Format("Below {0}%", _meanLimit),
          BranchesLowMeans =
            Mapper.Map<List<ASS_CiReportLowMean>, List<CiReportLowMean>>(
              reportLowMean.Where(r => r.AchievedPercent < 0.8).OrderBy(r => r.AchievedPercent).ToList())
        };
        worksheets.Add(lowMeanWorksheet);
        _logger.Debug("GetCiReport exporting");

        return ciReportExporter.ExportCiReport(new ExportCiReportModel
        {
          Worksheets = worksheets,
          StartDate = startDate,
          EndDate = endDate
        }, _logger);
      }
      catch (Exception exception)
      {
        _logger.Error(string.Format("CI Report Export Error: {0} - {1}", exception.Message, exception.StackTrace));
        throw;
      }
    }

    private ICollection<ASS_CiReportBranchSyncStatus> GetBranchSyncStatuses(ICollection<long> branchIds)
    {
      var branchSyncStatuses = _companyRepository.GetBranchSyncStatus(branchIds);
      return branchSyncStatuses.Select(b => new ASS_CiReportBranchSyncStatus
      {
        Name = b.BranchName,
        LastUpdatDate = b.LastSyncDate
      }).ToList();
    }

    private static Dictionary<long, string> GetLoop(IEnumerable<IBranch> branches, CiReportGrouping ciReportGrouping)
    {
      var loopData = ciReportGrouping == CiReportGrouping.Branch
        ? branches.Select(b => new { Id = b.BranchId, b.Name }).Distinct().ToDictionary(t => t.Id, t => t.Name)
        : branches.Select(b => new { Id = b.RegionId, Name = b.Region }).Distinct().ToDictionary(t => t.Id, t => t.Name);

      return loopData;
    }

    private static IExporter ActivateExporter(string path, string className)
    {
      try
      {
        var activationType = Assembly.LoadFrom(path).GetType(className);
        return (IExporter)Activator.CreateInstance(activationType);
      }
      catch (Exception)
      {
        return default(IExporter);
      }
    }
  }
}
using System;
using System.Configuration;
using System.Linq;
using Atlas.Ass.Repository;
using Atlas.Domain.Model;
using DevExpress.Xpo;
using Falcon.Common.Services;
using Falcon.Service.Business.Reporting;
using Quartz;
using Serilog;

namespace Falcon.Service.Tasks
{
  [DisallowConcurrentExecution]
  public class AssCIReportPastMonthsTask : IJob
  {
    private static readonly ILogger Log = Serilog.Log.Logger.ForContext<AssCIReportPastMonthsTask>();

    private readonly string _connectionString = ConfigurationManager.ConnectionStrings["AssConnection"] != null
      ? ConfigurationManager.ConnectionStrings["AssConnection"].ConnectionString
      : string.Empty;

    private readonly CIReportTaskHelpers _ciReportTaskHelpers;

    public AssCIReportPastMonthsTask()
    {
      var configService = new ConfigService();
      _ciReportTaskHelpers = new CIReportTaskHelpers(new AssCiRepository(configService, Log), new AssBureauRepository(configService));
    }

    public void Execute(IJobExecutionContext context)
    {
      if (!string.IsNullOrEmpty(_connectionString))
      {
        Log.Information("[FalconService][Task] {Job} Executing...", context.JobDetail.Key.Name);
        try
        {
          string[] branchNos;
          var allowedRegions = new long[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14};
          var notAllowedBranches = new long[] {1, 18, 21, 84, 111, 113, 149, 152, 167, 207};
          //var hoursToRunJobs = new[] { 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };

          // can be removed once regions have been unlinked properly
          using (var uow = new UnitOfWork())
          {
            var branches =
              new XPQuery<BRN_Branch>(uow).Where(
                b => b.BranchId > 1 && !b.IsClosed && // Skip HO and closed
                     allowedRegions.Contains(b.Region.RegionId) &&
                     !notAllowedBranches.Contains(b.BranchId))
                .Select(b => new {b.BranchId, b.LegacyBranchNum})
                .ToArray();
            branchNos = branches.Select(b => b.LegacyBranchNum).ToArray();
          }


          var startDate = DateTime.Today.AddDays(-DateTime.Today.Day + 1);
          var endDate = startDate.AddMonths(1).AddDays(-1);

            for (var i = 1; i <= 6; i++)
            {
              endDate = startDate.AddDays(-1);
              startDate = startDate.AddMonths(-1);
              _ciReportTaskHelpers.RunClientInfoQuery(branchNos, startDate, endDate, context.JobDetail.Key.Name,
                new TimeSpan(23, 45, 0));
              _ciReportTaskHelpers.RunVapQuery(branchNos, startDate, endDate, context.JobDetail.Key.Name,
                new TimeSpan(23, 45, 0));
              _ciReportTaskHelpers.RunReswipeInfoQuery(branchNos, startDate, endDate, context.JobDetail.Key.Name,
                new TimeSpan(23, 45, 0));
              _ciReportTaskHelpers.RunBasicInfoQuery(branchNos, startDate, endDate, context.JobDetail.Key.Name,
                new TimeSpan(23, 45, 0));
              _ciReportTaskHelpers.RunRolledAccounts(branchNos, startDate, endDate, context.JobDetail.Key.Name,
                new TimeSpan(23, 45, 0));
            }


          Log.Information("[FalconService][Task] {Job} Completed", context.JobDetail.Key.Name);
        }
        catch (Exception ex)
        {
          Log.Error("[FalconService][Task] {Job} Error: {Error}", context.JobDetail.Key.Name, ex.Message);
        }
      }
    }
  }
}

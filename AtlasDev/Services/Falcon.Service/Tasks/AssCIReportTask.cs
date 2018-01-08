using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Atlas.Ass.Repository;
using DevExpress.Spreadsheet;
using DevExpress.Xpo;
using Magnum;
using Quartz;
using Serilog;
using Atlas.Common.Utils;
using Atlas.Domain.Model;
using Atlas.Enumerators;
using Atlas.RabbitMQ.Messages.Notification;
using Falcon.Common.Services;
using Falcon.Service.Business.Reporting;
using Falcon.Service.Core;


namespace Falcon.Service.Tasks
{
  [DisallowConcurrentExecution]
  public class AssCiReportTask : IJob
  {
    private static readonly ILogger Log = Serilog.Log.Logger.ForContext<AssCiReportTask>();

    private readonly string _connectionString = ConfigurationManager.ConnectionStrings["AssConnection"] != null
      ? ConfigurationManager.ConnectionStrings["AssConnection"].ConnectionString
      : string.Empty;

    private readonly CIReportTaskHelpers _ciReportTaskHelpers;

    // DI does not work, because it was not implemented properly - Falcon service will be dead soon anyway
    public AssCiReportTask()
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
          long[] branchIds;
          var allowedRegions = new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };
          var notAllowedBranches = new long[] { 1, 18, 21, 84, 111, 113, 149, 152, 167, 207 };
          var toEmailGroup = ConfigurationManager.AppSettings["cidistributionlist"] ?? "lee@atcorp.co.za";

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
            branchIds = branches.Select(b => b.BranchId).ToArray();
          }

          var isMonthlyUpdate = bool.Parse(ConfigurationManager.AppSettings["monthlyTargetsUpdate"] ?? "false");

          if (isMonthlyUpdate)
          {
            ExportBranchBudgets(context.JobDetail.Key.Name);
            ExportDailyTargets(context.JobDetail.Key.Name);
          }
          
          var startDate = DateTime.Today.AddDays(-DateTime.Today.Day + 1);
          var endDate = startDate.AddMonths(1).AddDays(-1);

          _ciReportTaskHelpers.RunClientInfoQuery(branchNos, startDate, endDate, context.JobDetail.Key.Name);
          _ciReportTaskHelpers.RunVapQuery(branchNos, startDate, endDate, context.JobDetail.Key.Name);
          _ciReportTaskHelpers.RunBasicInfoQuery(branchNos, startDate, endDate, context.JobDetail.Key.Name);
          _ciReportTaskHelpers.RunRolledAccounts(branchNos, startDate, endDate, context.JobDetail.Key.Name);
          _ciReportTaskHelpers.RunCompuScanProducts(branchIds, startDate, endDate, context.JobDetail.Key.Name);
          _ciReportTaskHelpers.RunLastBranchSyncDate(branchNos, context.JobDetail.Key.Name);
          _ciReportTaskHelpers.RunReswipeInfoQuery(branchNos, startDate, endDate, context.JobDetail.Key.Name);

          if (DateTime.Now.Hour == 20 || DateTime.Now.Hour == 19 || DateTime.Now.Hour == 7 || DateTime.Now.Hour == 11 || DateTime.Now.Hour == 13)
          {
            Log.Information("[FalconService][Task] {Job} Sending Reports via Email", context.JobDetail.Key.Name);

            if (RedisConnection.GetObjectFromString<DateTime?>(AssReporting.REDIS_KEY_EMAIL_SENT) == null)
            {
              if (DateTime.Now.Hour == 19 || DateTime.Now.Hour == 11)
              {
                startDate = endDate = DateTime.Today;
              }
              else
              {
                startDate = DateTime.Today.AddDays(-DateTime.Today.Day + 1);
                endDate = startDate.AddMonths(1).AddDays(-1);
              }
              var attachment = new List<Tuple<string, string, string>>();
              using (var uow = new UnitOfWork())
              {
                for (var i = 0; i <= allowedRegions.Length; i++)
                {
                  var tempRegions = new List<long>();
                  if (i == allowedRegions.Length)
                    tempRegions = allowedRegions.ToList();
                  else
                    tempRegions.Add(allowedRegions[i]);
                  var branches =
                    new XPQuery<BRN_Branch>(uow).Where(
                      b => b.BranchId > 1 && !b.IsClosed && 
                        tempRegions.Contains(b.Region.RegionId) && !notAllowedBranches.Contains(b.BranchId))
                      .Select(b => new { b.BranchId, b.LegacyBranchNum })
                      .ToArray();
                  
                  if (branches.Any())
                  {                   
                    var extract = new CiExtract().ExportCiReport(branches.Select(b => b.BranchId).ToList(), startDate,
                      endDate, Log, true);
                    var data = Base64.EncodeString(extract);
                    var branch = branches.FirstOrDefault();
                    if (branch != null)
                      attachment.Add(
                        new Tuple<string, string, string>(
                          string.Format("CIReport_{0}_{1}_{2}",
                            i == allowedRegions.Length ? "AllRegions" : branch.LegacyBranchNum,
                            startDate.ToString("ddMMyyyy"), endDate.ToString("ddMMyyyy")), ".xlsx", data));
                  }
                }
              }

              MqBus.Bus().Publish(new EmailNotifyMessage(CombGuid.Generate())
                {
                  CreatedAt = DateTime.Now,
                  ActionDate = DateTime.Today,
                  Body = string.Format("Hi, {0}{0}Please see attached CI Report.{0}{0}Regards,{0}Falcon", Environment.NewLine),
                  From = "falcon@atcorp.co.za",
                  IsHTML = false,
                  Attachments = attachment,
                  Priority = Notification.NotificationPriority.High,
                  Subject =
                    string.Format("CI Report dated: {0} to {1}", startDate.ToString("dd MMM yyyy"),
                      endDate.ToString("dd MMM yyyy")),
                  To = toEmailGroup
                });

              RedisConnection.SetStringFromObject<DateTime?>(AssReporting.REDIS_KEY_EMAIL_SENT, DateTime.Now, new TimeSpan(1, 0, 0));

              Log.Information("[FalconService][Task] {Job} Sent Reports via Email", context.JobDetail.Key.Name);
            }
            else
            {
              Log.Information("[FalconService][Task] {Job} Already Sent Reports via Email", context.JobDetail.Key.Name);
            }
          }

          Log.Information("[FalconService][Task] {Job} Completed", context.JobDetail.Key.Name);
        }
        catch (Exception ex)
        {
          Log.Error("[FalconService][Task] {Job} Error: {Error}", context.JobDetail.Key.Name, ex.Message);
        }
      }
    }

    //19.06.2015 - Exporting monthly sales targets into database
    private static void ExportDailyTargets(string jobName)
    {
      Log.Information(string.Format("[FalconService][Task][{0}] - Started importing monthly targets to database", jobName));
      IWorkbook workbook = new Workbook();

      // Load a workbook from a stream. 
      var dailyTargets = new Dictionary<int, float>();
      var j = 7;
      using (var stream = new FileStream("MonthlyTargetsCI.xlsx", FileMode.Open))
      {
        workbook.LoadDocument(stream, DocumentFormat.OpenXml);
        var ws = workbook.Worksheets[1];
        var rc = ws.Rows;
        var firstRow = rc[0];
        for (var i = 1; i <= 31; i++)
        {
          var dayPer = string.IsNullOrEmpty(firstRow[j].Value.ToString()) ? "0.0" : firstRow[j].Value.ToString();
          dailyTargets.Add(i, (Convert.ToSingle(dayPer)) * 100);
          j++;
        }
      }

      using (var uow = new UnitOfWork())
      {
        //delete old records
        var oldData = new XPQuery<TAR_DailySale>(uow);
        foreach (var o in oldData)
        {
          uow.Delete(o);
          //uow.PurgeDeletedObjects();
        }

        uow.CommitChanges();
        //Insert new month data
        var sysUser =
          new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == (int)General.Person.System);
        foreach (var items in dailyTargets)
        {
          new TAR_DailySale(uow)
          {
            TargetDate = new DateTime(DateTime.Today.Year, DateTime.Today.AddMonths(1).Month, items.Key),
            Percent = items.Value,
            CreateUser = sysUser
          };
          uow.CommitChanges();
        }
      }
      Log.Information(string.Format("[FalconService][Task][{0}] - Finishing importing monthly targets to database", jobName));
    }

    //19.06.2015 - Exporting monthly sales targets into database
    private static void ExportBranchBudgets(string jobName)
    {
      Log.Information(string.Format("[FalconService][Task][{0}] - Started importing BranchBudgets to database", jobName));
      IWorkbook workbook = new Workbook();
      // Load a workbook from a stream. 
      var branchList = new Dictionary<string, float>();
      using (var stream = new FileStream("MonthlyTargetsCI.xlsx", FileMode.Open))
      {
        workbook.LoadDocument(stream, DocumentFormat.OpenXml);
        var ws = workbook.Worksheets[1];
        var branchesColumn = ws.Columns["F"];
        var budgetColumn = ws.Columns["AR"];
        for (var i = 15; i < 300; i++)
        {
          if (!string.IsNullOrEmpty(branchesColumn[i].Value.ToString()))
          {
            var budget = string.IsNullOrEmpty(budgetColumn[i].Value.ToString()) ? "0.0" : budgetColumn[i].Value.ToString();
            branchList.Add(branchesColumn[i].Value.ToString(), Convert.ToSingle(budget));
          }
        }

        using (var uow = new UnitOfWork())
        {
          //delete old records
          var oldData = new XPQuery<TAR_BranchCIMonthly>(uow);
          foreach (var o in oldData)
          {
            uow.Delete(o);
          }
          //uow.CommitChanges();
          
          var branches = new XPQuery<BRN_Branch>(uow).Where(s => s.BranchId > 1 && !s.IsClosed).Select(s => new { s.BranchId, s.LegacyBranchNum }).ToList();
          var branchCodeLu = branches.ToDictionary(branch => BRN_Branch.ASSBranchCodeToGL(branch.LegacyBranchNum).TrimStart('0'), branch => branch.BranchId);

          //Insert new month data
          var sysUser =
            new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == (int)General.Person.System);

          foreach (var branch in branchList)
          {
            var key = branch.Key.TrimStart('0');
            var branchNum = branchCodeLu.ContainsKey(key) ? branchCodeLu[key] : 0;
            if (branchNum == 0)
            {
              Log.Error("Unable to locate {0}", key);
            }
            else
            {
              var branchDb = new XPQuery<BRN_Branch>(uow).FirstOrDefault(s => s.BranchId == branchNum);
              new TAR_BranchCIMonthly(uow)
              {
                TargetDate = DateTime.Now,
                Branch = branchDb,
                CreateUser = sysUser,
                Amount = (decimal)branch.Value
              };

              if (branchDb != null)
                RedisConnection.SetStringFromObject(
                  string.Format(AssReporting.REDIS_KEY_BRANCH_80_PERC_LESS_SYNC_DATA, branchNum),
                  new Tuple<string, string, float>(branchDb.BranchId.ToString(), branchDb.Company.Name,
                    Convert.ToSingle(branch.Value)), new TimeSpan(23, 30, 0));
            }
          }

          uow.CommitChanges();
        }

        Log.Information(string.Format("[FalconService][Task][{0}] - Finishing importing BranchBudgets to database", jobName));
      }
    }
  }
}
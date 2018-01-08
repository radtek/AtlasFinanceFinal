/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013-2015 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Export DBF ZIP files to downloadable links
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     31 May 2013 - Created
 * 
 * 
 *  Comments:
 *  ------------------
 *   
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Mail;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

using DevExpress.Xpo;

using Atlas.Common.Extensions;
using Atlas.Domain.Model;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Common.Utils;
using ASSServer.WCF;
using ASSServer.Utils.PSQL;
using Atlas.Cache.Interfaces.Classes;
using Atlas.Cache.DataUtils;


namespace ASSServer.QuartzTasks
{
  [global::Quartz.DisallowConcurrentExecution]
  public class ExportDBFLinksDaily : global::Quartz.IJob
  {
    public ExportDBFLinksDaily(ILogging log, IConfigSettings config, ICacheServer cache)
    {
      _log = log;
      _config = config;
      _cache = cache;
    }


    /// <summary>
    /// Main Quartz method
    /// </summary>
    /// <param name="context"></param>
    public void Execute(global::Quartz.IJobExecutionContext context)
    {
      var methodName = "ExportDBFLinksDaily.Execute";
      var timeProcess = Stopwatch.StartNew();
      try
      {
        _log.Information("{MethodName} starting", methodName);

        var rootHTTPPath = _config.GetCustomSetting("", "WebServerHTTPPath", false) ?? "http://172.31.75.38/Software/Atlas/Branch/";
        var rootWebServerPath = _config.GetCustomSetting("", "WebServerRootPath", false) ?? "D:\\Websites\\AtlasSoftware\\Atlas\\Branch";

        var pendingBranches = new ConcurrentQueue<string>();
        var activeBranchIds = _cache.GetAll<ASS_BranchServer_Cached>().Where(s => s.Branch != null).Select(s => s.Branch.Value).ToList();
        var branchList = _cache.Get<BRN_Branch_Cached>(activeBranchIds)        
          .Select(s => s.LegacyBranchNum.PadLeft(3, '0')).ToList();
        var currVer = CacheUtils.GetCurrentDbVersion(_cache).DbUpdateScriptId;

        foreach (var branch in branchList)
        {
          pendingBranches.Enqueue(branch);
        }

        var branchLinks = new ConcurrentBag<Tuple<string, string>>(); // branch, http link

        var tasks = new Task[TASK_COUNT];
        for (var i = 0; i < TASK_COUNT; i++)
        {
          tasks[i] = Task.Run(() =>
            {
              string branch;
              while (pendingBranches.TryDequeue(out branch))
              {
                var branchTimer = Stopwatch.StartNew();

                try
                {
                  _log.Information("{Branch} Export DBF ZIP data started", branch);

                  #region Try export the branch to ZIP DBF
                  var transactionId = Guid.NewGuid().ToString("N");
                  ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Started);

                  ExportBranchToZipDBF.Execute(_log, _cache, _config, branch.ToLower(), transactionId, currVer);

                  string errorMessage = null;
                  string fileName = null;
                  var status = ProcessTracking.CurrentStatus.NotSet;
                  while (status == ProcessTracking.CurrentStatus.Started || status == ProcessTracking.CurrentStatus.NotSet)
                  {
                    if (branchTimer.Elapsed > TimeSpan.FromMinutes(20))
                    {
                      ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, "Export process timed-out!");
                    }

                    Thread.Sleep(1000);
                    ProcessTracking.GetTransactionState(transactionId, out status, out errorMessage, out fileName);
                  }

                  _log.Information("{Branch} Export DBF ZIP completed- {Status}- {Error}", branch, status.ToStringEnum(), errorMessage);
                  #endregion

                  #region Move file to web server & indicate link/status
                  if (status == ProcessTracking.CurrentStatus.Completed && File.Exists(fileName))
                  {
                    var destFile = Path.Combine(rootWebServerPath, Path.GetFileName(fileName));
                    File.Move(fileName, destFile);
                    branchLinks.Add(new Tuple<string, string>(branch, string.Format("{0}- {1}{2}", branch, rootHTTPPath, Path.GetFileName(fileName))));
                  }
                  else
                  {
                    branchLinks.Add(new Tuple<string, string>(branch, string.Format("{0}- Export failed: '{1}'", branch, errorMessage)));
                  }
                  #endregion
                }
                catch (Exception err)
                {
                  branchLinks.Add(new Tuple<string, string>(branch, string.Format("{0}- Export error: '{1}'", branch, err.Message)));
                }

                _log.Information("{Branch} Export DBF ZIP data completed: {Elapsed}", branch, TimeSpanUtils.GetReadableTimeSpan(branchTimer.Elapsed));
              }
            });
        }

        Task.WaitAll(tasks, TimeSpan.FromHours(4));

        #region Get e-mail alert settings
        var recipients = new List<string>();
        string mailServer = null;

        using (var unitOfWork = new UnitOfWork())
        {
          var mailServerSetting = unitOfWork.Query<Config>().FirstOrDefault(s => s.DataEntity == "ASS DATASYNC EMAIL SERVER");
          if (mailServerSetting == null)
          {
            _log.Warning("No SMTP e-mail server has been configured for alerts- please add entry in the Config table (DataEntity = 'ASS DATASYNC EMAIL SERVER'");
            return;
          }
          mailServer = mailServerSetting.DataValue;
          recipients = unitOfWork.Query<Config>()
            .Where(s => s.DataEntity == "ASS DAILY DBF LINKS EMAIL")
            .Select(s => s.DataValue)
            .Distinct()
            .ToList();
          if (recipients == null || recipients.Count == 0)
          {
            _log.Warning("No e-mail recipients have been defined for ASS DBF links e-mail- " +
              "please add some recipient to the Config table (DataEntity = 'ASS DAILY DBF LINKS EMAIL')");
            return;
          }
        }
        #endregion

        #region Send e-mail
        if (recipients.Count > 0 && !string.IsNullOrEmpty(mailServer))
        {
          _log.Information("Sending e-mail");

          using (var client = new SmtpClient(host: mailServer))
          {
            #region Build message
            var message = new StringBuilder();
            message.AppendFormat("Daily DBF export process:\r\n");

            // Show exported links in alphabetical order
            var linkInfo = new List<Tuple<string, string>>();
            linkInfo.AddRange(branchLinks.Take(branchLinks.Count));
            linkInfo.OrderBy(s => s.Item1).ToList().ForEach(s => message.AppendFormat("{0}\r\n", s.Item2));

            message.AppendFormat("\r\n\r\n\r\nAtlas DataSync Alerting System");
            #endregion

            client.UseDefaultCredentials = false;
            client.Timeout = 20000; // 20 seconds            

            using (var mailMessage = new MailMessage(from: "info@atcorp.co.za", to: string.Join(",", recipients), subject: "Atlas DataSync- DBF Download links",
              body: message.ToString()))
            {
              mailMessage.IsBodyHtml = false;
              client.Send(mailMessage);
            }
          }

          _log.Information("Completed sending alert e-mail");
        }
        #endregion
      }
      catch (Exception err)
      {
        _log.Error(err, methodName);
      }

      _log.Information("{MethodName} completed- took {Elapsed}", methodName,
        TimeSpanUtils.GetReadableTimeSpan(timeProcess.Elapsed));
    }


    #region Private fields

    /// <summary>
    /// Logging
    /// </summary>
    private readonly ILogging _log;

    /// <summary>
    /// Max branch last sync age- used to not bother exporting old branch data
    /// </summary>
    private const int MAX_SYNC_AGE_MINUTES = 96 * 60; // Allow 4 days


    /// <summary>
    /// Concurrent task count
    /// </summary>
    private const int TASK_COUNT = 4;
    private readonly IConfigSettings _config;
    private readonly ICacheServer _cache;

    #endregion

  }
}

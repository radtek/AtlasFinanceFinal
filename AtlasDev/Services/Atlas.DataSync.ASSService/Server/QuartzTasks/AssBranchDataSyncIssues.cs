/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Send alerts for detected data sync problems
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
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Collections.Generic;

using Quartz;

using DevExpress.Xpo;

using Atlas.Domain.Model;

using Atlas.Common.Utils;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Cache.DataUtils;


namespace ASSServer.QuartzTasks
{
  [DisallowConcurrentExecution]
  public class AssBranchDataSyncIssues : IJob
  {
    public AssBranchDataSyncIssues(ILogging log, IConfigSettings config, ICacheServer cache)
    {
      _log = log;
      _config = config;
      _cache = cache;
    }


    /// <summary>
    /// Main Quartz method
    /// </summary>
    /// <param name="context"></param>
    public void Execute(IJobExecutionContext context)
    {
      var methodName = "Alerting.Execute";
      try
      {
        _triggeredCount++;

        _log.Information("{MethodName} starting- triggered: {TriggeredCount}", methodName, _triggeredCount);

        // Avoid firing the first time round, as branches may not yet have uploaded...
        if (_triggeredCount > 1)
        {
          var mailBody = new StringBuilder();
          var recipients = new List<string>();
          string mailServer = null;

          using (var unitOfWork = new UnitOfWork())
          {
            #region Get e-mail alert settings
            var mailServerSetting = unitOfWork.Query<Atlas.Domain.Model.Config>().FirstOrDefault(s => s.DataEntity == "ASS DATASYNC EMAIL SERVER");
            if (mailServerSetting == null)
            {
              _log.Warning("No SMTP e-mail server has been configured for ASS Data Sync alerts- please add entry in the Config table (DataEntity = 'ASS DATASYNC EMAIL SERVER'");
              return;
            }
            mailServer = mailServerSetting.DataValue;
            recipients = unitOfWork.Query<Atlas.Domain.Model.Config>()
              .Where(s => s.DataEntity == "ASS DATASYNC EMAIL ALERT RECIPIENT")
              .Select(s => s.DataValue)
              .Distinct()
              .ToList();
            if (recipients == null || recipients.Count == 0)
            {
              _log.Warning("No recipients have been defined for ASS Data Sync alerts- please add some recipient to the Config table (DataEntity = 'ASS DATASYNC EMAIL ALERT RECIPIENT')");
              return;
            }
            #endregion

            #region Check for branches which have not uploaded any data for 60 minutes
            var minDate = DateTime.Now.Subtract(TimeSpan.FromDays(5));
            var greaterThan60MinsAgo = DateTime.Now.Subtract(TimeSpan.FromMinutes(60));
            var offlineServers = unitOfWork.Query<ASS_BranchServer>().Where(s =>
              s.Branch != null && s.Machine != null &&  // Is active
              s.Branch.CloseDT == null &&          // Branch is open              
              s.LastSyncDT > minDate &&              // Has not synced for more than 30 days- there is a problem, no point alerting
              s.LastSyncDT < greaterThan60MinsAgo);  // No sync for past 30 mins

            foreach (var offlineServer in offlineServers)
            {
              _log.Error("Branch server named '{0}', for branch '{1}' (2) has not uploaded any ASS data since: {3:yyyy-MM-dd HH:mm:ss} ({4})",
                offlineServer.Machine.MachineName, offlineServer.Branch.LegacyBranchNum, offlineServer.Branch.Company.Name, offlineServer.LastSyncDT,
                TimeSpanUtils.GetReadableTimeSpan(DateTime.Now.Subtract(offlineServer.LastSyncDT)));

              mailBody.AppendFormat("  - Branch server named '{0}', for branch '{1} ({2})' has not uploaded any ASS\r\n    data since: {3:yyyy-MM-dd HH:mm:ss} ({4})\r\n\r\n",
                offlineServer.Machine.MachineName, offlineServer.Branch.LegacyBranchNum, offlineServer.Branch.Company.Name, offlineServer.LastSyncDT,
                TimeSpanUtils.GetReadableTimeSpan(DateTime.Now.Subtract(offlineServer.LastSyncDT)));
            }
            #endregion

            #region Check branches which are not on the latest ASS DB current version
            var updateScriptId = CacheUtils.GetCurrentDbVersion(_cache).DbUpdateScriptId;
            foreach (var branch in unitOfWork.Query<ASS_BranchServer>().Where(s => s.Branch != null && s.Machine != null))
            {
              if (branch.RunningDBVersion != null && branch.RunningDBVersion.DbUpdateScriptId != updateScriptId)
              {
                _log.Error("Branch server named '{0}', for branch {1} reports a current DB version: {2}, server current DB ver: {3}",
                  branch.Machine.MachineName, branch.Branch.LegacyBranchNum, branch.RunningDBVersion.DbVersion, updateScriptId);

                mailBody.AppendFormat("Branch server named '{0}', for branch {1} ({2}) reports\r\ncurrent DB version: {3}, server current DB ver: {4}\r\n\r\n",
                  branch.Machine.MachineName, branch.Branch.LegacyBranchNum, branch.Branch.Company.Name, branch.RunningDBVersion.DbVersion, updateScriptId);
              }
            }
            #endregion

            #region Severe errors since last alert sent
            var severeErrors = unitOfWork.Query<ASS_LogSyncServerEvent>()
              .Where(s => s.Server != null && s.Severity >= 5 && s.RaisedDT > _lastProcessedError)
              .Select(s => new
              {
                LegacyBranchNum = s.Server.Branch.LegacyBranchNum,
                Task = s.Task,
                Message = s.EventMesage,
                RaisedDT = s.RaisedDT,
                CompanyName = s.Server.Branch.Company.Name
              })
              .OrderBy(s => s.LegacyBranchNum).ToList();
            if (severeErrors.Any())
            {
              mailBody.AppendFormat("\r\n\r\nThe following new errors have been logged since {0:yyyy-MM-dd HH:mm:ss}", _lastProcessedError);

              string _lastServer = null;
              var groupedByServerAndError = severeErrors.GroupBy(s => new { s.LegacyBranchNum, s.Task, s.Message })
                .OrderBy(s => string.Format("{0}{1}{2}", s.Key.LegacyBranchNum, s.Key.Task, s.Key.Message))
                .ToList();
              foreach (var error in groupedByServerAndError)
              {
                var firstError = error.First();
                if (_lastServer != firstError.LegacyBranchNum)
                {
                  mailBody.AppendFormat("\r\n\r\nBranch {0}- '{1}':", firstError.LegacyBranchNum, firstError.CompanyName);
                  _lastServer = firstError.LegacyBranchNum;
                }

                mailBody.AppendFormat("\r\n {0} x [{1}]- '{2}': {3:HH:mm:ss}{4}",
                  error.Count(), firstError.Task, firstError.Message, error.Min(s => s.RaisedDT),
                  error.Count() > 1 ? string.Format("-{0:HH:mm:ss}", error.Max(s => s.RaisedDT)) : null);
              }

              _lastProcessedError = severeErrors.Max(s => s.RaisedDT).Value;
            }
            else
            {
              _lastProcessedError = DateTime.Now;
            }
            #endregion
          }

          #region Send e-mail
          if (mailBody.Length > 0 && recipients.Count > 0 && !string.IsNullOrEmpty(mailServer))
          {
            _log.Information("Sending alert e-mail");

            mailBody.Insert(0, "The ASS Data Sync server has detected the following issues, requiring your attention:\r\n\r\n");
            using (var client = new SmtpClient(host: mailServer))
            {
              client.UseDefaultCredentials = false;
              client.Timeout = 20000; // 20 seconds            

              mailBody.AppendFormat("\r\n\r\n\r\nAtlas Data Sync Alerting System");
              using (var mailMessage = new MailMessage(from: "info@atcorp.co.za", to: string.Join(",", recipients), subject: "Atlas Data Sync- ALERTS",
                body: mailBody.ToString()))
              {
                mailMessage.IsBodyHtml = false;
                client.Send(mailMessage);
              }
            }

            _log.Information("Completed sending alert e-mail");
          }
          #endregion
        }

        _log.Information("{MethodName} completed", methodName);
      }
      catch (Exception err)
      {
        _log.Error(err, methodName);
      }
    }


    #region Private vars

    private readonly ILogging _log;
    private readonly IConfigSettings _config;
    private readonly ICacheServer _cache;

    /// <summary>
    /// Amount of times has been triggered
    /// </summary>
    private static int _triggeredCount = 0;

    private static DateTime _lastProcessedError = DateTime.Now.Subtract(TimeSpan.FromMinutes(90));



    #endregion

  }
}

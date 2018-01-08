using System;
using System.Linq;
using System.Net.Mail;
using System.Text;

using Quartz;

using Atlas.Common.Extensions;
using Atlas.Enumerators;
using AvsEngineLight.DB;
using Atlas.Common.Interface;


namespace AvsEngineLight.QuartzTasks
{
  [DisallowConcurrentExecution]
  internal class CheckXdsResponsive : IJob
  {
    public CheckXdsResponsive(ILogging log)
    {
      _log = log;
    }


    public void Execute(IJobExecutionContext context)
    {
      _log.Information("CheckXdsResponsive.Execute starting");
      try
      {
        var emailMessage = new StringBuilder();
        int success;
        int waiting;
        int timeouts;
        var xds5to15Recent = AvsDbRepository.FetchRecent(AVS.Service.XDS, null, null, 15, 5);
        
        #region Last 15 minutes
        AvsDbRepository.CountAVS(xds5to15Recent, out success, out timeouts, out waiting);
        _log.Information("XDS 5-15Min: {success}, {timeouts}, {waiting}", success, timeouts, waiting);
        if (waiting + success + timeouts > 5) // we have enough data to check...
        {          
          if ((timeouts > 2 && ((decimal)(decimal)timeouts / (decimal)Math.Max(1, success)) > 0.2M) || (timeouts > 2 && timeouts > success)) // Errors >20% of success, there is a problem
          {
            _log.Warning("XDS 5-15Min errors: {success}, {timeouts}, {waiting}", success, timeouts, waiting);
            emailMessage.AppendFormat("- In the past 15 minutes, there are lots of XDS AVS time-outs: {0} time-outs, {1} successful\r\n", timeouts, success);
            emailMessage.AppendFormat(string.Join(Environment.NewLine, xds5to15Recent
              .Where(s => s.Result.ResultId == (int)AVS.Result.NoResult)
              .Select(s => string.Format("  * {0: HH:mm:ss}- {1} {2} {3} {4}", s.CreateDate, ((General.BankName)s.Bank.BankId).ToStringEnum(),
                ((AVS.Status)s.Status.StatusId).ToStringEnum(), ((AVS.Result)s.Result.ResultId).ToStringEnum(), s.LastName))));
          }

          if (waiting > 10) // More than 30 waiting, there is a problem
          {
            _log.Warning("XDS 5-15Min waiting: {success}, {timeouts}, {waiting}", success, timeouts, waiting);
            emailMessage.AppendFormat("- In the past 15 minutes, there are lots of XDS AVS still waiting: {0} waiting, {1} successful\r\n", waiting, success);
            emailMessage.AppendFormat(string.Join(Environment.NewLine, xds5to15Recent
              .Where(s => s.Status.StatusId == (int)AVS.Status.Pending || s.Status.StatusId == (int)AVS.Status.Queued)
              .Select(s => string.Format("  * {0: HH:mm:ss}- {1} {2} {3} {4}", s.CreateDate, ((General.BankName)s.Bank.BankId).ToStringEnum(),
                ((AVS.Status)s.Status.StatusId).ToStringEnum(), ((AVS.Result)s.Result.ResultId).ToStringEnum(), s.LastName))));
          }
        }
        #endregion

        #region Last hour
        var xdsLastHour = AvsDbRepository.FetchRecent(AVS.Service.XDS, null, null, 60, 5);
        AvsDbRepository.CountAVS(xdsLastHour, out success, out timeouts, out waiting);
        _log.Information("XDS 5-60Min: {success}, {timeouts}, {waiting}", success, timeouts, waiting);
        if (waiting + success + timeouts > 10) // we have enough data to check...
        {
          if ((timeouts > 3 && ((decimal)(decimal)timeouts / (decimal)Math.Max(1, success)) > 0.05M) || (timeouts > 2 && timeouts > success)) // Error count > 5% of success count, there is a problem
          {
            _log.Warning("XDS 5-60Min errors: {success}, {timeouts}, {waiting}", success, timeouts, waiting);
            emailMessage.AppendFormat("- In the past 60 minutes, there are lots of XDS AVS time-outs: {0} time-outs, {1} successful:\r\n", timeouts, success);
            emailMessage.AppendFormat(string.Join(Environment.NewLine, xds5to15Recent
              .Where(s => s.Result.ResultId == (int)AVS.Result.NoResult)
              .Select(s => string.Format("  * {0: HH:mm:ss}- {1} {2} {3} {4}", s.CreateDate, ((General.BankName)s.Bank.BankId).ToStringEnum(),
                ((AVS.Status)s.Status.StatusId).ToStringEnum(), ((AVS.Result)s.Result.ResultId).ToStringEnum(), s.LastName))));
          }

          if (waiting > 30) // More than 30 waiting, there is a problem
          {
            _log.Warning("XDS 5-60Min waiting: {success}, {timeouts}, {waiting}", success, timeouts, waiting);
            emailMessage.AppendFormat("- In the past 60 minutes, there are lots of XDS AVS still waiting: {0} waiting, {1} successful:\r\n", waiting, success);
            emailMessage.AppendFormat(string.Join(Environment.NewLine, xds5to15Recent
              .Where(s => s.Status.StatusId == (int)AVS.Status.Pending || s.Status.StatusId == (int)AVS.Status.Queued)
              .Select(s => string.Format("  * {0: HH:mm:ss}- {1} {2} {3} {4}", s.CreateDate, ((General.BankName)s.Bank.BankId).ToStringEnum(),
                ((AVS.Status)s.Status.StatusId).ToStringEnum(), ((AVS.Result)s.Result.ResultId).ToStringEnum(), s.LastName))));
          }
        }
        #endregion

        if (emailMessage.Length > 0)
        {
          _log.Warning(emailMessage.ToString());
          emailMessage.AppendFormat("\r\n\r\n\r\nAtlas AVS Alerting System");
          SendEmail(emailMessage);         
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "CheckXdsResponsive.Execute");
      }
      _log.Information("CheckXdsResponsive.Execute completed");
    }


    private void SendEmail(StringBuilder message)
    {
      try
      {
        using (var client = new SmtpClient(host: "mail.atcorp.co.za"))
        {
          client.UseDefaultCredentials = false;
          client.Timeout = 20000; // 20 seconds            

          using (var mailMessage = new MailMessage(from: "it@atcorp.co.za", to: "it@atcorp.co.za",
            subject: "Atlas AVS- ALERTS", body: message.ToString()))
          {
            mailMessage.IsBodyHtml = false;
            client.Send(mailMessage);
          }
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "SendEmail");
      }
    }


    /// <summary>
    /// Logging
    /// </summary>
    private readonly ILogging _log;

  }
}

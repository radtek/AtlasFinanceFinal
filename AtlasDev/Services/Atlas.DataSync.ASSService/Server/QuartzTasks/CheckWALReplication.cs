using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Mail;

using Quartz;
using DevExpress.Xpo;
using Npgsql;

using Atlas.Common.Interface;


namespace ASSServer.QuartzTasks
{
  [DisallowConcurrentExecution]
  public class CheckWALReplication : IJob
  {
    public CheckWALReplication(ILogging log, IConfigSettings config)
    {
      _log = log;
      _config = config;
    }


    /// <summary>
    /// Check if replication is falling behind
    /// </summary>
    /// <param name="context"></param>
    public void Execute(IJobExecutionContext context)
    {
      var methodName = "CheckWALReplication.Execute";
      try
      {
        _log.Information("{MethodName} starting", methodName);
        var clients = new List<Tuple<string, string, string>>();
        using (var conn = new NpgsqlConnection(_config.GetAssConnectionString()))
        {
          conn.Open();

          using (var cmd = conn.CreateCommand())
          {
            // TODO: This returns how many seconds the slave is behind (Connect to slaves):
            // SELECT CASE
            //    WHEN pg_last_xlog_receive_location() = pg_last_xlog_replay_location() THEN 0
            //      ELSE EXTRACT (EPOCH FROM now() - pg_last_xact_replay_timestamp())
            //     END AS log_delay;

            // Slave listing/byte lag (run on master):
            //  SELECT client_hostname, client_addr, pg_xlog_location_diff(pg_stat_replication.sent_location, pg_stat_replication.replay_location) AS byte_lag    
            //  FROM pg_stat_replication


            // TODO: Pop these into cache for seperate monitoring
            cmd.CommandText = "SELECT client_addr::text, sent_location::text, replay_location::text FROM pg_stat_replication";
            using (var rdr = cmd.ExecuteReader())
            {
              while (rdr.Read())
              {
                clients.Add(new Tuple<string, string, string>(rdr.GetString(0),
                  rdr.GetString(1).Replace("/", ""), rdr.GetString(2).Replace("/", "")));
              }
            }
          }
        }

        var clientList = string.Join(", ", clients.Select(s => s.Item1));

        switch (clients.Count)
        {
          case 0:
            SendAlertEMail("FATAL: PostgreSQL data replication (WAL streaming)- No clients", "SEVERE WARNING: No clients are accepting WAL streams- there is no DR!");
            break;

          case 1:
          case 2:
            SendAlertEMail(
              string.Format("ERROR: PostgreSQL data replication (WAL streaming)- Only {0} WAL client(s) active, expected 3", 
              clients.Count),
              string
              .Format("Only the following machines are receiving WAL streams:\r\n{0}- limited functionality!", clientList));
            break;

          // 3 is good!
        }
        
        foreach (var client in clients)
        {
          var sent = Int64.Parse(client.Item2, System.Globalization.NumberStyles.HexNumber);
          var flushed = Int64.Parse(client.Item3, System.Globalization.NumberStyles.HexNumber);

          if ((sent - flushed) > 500000000)
          {
            SendAlertEMail(string.Format("WARN: PostgreSQL data replication (WAL streaming)- client '{0}' appears to be falling dangerously far behind data on the master", client.Item1),
              string.Format("Client WAL stream details: {0}\r\n\r\nSent: {1}, Flushed: {2}", client.Item1, client.Item2, client.Item3));
          }
        }
      }
      catch (Exception err)
      {
        _log.Error(err, methodName);
      }

      _log.Information("{MethodName} completed", methodName);
    }


    #region Private methods

    /// <summary>
    /// Send an alert e-mail
    /// </summary>
    /// <param name="subject"></param>
    /// <param name="message"></param>
    private void SendAlertEMail(string subject, string message)
    {
      #region Get e-mail alert settings
      var recipients = new List<string>();
      string mailServer = null;

      using (var unitOfWork = new UnitOfWork())
      {
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
          _log.Warning("No recipients have been defined for ASS Data Sync alerts- please add some recipients to the Config table (DataEntity = 'ASS DATASYNC EMAIL ALERT RECIPIENT')");
          return;
        }
      }
      #endregion

      #region Send
      using (var client = new SmtpClient(host: mailServer))
      {
        client.UseDefaultCredentials = false;
        client.Timeout = 20000; // 20 seconds            

        using (var mailMessage = new MailMessage(from: "info@atcorp.co.za", to: string.Join(",", recipients), subject: subject,
          body: message))
        {
          mailMessage.IsBodyHtml = false;
          client.Send(mailMessage);
        }
      }
      #endregion

      _log.Information("Completed sending alert e-mail");
    }

    #endregion


    #region Private fields

    private readonly ILogging _log;
    private readonly IConfigSettings _config;

    #endregion

  }
}

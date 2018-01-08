using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Diagnostics;

using Quartz;
using Serilog;
using Npgsql;

using ASSSyncClient.Utils;
using Atlas.DataSync.WCF.Client.ClientProxies;
using ASSSyncClient.Utils.Settings;


namespace ASSSyncClient.QuartzTasks
{
  /// <summary>
  /// Task to delete client records and transactions where last transaction >3 years & loan balance = 0
  /// </summary>
  [global::Quartz.DisallowConcurrentExecution]
  internal class POPIDelete : IJob
  {
    public void Execute(IJobExecutionContext context)
    {
      var methodName = "POPIDelete.Execute";
      try
      {
        _log.Information("{MethodName} Starting", methodName);
        // Ensure our local time is currently correct, to ensure we don't delete valid rows due to local date/time error
        var serverDateTime = DateTime.MinValue;
        using (var client = new DataSyncDataClient())
        {
          serverDateTime = client.GetServerDateTime();
        }

        _log.Information("{MethodName} Checking local time: {Local}, remote: {Remote}", methodName, DateTime.Now, serverDateTime);
        if (serverDateTime > DateTime.MinValue && Math.Abs(DateTime.Now.Subtract(serverDateTime).TotalMinutes) <= 5)
        {
          var clientsToDelete = new List<string>();

          using (var conn = new NpgsqlConnection(AppSettings.NPGSQLConnStr))
          {
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "";
            cmd.CommandType = CommandType.Text;

            #region Get clients where last transaction was >3 years ago
            cmd.CommandText = "SELECT t.client, t.trdate " +
              "FROM trans t  " +
              "JOIN client c on t.client = c.client " +
              "GROUP BY t.client " +
              "HAVING MAX(t.trdate) < @MinDate";
            cmd.Parameters.AddWithValue("MinDate", DateTime.Today.AddMonths(-38));

            cmd.CommandTimeout = (int)TimeSpan.FromMinutes(30).TotalSeconds;
            using (var rdr = cmd.ExecuteReader())
            {
              while (rdr.Read())
              {
                clientsToDelete.Add(rdr.GetString(0));
              }
            }
            _log.Information("{MethodName} Found {Clients} to delete", methodName, clientsToDelete.Count);
            #endregion
                       
            if (clientsToDelete.Count > 0)
            { 
              #region Ensure loans.outamnt = 0 for each client
              // There is no index on the 'client' field in the 'loans' table, so iterating through loans 
              // for each client is **very** slow, so we get the data once into memory and then process...
              cmd.CommandText = string.Format("SELECT client " +
                "FROM loans " +
                "WHERE (outamnt > 0) AND (client IN ({0}));",
                string.Join(",", clientsToDelete.Select(s => string.Format("'{0}'", s))));
              cmd.Parameters.Clear();
              var clientsWithPosBalances = new HashSet<string>();
              using (var rdr = cmd.ExecuteReader())
              {
                while (rdr.Read())
                {
                  if (!clientsWithPosBalances.Contains(rdr.GetString(0)))
                  {
                    clientsWithPosBalances.Add(rdr.GetString(0));
                  }
                }
              }

              foreach (var client in clientsWithPosBalances)
              {
                clientsToDelete.Remove(client);
              }

              _log.Information("{MethodName} Removed {Clients} from delete batch due to a positive loan balance", 
                methodName, clientsWithPosBalances.Count);
              #endregion

              #region Perform the deletion
              var deleteTimer = Stopwatch.StartNew();
              if (clientsToDelete.Count > 0)
              {
                var clientNumsCsv = string.Join(",", clientsToDelete.Select(s => string.Format("'{0}'", s)));
                cmd.Parameters.Clear();
                cmd.CommandText = string.Format(
                  "BEGIN;" +
                  "DELETE FROM client c where c.client IN ({0});" +
                  //"DELETE FROM afford a where a.client IN ({0});" +
                  "DELETE FROM cbtrans c where c.client IN ({0});" +
                  //"DELETE FROM avstrnid a where a.identno IN ({1});" +
                  "DELETE FROM loans l where l.client IN ({0});" +
                  "DELETE FROM approval a WHERE a.client IN ({0});" +
                  "DELETE FROM authorit a WHERE a.client IN ({0});" +
                  "DELETE FROM claud c WHERE c.client IN ({0});" +
                  "DELETE FROM comments c WHERE c.client IN ({0});" +
                  "DELETE FROM loanaud l WHERE l.client IN ({0});" +
                  "DELETE FROM paypland p WHERE p.client IN ({0});" +
                  "DELETE FROM payplanh p WHERE p.client IN ({0});" +
                  "DELETE FROM stars s WHERE s.client IN ({0});" +
                  "DELETE FROM trans t WHERE t.client IN ({0});" +
                  "COMMIT;", clientNumsCsv /*, idNumbersCsv */);

                cmd.CommandTimeout = (int)TimeSpan.FromHours(2).TotalSeconds;
                cmd.ExecuteNonQuery();

                _log.Information("{MethodName} Successfully deleted {ClientCount} clients in {Elapsed}s", 
                  methodName, clientsToDelete.Count, deleteTimer.Elapsed.Seconds);
              }
              else
              {
                _log.Information("{MethodName} No clients found to delete", methodName);
              }
              #endregion
            }
          }
        }
        else
        {
          var err = new Exception(string.Format("Unable to delete local records as time is >5 mins out. " +
            "Server: {0:yyyy-MM-dd HH:mm:ss}, Local: {1:yyyy-MM-dd HH:mm:ss}", serverDateTime, DateTime.Now));
          LogEvents.Log(DateTime.Now, "POPIDelete.Execute", err.Message, 5);
          _log.Warning(err, "{MethodName}", methodName);
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "{MethodName}", methodName);
      }
    }


    #region Private fields

    private static readonly ILogger _log = Log.Logger.ForContext<POPIDelete>();

    #endregion

  }
}

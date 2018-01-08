using System;
using System.Linq;
using System.Configuration;
using System.ServiceProcess;

using Serilog;
using Npgsql;


namespace Atlas.Server.Training.QuartzTasks
{
  internal class ServiceUtils
  {

    /// <summary>
    /// Closes all connections to a specific database, so it can be dropped/renamed
    /// </summary>
    /// <param name="log">ILogger to use</param>
    /// <param name="database">Name of database to close all connections</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    internal static void CloseAllConnectionsToDb(ILogger log, string database)
    {
      try
      {
        // PostgreSQL 'system' connection- NOTE: do not specify a data for dropping/creating a DB!!!
        var connStr = new NpgsqlConnectionStringBuilder(ConfigHelper.PSQLDestConnectionString()) { Database = null };

        using (var conn = new NpgsqlConnection(connStr))
        {
          conn.Open();

          using (var cmd = conn.CreateCommand())
          {
            cmd.CommandText = string.Format(
              "SELECT pg_terminate_backend(pg_stat_activity.pid) " +
              "FROM pg_stat_activity " +
              "WHERE pg_stat_activity.datname = '{0}' " +
              "AND pid <> pg_backend_pid();", database);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandTimeout = (int)TimeSpan.FromMinutes(1).TotalSeconds;
            cmd.ExecuteNonQuery();
          }
        }
      }
      catch (Exception err)
      {
        log.Error(err, "CloseAllConnections");
      }
    }


    /// <summary>
    /// Starts/stops a service
    /// </summary>
    /// <param name="startServices">Set to true to start all services, false to stop all services</param>
    internal static void StartStopServices(ILogger log, bool startServices, TimeSpan? timeout = null)
    {
      if (timeout == null)
      {
        timeout = TimeSpan.FromSeconds(90);
      }

      var index = 1;
      var serviceIndex = string.Format("service{0}", index);
      while (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[serviceIndex]))
      {
        var serviceName = ConfigurationManager.AppSettings[serviceIndex];
        var service = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName.ToLower().Contains(serviceName.ToLower()));
        if (service != null)
        {
          if (!startServices)
          {
            if (service.Status == ServiceControllerStatus.Running)
            {
              #region Stop service
              log.Information("Stopping {ServiceName}", serviceName);
              service.Stop();
              service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
              if (service.Status == ServiceControllerStatus.Stopped)
              {
                log.Information("Successfully stopped {ServiceName}", serviceName);
              }
              else
              {
                log.Error("Failed to stop {ServiceName}", serviceName);
              }
              #endregion
            }
          }
          else
          {
            #region Start service
            var attemptCount = 0;
            var successful = false;
            while (!successful && attemptCount++ < 5)
            {
              try
              {
                log.Information("Attempting to start {ServiceName}", serviceName);
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout.Value);
                if (service.Status == ServiceControllerStatus.Running)
                {
                  log.Information("Successfully started {ServiceName}", serviceName);
                  successful = true;
                }
                else
                {
                  log.Warning("Failed to start {ServiceName}- Attempt {Attempt}", serviceName, attemptCount);
                }
              }
              catch (Exception err)
              {
                log.Error(err, "StartStopServices()- {ServiceName}- Attempt {Attempt}", serviceName, attemptCount);
              }
            }
            #endregion
          }
        }
        else
        {
          log.Warning("Failed to locate {ServiceName}", serviceName);
        }

        serviceIndex = string.Format("service{0}", ++index);
      }
    }

  }
}

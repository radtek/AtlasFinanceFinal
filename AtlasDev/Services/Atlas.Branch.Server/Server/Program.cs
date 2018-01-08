/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *   Main entry for server
 *   
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2013-06-13  Created
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using Serilog;
using Topshelf;


namespace ASSSyncClient
{
  class Program
  {
    /// <summary>
    /// Main entry 
    /// </summary>
    static void Main()
    {
      try
      {        
        #region Ensure config file exists
        var executing = typeof(Program).Assembly.Location;
        var checkFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
        if (!File.Exists(checkFile))
        {
          var error = new Exception(string.Format("Config file '{0}' missing!", checkFile));
          Console.WriteLine(error.Message);
          return;
        }
        #endregion

        #region Logging
        var logPath = Path.Combine(Path.GetDirectoryName(executing), "Logs", @"ASSSyncClient-{Date}.log");
        Log.Logger = new LoggerConfiguration()
          .Enrich.WithProperty("SourceContext", "")
          .WriteTo.ColoredConsole(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
            outputTemplate: "{Timestamp:HH:mm:ss.fff} {SourceContext:l}- {Message}{NewLine}{Exception}")
            .WriteTo.RollingFile(pathFormat: logPath, retainedFileCountLimit: 10)
          .CreateLogger();
        var log = Log.Logger.ForContext<Program>();
        #endregion        
        
        #region Ensure the PostgreSQL database service installed
        var postgreSQLService = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName.ToLower().Contains("postgresql"));
        if (postgreSQLService == null)
        {
          var error = new Exception("PostgreSQL Server service must be installed- service could not be located");
          log.Error("Failed to locate the PostgreSQL server service", null);
          throw error;
        }
        #endregion

        #region Topshelf service hosting
        HostFactory.Run(x =>
        {
          x.UseSerilog();

          x.Service<MainService>(s =>
            {                    
              s.ConstructUsing(name => new MainService());              
              s.WhenStarted((tc, control) => tc.Start("ass_datasync_client_v1", control));
              s.WhenStopped(tc => tc.Stop()); 
            });

          x.EnableServiceRecovery(r =>
          {
            r.RestartService(5);  // 1st failure
            r.RestartService(15); // 2nd failure
            r.RestartComputer(60, "Atlas DataSync failed 3 times- system restarting!"); // 3rd failure

            r.SetResetPeriod(1);
          });
         
          //x.UseSerilog();
          x.RunAsLocalSystem();
          x.StartAutomatically();                   
          
          x.SetDescription("Atlas Legacy LMS Data Sync Client Service. This service ensures the local LMS database " +
            "remains in sync with the central LMS database. If this service is stopped, data between this branch and " +
            "the centralized database system will not synchronize- master table data will not replicate to the branch and " +
            "changes made at branch level will not replicate to the centralised database. This server also facilitates other branch " +
            "server functionality and tasks, like database clean-ups, a document scanning file server, as well as document decoding and file uploading.");

          x.SetDisplayName("Atlas ASS Data WCF Client Service V1.0");
          x.SetServiceName("ass_datasync_client_v1");   
        });
        #endregion
      }
      catch (Exception err)
      {
        Console.WriteLine("Start-up error: '{0}'", err.Message);
        throw;
      }
    }

  }
}
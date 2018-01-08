/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Main application entry point
 *     
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     
 *     
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.IO;

using Topshelf;
using Serilog;


namespace AClientSvc
{
  class Program
  {
    static void Main()
    {
      var executing = System.Reflection.Assembly.GetEntryAssembly().Location;
     
      #region Register logging
      Log.Logger = new LoggerConfiguration()
          .Enrich.WithProperty("SourceContext", "")
          .WriteTo.RollingFile(Path.Combine(Path.GetDirectoryName(executing), "Logs", "AClientSvc-{Date}.txt"))
          .WriteTo.ColoredConsole()
          .CreateLogger();

      var log = Log.ForContext<Program>();
      log.Information("Logging started");
      #endregion

      #region Topshelf service hosting
      HostFactory.Run(x =>
      {
        x.Service<MainService>(s =>
        {
          s.ConstructUsing(name => new MainService());
          // Events
          s.WhenStarted(tc => tc.Start(ServiceName));
          s.WhenStopped(tc => tc.Stop());
        });

        x.RunAsLocalSystem();
        x.StartAutomatically();

        x.UseSerilog();

        x.SetDisplayName("Atlas Core Client Service");
        x.SetDescription("Atlas Core Client Service. This service communicates with Atlas services, to provide " +
          "hardware/software instrumentation and to facilitate company-wide communications.");
               
        x.SetServiceName(ServiceName);

        x.EnableServiceRecovery(r =>
          {
            r.RestartService(5);
            r.RestartService(15);
            r.RestartService(30);
            r.SetResetPeriod(1);

          });
      });
      #endregion
    }


    /// <summary>
    /// Name of service to be installed
    /// </summary>
    private static string ServiceName
    {
      get { return "AtlasCoreClient.V1.0"; }
    }

  }
}

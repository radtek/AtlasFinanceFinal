using System;
using System.IO;
using System.Reflection;

using Topshelf;
using Serilog;
using Serilog.Enrichers;
using Serilog.Events;

using Atlas.ACCPAC;


namespace WinServices
{
  class Program
  {
    /// <summay>
    /// The main entry point for the application.
    /// </summary>
    static void Main()
    {
      var executing = Assembly.GetEntryAssembly().Location;

      #region Ensure config file exists
      var checkFile = string.Format("{0}.config", executing);
      if (!File.Exists(checkFile))
      {
        Console.WriteLine("Config file '{0}' missing!", checkFile);
        return;
      }
      #endregion

      #region Configure Logging
      var logPath = Path.Combine(Path.GetDirectoryName(executing), "Logs", @"ACCPACImport-{Date}.log");
      Log.Logger = new LoggerConfiguration()
        .Enrich.WithProperty("ApplicationId", "ACCPACImport")
        .Enrich.With(new ThreadIdEnricher())
        .Enrich.With(new ProcessIdEnricher())        
        .Enrich.WithProperty("SourceContext", "")
        .WriteTo.ColoredConsole(restrictedToMinimumLevel: LogEventLevel.Information,
          outputTemplate: "{Timestamp:HH:mm:ss.fff} ({ThreadId}) [{Level}] {SourceContext:l}  {Message}{NewLine}{Exception}")
        .WriteTo.RollingFile(logPath, LogEventLevel.Information)
        .CreateLogger();
      var log = Log.Logger.ForContext<Program>();
      #endregion

      log.Information("Starting");
      try
      {
        #region TopShelf service hosting
        HostFactory.Run(x =>
        {
          x.Service<ACCPACService>(s =>
          {
            s.ConstructUsing(builder => new ACCPACService());
            // Events
            s.WhenStarted(tc => tc.Start());
            s.WhenStopped(tc => tc.Stop());
          });
          x.DependsOnMsSql();

          x.RunAsLocalSystem();
          x.StartAutomaticallyDelayed(); // Give time for system stabilisation

          x.DependsOnMsSql();                    
          x.UseSerilog();
          x.SetDescription("Atlas ACCPAC QD to GL converter and GL batch ACCPAC importer (QD's/VAT adjustments). If this service is stopped, no ACCPAC data will be updated/imported.");
          x.SetDisplayName("Atlas ACCPAC Service V1.1");
          x.SetServiceName("Atlas_ACCPAC_Importer_V1_1");
        });
        #endregion
      }
      catch (Exception err)
      {
        if (log != null)
        {
          log.Error(err, "Start-up error: '{0}'");
        }
      }
    }
  }
}

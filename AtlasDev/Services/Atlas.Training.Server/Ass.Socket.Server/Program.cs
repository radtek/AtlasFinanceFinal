using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

using Serilog;
using Serilog.Events;
using Topshelf;
using Serilog.Enrichers;


namespace Atlas.Ass.Socket.Server
{
  class Program
  {
    static void Main(string[] args)
    {
      var executing = System.Reflection.Assembly.GetEntryAssembly().Location;

      #region Ensure config file exists
      var checkFile = string.Format("{0}.config", executing);
      if (!File.Exists(checkFile))
      {
        Console.WriteLine("Config file '{0}' missing!", checkFile);
        return;
      }
      #endregion

      #region Serilog
      var logPath = Path.Combine(Path.GetDirectoryName(executing), "Logs", @"ASS_Socket_Server-{Date}.log");
      Log.Logger = new LoggerConfiguration()
          .Enrich.WithProperty("ApplicationId", "ASS Socket Server")
          .Enrich.With(new ThreadIdEnricher())
          .Enrich.WithProperty("SourceContext", "")
          //.Enrich.With(new ProcessIdEnricher())
          .WriteTo.Seq(ConfigurationManager.AppSettings["SeqEndpoint"] ?? "http://172.31.75.41:5341", LogEventLevel.Information)
          .WriteTo.ColoredConsole(restrictedToMinimumLevel: LogEventLevel.Information, outputTemplate: "{Timestamp:HH:mm:ss.fff} ({ThreadId}) [{Level}] {SourceContext:l}  {Message}{NewLine}{Exception}")
          .WriteTo.RollingFile(logPath, LogEventLevel.Information)
          .CreateLogger();

      var log = Log.Logger.ForContext<Program>();
      log.Information("Starting");
      #endregion

      #region Topshelf service hosting
      try
      {
        HostFactory.Run(x =>
        {
          x.Service<ServiceEvents>(s =>
          {
            s.ConstructUsing(() => new ServiceEvents());
            // Events
            s.WhenStarted((c, hostControl) => c.Start(hostControl));
            s.WhenStopped(c => c.Stop());
          });

          x.UseSerilog();

          x.RunAsLocalService();
          x.StartAutomatically();

          x.SetDisplayName("ASS Socket Server");
          x.SetDescription("ASS Socket Server- provides routing of Ass socket messages to desktop.");

          x.SetDisplayName("ASS Socket Server");
          x.SetServiceName("ASSSocketServer");
        });

      }
      catch (Exception err)
      {
        Console.WriteLine("Start-up error: '{0}'", err.Message);
        log.Error(err, "Main()");
      }
      #endregion

    }
  }
}

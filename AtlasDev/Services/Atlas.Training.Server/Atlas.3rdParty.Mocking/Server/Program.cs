using System;
using System.IO;
using System.Reflection;
using System.Configuration;

using Serilog;
using Serilog.Enrichers;
using Serilog.Events;
using Topshelf;


namespace Atlas.Server.Training
{
  class Program
  {
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

      #region Serilog
      var logPath = Path.Combine(Path.GetDirectoryName(executing), "Logs", @"TrainingServer-{Date}.log");
      Log.Logger = new LoggerConfiguration()
          .Enrich.WithProperty("ApplicationId", "TrainingServer")
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
            x.Service<MainService>(s =>
            {
              s.ConstructUsing(() => new MainService());
              // Events
              s.WhenStarted((c, hostControl) => c.Start(hostControl));
              s.WhenStopped(c => c.Stop());
            });

            x.UseSerilog();

            x.RunAsLocalService();
            x.StartAutomatically();

            x.SetDisplayName("Atlas Training Service");
            x.SetDescription("Atlas Training service- provides mocking of 3rd party functionality, daily ass/atlas core database copy and Atlas service restarts");

            x.SetDisplayName("Atlas Training Service");
            x.SetServiceName("AtlasTrainingService");
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
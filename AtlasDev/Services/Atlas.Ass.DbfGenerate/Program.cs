using System;
using System.Configuration;
using System.IO;

using Serilog;
using Serilog.Enrichers;
using Topshelf;


namespace Atlas.Services.DbfGenerate
{
  class Program
  {
    static void Main()
    {
      #region Ensure config file exists
      var executing = System.Reflection.Assembly.GetEntryAssembly().Location;
      var checkFile = string.Format("{0}.config", executing);
      if (!File.Exists(checkFile))
      {
        var error = new Exception(string.Format("ERROR: Config file '{0}' missing!", checkFile));
        Console.WriteLine(error.Message);
        throw error;
      }
      #endregion

      #region Serilog
      var logPath = Path.Combine(Path.GetDirectoryName(executing), "Logs", @"DbfQdLog-{Date}.log");
      Log.Logger = new LoggerConfiguration()
        .Enrich.With(new ThreadIdEnricher())
        .Enrich.With(new ProcessIdEnricher())
        //.Enrich.With(new StackTraceEnricher())
        .Enrich.WithProperty("ApplicationId", "DbfProcess")       
        .WriteTo.Seq(ConfigurationManager.AppSettings["SeqEndpoint"] ?? "http://172.31.75.41:5341", Serilog.Events.LogEventLevel.Information)
        .WriteTo.ColoredConsole(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
          outputTemplate: "{Timestamp:HH:mm:ss.fff} ({ThreadId}) {Message}{NewLine}{Exception}")
        .WriteTo.RollingFile(logPath)
        .CreateLogger();

      var log = Log.Logger.ForContext<Program>();
      log.Information("Application loading...");
      #endregion

      #region Topshelf service hosting
      HostFactory.Run(x =>
      {
        x.Service<ServiceEvents>(s =>
        {
          s.ConstructUsing(name => new ServiceEvents());
          // Events
          s.WhenStarted(tc => tc.Start());
          s.WhenStopped(tc => tc.Stop());
        });

        x.UseSerilog();
        x.RunAsLocalService();
        x.StartAutomatically();

        x.SetServiceName("atlas.dbf.service.v1");
        x.SetDisplayName("Atlas branch DBF generator & FixChq scheduler");
        x.SetDescription("Atlas branch DBF generator & FixChq scheduler service. This service generates the daily branch DBFs and then runs the ASS reindex/QD/month-end generation process, via fixchq.exe.");        
      });
      #endregion
            
    }
  }
}

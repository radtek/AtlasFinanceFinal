using Atlas.Common.Interface;
using Serilog;

using Serilog.Enrichers;
using System;

using System.IO;


namespace Atlas.Servers.Common.Logging
{
  /// <summary>
  /// Implement ILogging with Serilog
  /// </summary>
  public class SerilogLogging : ILogging
  {
    public SerilogLogging(string applicationId, bool useSeq = false, Type main = null)
    {
      var executing = main != null ? System.Reflection.Assembly.GetAssembly(main).Location : System.Reflection.Assembly.GetAssembly(typeof(SerilogLogging)).Location;

      var logPath = Path.Combine(Path.GetDirectoryName(executing), "Logs");
      if (!Directory.Exists(logPath))
        Directory.CreateDirectory(logPath);

      var name = Path.GetFileNameWithoutExtension(executing);
      var allLogPath = Path.Combine(logPath, string.Format("{0}-{{Date}}.log", name));
      var errLogPath = Path.Combine(logPath, string.Format("{0}-{{Date}}-Errs.log", name));

      var logger = new LoggerConfiguration()
        .Enrich.WithProperty("ApplicationId", applicationId) // only required for Seq/Exceptionless...
        .Enrich.With(new ThreadIdEnricher())
        .WriteTo.ColoredConsole(outputTemplate: "{Timestamp:HH:mm:ss.fff} ({ThreadId}) {Message}{NewLine}{Exception}", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
        // file per day with full date- no need to log date
        .WriteTo.RollingFile(allLogPath, outputTemplate: "{Timestamp:HH:mm:ss.fff} ({ThreadId}) [{Level}] {Message}{NewLine}{Exception}")
        // file per day with full date- no need to log date
        .WriteTo.RollingFile(errLogPath, outputTemplate: "{Timestamp:HH:mm:ss.fff} ({ThreadId}) [{Level}] {Message}{NewLine}{Exception}", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error);

      if (useSeq)
      {
        //logger = logger.WriteTo.Seq("http://172.31.75.41:5341");
      }

      Log.Logger = logger.CreateLogger();
      Log.Information("Starting- logging: {@Path}", new { Path = logPath });
    }


    public void Error(string messageTemplate, params object[] propertyValues)
    {
      Log.Error(messageTemplate, propertyValues);
    }

    public void Error(Exception exception, string messageTemplate, params object[] propertyValues)
    {
      Log.Error(exception, messageTemplate, propertyValues);
    }

    public void Fatal(string messageTemplate, params object[] propertyValues)
    {
      Log.Fatal(messageTemplate, propertyValues);
    }

    public void Fatal(Exception exception, string messageTemplate, params object[] propertyValues)
    {
      Log.Fatal(exception, messageTemplate, propertyValues);
    }

    public void Information(string messageTemplate, params object[] propertyValues)
    {
      Log.Information(messageTemplate, propertyValues);
    }

    public void Information(Exception exception, string messageTemplate, params object[] propertyValues)
    {
      Log.Information(exception, messageTemplate, propertyValues);
    }

    public void Warning(string messageTemplate, params object[] propertyValues)
    {
      Log.Warning(messageTemplate, propertyValues);
    }

    public void Warning(Exception exception, string messageTemplate, params object[] propertyValues)
    {
      Log.Warning(exception, messageTemplate, propertyValues);
    }

  }
}
using System;
using System.Text;
using System.IO;
using System.Configuration;

using NDesk.Options;
using Serilog;
using Serilog.Enrichers;
using Serilog.Events;
using System.Collections.Generic;

namespace ZipSQLDumps
{
  public class Program
  {

    static int Main(string[] args)
    {
      var executing = System.Reflection.Assembly.GetExecutingAssembly().Location;

      #region Ensure config file exists
      var checkFile = string.Format("{0}.config", executing);
      if (!File.Exists(checkFile))
      {
        Console.WriteLine("Config file '{0}' missing!", checkFile);
        return 99;
      }
      #endregion

      #region Serilog
      var logPath = Path.Combine(Path.GetDirectoryName(executing), "Logs", @"TrainingServer-{Date}.log");
      Log.Logger = new LoggerConfiguration()
          .Enrich.WithProperty("ApplicationId", "ZipMSSQLDumps")
          .Enrich.With(new ThreadIdEnricher())
          .Enrich.WithProperty("SourceContext", "")
          .Enrich.With(new ProcessIdEnricher())
          .WriteTo.Seq(ConfigurationManager.AppSettings["SeqEndpoint"] ?? "http://172.31.75.41:5341", LogEventLevel.Information)
          .WriteTo.ColoredConsole(restrictedToMinimumLevel: LogEventLevel.Information, outputTemplate: "{Timestamp:HH:mm:ss.fff} ({ThreadId}) [{Level}] {SourceContext:l}  {Message}{NewLine}{Exception}")
          .WriteTo.RollingFile(logPath, LogEventLevel.Information)
          .CreateLogger();

      var log = Log.Logger.ForContext<Program>();
      log.Information("Starting");
      #endregion

      string zipError = null;
      string deleteError = null;
      string rotateError = null;
      string error = null;
      try
      {
        #region Basic command line checking
        if (args.Length < 2)
        {
          ShowUsage(string.Empty);
          return -1;
        }
        #endregion

        #region Default values
        // Source dir of backup files- will need to recurse
        var srcDir = string.Empty;

        // Destination directory of 7z files
        var destDir = string.Empty;

        // How many rolling weeks to keep (weekly)
        var rollingWeeks = 4;

        // How many rolling days to keep (daily backups)
        var rollingDays = 7;
        #endregion

        #region Parse parameters
        var options = new OptionSet()
                {                     
                    // Required- part of PK to find alert details
                    { "source=|Source=", "The source directory", v => srcDir = v.Trim() },

                    // Required- part of PK to find alert details
                    { "dest=|Dest=", "The destination directory", v => destDir = v.Trim() },

                    { "weeks=|Weeks=", "", v => int.TryParse(v, out rollingWeeks) },

                    { "days=|Days=", "", v => int.TryParse(v, out rollingDays) },
                };

        var extra = options.Parse(args);
        if (extra.Count > 0)
        {
          ShowUsage(string.Format("Unknown command line argument{0}: '{1}'", extra.Count == 1 ? "" : "s", string.Join(" ", extra)));
          return -1;
        }

        #endregion

        #region Check key parameters provided
        if (!Directory.Exists(srcDir))
        {
          ShowUsage(string.Format("<Source> directory '{0}' does not exist!", srcDir));

        }

        if (string.IsNullOrEmpty(srcDir))
        {
          ShowUsage("SrcDir parameter not supplied!");
          return -1;
        }

        if (string.IsNullOrEmpty(destDir))
        {
          ShowUsage("Destdir parameter not supplied");
          return -1;
        }
        #endregion

        #region Process
        zipError = ZipSQLDumpFiles.Execute(srcDir, destDir);
        
        // Delete all .bak files
        deleteError = DeleteFiles.Execute(srcDir);
        
        // Rotate .7z files
        rotateError = RotateBackupFiles.Execute(destDir, rollingDays, rollingWeeks);
        #endregion
      }
      catch (Exception err)
      {
        error = string.Format("Unhandled Main() error: '{0}'", err.Message);
      }

      #region Return any errors
      var errors = new List<string>();
      if (!string.IsNullOrEmpty(zipError))
        errors.Add(zipError);

      if (!string.IsNullOrEmpty(deleteError))
        errors.Add(deleteError);

      if (!string.IsNullOrEmpty(rotateError))
        errors.Add(rotateError);

      if (!string.IsNullOrEmpty(error))
        errors.Add(error);

      if (errors.Count > 0)
      {
        Console.WriteLine(errors.ToString());
        foreach (var err in errors)
        {
          log.Error(new Exception(err), "main()");
        }

        return 99;
      }
      #endregion

      log.Information("Complete");

      return 0;
    }


    static private void ShowUsage(string error)
    {
      if (!string.IsNullOrEmpty(error))
      {
        Console.WriteLine(string.Format("ERROR: '{0}'", error));
      }

      Console.WriteLine("USAGE: ZipSQLDumps Source=<Source dir> Dest=<Dest Dir> Days=<x days to keep> Weeks=<x weeks to keep>");
    }

  }
}

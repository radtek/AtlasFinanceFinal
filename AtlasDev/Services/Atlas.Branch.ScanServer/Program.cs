using System;
using System.IO;

using Topshelf;
using Serilog;
using GdPicture12;


namespace Atlas.Branch.ScanServer
{
  class Program
  {
    static void Main()
    {
      var executing = System.Reflection.Assembly.GetEntryAssembly().Location;

      #region Logging
      var logPath = Path.Combine(Path.GetDirectoryName(executing), "Logs", @"BranchScanServer-{Date}.log");
      Log.Logger = new LoggerConfiguration()
        //.Enrich.WithProperty("SourceContext", "")
        .WriteTo.ColoredConsole(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
          outputTemplate: "{Timestamp:HH:mm:ss.fff} {SourceContext:l}- {Message}{NewLine}{Exception}")
          .WriteTo.RollingFile(pathFormat: logPath, retainedFileCountLimit: 10)
        .CreateLogger();
      var log = Log.Logger.ForContext<Program>();
      #endregion

      log.Information("Loading...");

      #region Register GDPicture licenses
      var license = new LicenseManager();
      if (!license.RegisterKEY("13293665993739579111911142381496679811")) //132965496977096821115122879353532"))
      {
        throw new Exception("Failed to register key for GdPicture.NET Document Imaging SDK V12");
      }

      if (!license.RegisterKEY("72631698992742977151914149793665937433")) //718318999959219831319163658204273"))
      {
        throw new Exception("Failed to register key for GdPicture.NET Managed PDF Plugin V12");
      }
      #endregion
                  
      #region TopShelf service hosting
      HostFactory.Run(hc =>
      {
        // Config DI
        hc.UseSerilog();

        hc.RunAsLocalSystem();
        hc.StartAutomaticallyDelayed(); // Give time for system stabilization
        hc.SetServiceName("Atlas_Branch_Scan_V1_0");
        hc.SetDisplayName("Atlas Branch Scan Service V1.0");
        hc.SetDescription("Atlas Branch scan Server. If this service is stopped, branch scans will not be processed.");

        hc.Service<MainService>(sc =>
        {
          sc.ConstructUsing<MainService>(s => new MainService(log));
          sc.WhenStarted(s => s.Start());
          sc.WhenStopped(s => s.Stop());
        });
      });
      #endregion            
    }

  }
  
}

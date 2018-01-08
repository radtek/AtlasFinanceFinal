
namespace Atlas.ConfigServer
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.IO;
  using Topshelf;
  using Atlas.PDF.Server.WCF;

  class Program
  {
    /// <summary>
    /// Main application entry point
    /// </summary>
    static void Main()
    {
      var path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

      var checkFile = Path.Combine(path, "App.Config");
      if (!File.Exists(checkFile))
      {
        return;
      }

      #region Topshelf service hosting

      HostFactory.Run(c =>
        {
          c.SetServiceName("PDfService");
          c.SetDisplayName("Atlas PDF Engine");

          c.RunAsLocalSystem();

          c.Service<PDFService>(s =>
          {
            s.ConstructUsing(builder => new PDFService());
            s.WhenStarted(o => o.Start());
            s.WhenStopped(o => o.Stop());
          });
        });
      #endregion
    }


    /// <summary>
    /// Runs before service is installed- used to register this application as a Windows log event source
    /// </summary>
    public static void BeforeInstall()
    {
      try
      {
        if (!System.Diagnostics.EventLog.SourceExists("PDFService"))
        {
          System.Diagnostics.EventLog.CreateEventSource("PDFService", "PDFService");
        }
      }
      catch (Exception err)
      {
        throw new Exception(string.Format("Unexpected error trying  to register 'PDFService' Event Log Source in the Windows event log: '{0}'", err.Message));
      }
    }

  }
}

using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using Topshelf;

namespace AccountService
{
  class Program
  {
    [STAThread]
    static void Main(string[] args)
    {
      var path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

      var checkFile = Path.Combine(path, string.Format("{0}{1}", Assembly.GetExecutingAssembly().ManifestModule.Name, ".Config"));
      if (!File.Exists(checkFile))
      {
        throw new FileNotFoundException("Log configuration file was not found");
      }
      log4net.Config.XmlConfigurator.Configure();

      Atlas.Domain.DomainMapper.Map();

      #region Topshelf service hosting

      HostFactory.Run(x =>
      {
        x.SetServiceName("Atlas Online Account Service");
        x.Service<Engine>(s =>
        {
          s.ConstructUsing(name => new Engine());
          // Events
          s.WhenStarted(tc => tc.Start());
          s.WhenStopped(tc => tc.Stop());
          //s.WhenPaused                                                                                
          //s.WhenStopped
        });

        x.RunAsLocalSystem();

        x.DependsOnMsSql();
        x.DependsOnEventLog();

        // Register in event log
        x.BeforeInstall(() => BeforeInstall());

        x.SetDescription("Atlas Online Account Service enables communication between Atlas Online and the DB.");

        x.SetDisplayName("Atlas Online Account Service");
        x.SetServiceName("AtlasOnlineAccountService");
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
        if (!System.Diagnostics.EventLog.SourceExists("AtlasOnlineAccountService"))
        {
          System.Diagnostics.EventLog.CreateEventSource("AtlasOnlineAccountService", "AtlasOnlineAccountService");
        }
      }
      catch (Exception err)
      {
        throw new Exception(string.Format("Unexpected error trying  to register 'AtlasOnlineAccountService' Event Log Source in the Windows event log: '{0}'", err.Message));
      }
    }
  }
}

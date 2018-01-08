/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012-2016 Atlas Finance (Pty() Ltd.
*
*  Description:
*  ------------------
*    Main entry- Atlas Server Service- Core Atlas WCF Server
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
*
* ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Net;

using Topshelf;
using SimpleInjector;
using Topshelf.SimpleInjector;
using Topshelf.SimpleInjector.Quartz;

using Atlas.Common.Interface;
using Atlas.Server.WCF_DI;
using Atlas.WCF.QuartzTasks;
using Atlas.Servers.Common.Logging;
using Atlas.Servers.Common.Xpo;
using Atlas.Servers.Common.Config;
using Atlas.Cache.Interfaces;
using Atlas.Cache.Redis;


namespace Atlas.Server
{
  internal class Program
  {
    /// <summary>
    /// Main application entry point
    /// </summary>
    private static void Main()
    {
      try
      {
        // DI
        RegisterDependencies();

        // XPO
        XpoUtils.CreateXpoDomain(_config, _log);
              
          // WCF special config
        ServicePointManager.DefaultConnectionLimit = 1000;
        // 2 is default- Gets or sets the maximum number of concurrent connections allowed by a
        // ServicePoint (connection management for HTTP connections) object.
        // Set the maximum number of ServicePoint instances to maintain. If a ServicePoint instance
        // for that host already exists when your application requests a connection to an Internet
        // resource, the ServicePointManager object returns this existing ServicePoint instance.
        // If none exists for that host, it creates a new ServicePoint instance.

        Domain.DomainMapper.Map();

        #region TopShelf      
        HostFactory.Run(hc =>
        {
          // Config DI
          hc.UseSerilog();
          hc.UseSimpleInjector(_container);
          hc.UseQuartzSimpleInjector(_container); // can't specify delayed start?

          // Service settings
          hc.RunAsLocalSystem();
          hc.StartAutomatically();
          hc.SetServiceName("Atlas_Basic_WCF_Server_V1_5");
          hc.SetDisplayName("Atlas Basic WCF Server V1.5");
          hc.SetDescription("Atlas Basic WCF Server. This service exposes basic AVS, NAEDO, and TCC core functionality, via " +
                  "HTTP SOAP and .NET binary WCF services. If this service is stopped, this functionality will not be available to Atlas clients.");

          hc.Service<MainService>(sc =>
          {
            sc.ConstructUsingSimpleInjector();

            sc.WhenStarted((service, control) => service.Start());
            sc.WhenStopped((service, control) => service.Stop());

            sc.ScheduleQuartzJob(config => config.WithCronSchedule<QuartzTasks.Test>("0 0/1 7-20 ? * MON-FRI *", "Test"));

            #region Quartz tasks - TODO: Could use reflection to get IJob types and CRON attribute?            
            //Non-intrusive TCC terminal status check-every 1 minute, office-hours (NOTE: This must be minimum 1 per minute for logging uptime / minute)
            sc.ScheduleQuartzJob(config => config.WithCronSchedule<TCCCheckSafe>("0 0/1 7-20 ? * MON-FRI *", "TCCSafeWeekday"));
            sc.ScheduleQuartzJob(config => config.WithCronSchedule<TCCCheckSafe>("0 0/1 7-15 ? * SAT *", "TCCSafeSat"));

            //  Check for status stuck terminals (odd intervals are to avoid hitting the same terminal with the TCC 'safe' and 'stuck' checks)
            sc.ScheduleQuartzJob(config => config.WithCronSchedule<TCCCheckStatusStuck>("45 0/1 7-20 ? * MON-FRI *", "TCCStuckWeekday"));
            sc.ScheduleQuartzJob(config => config.WithCronSchedule<TCCCheckStatusStuck>("45 0/1 7-15 ? * SAT *", "TCCStuckSat"));

            // Purge old logs daily at 00:00
            sc.ScheduleQuartzJob(config => config.WithCronSchedule<DBCleanOldLogs>("0 0 0 ? * * *", "DBCleanOldLogs"));

            //  Reset all active terminals to status 2, to force handshake MON-SAT at 06:00
            sc.ScheduleQuartzJob(config => config.WithCronSchedule<TCCResetAllToUnknown>("0 0 6 ? * MON-SAT *", "TCCResetAllToUnknown"));

            // Double-check EDOs cancelled 03:45
            sc.ScheduleQuartzJob(config => config.WithCronSchedule<CancelPendingEDO>("0 45 3 ? * * *", "CancelPendingEDO"));
            #endregion
          });
        });
        #endregion
      }
      catch (Exception err)
      {
        _log.Error(err, "Main()");
        Console.WriteLine("Start-up error: '{0}'", err.Message);
        return;
      }
    }


    /// <summary>
    /// DI registration
    /// </summary>
    private static void RegisterDependencies()
    {
      // Infrastructure 
      // ---------------------------------------
      _container.RegisterSingleton(_log);
      _container.RegisterSingleton(_config);
      _container.RegisterSingleton(_cache);

      // WCF
      // ---------------------------------------
      _container.Register<ITCCTerminalServiceHost>(() => new TCCTerminalServiceHost(_container));
      _container.Register<IASSServiceHost>(() => new ASSServiceHost(_container));
      _container.Register<IAssCdvServiceHost>(() => new AssCdvServiceHost(_container));
      _container.Register<IThirdPartyServiceHost>(() => new ThirdPartyServiceHost(_container));
      _container.Register<IAssAvsServiceHost>(() => new AssAvsServiceHost(_container));
    }


    // *Cross-cutting concerns*-  we need instances up front, so create here and register as singletons
    private static readonly ILogging _log = new SerilogLogging("Atlas.ASS.Server", true, typeof(Program));
    private static readonly IConfigSettings _config = new ConfigFileSettings();
    private static readonly ICacheServer _cache = new RedisCacheServer(_config, _log);

    // DI
    private static readonly Container _container = new Container();

  }
}
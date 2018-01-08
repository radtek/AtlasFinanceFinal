/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012-2014 Atlas Finance (Pty) Ltd.
* 
* 
*  Description:
*  ------------------
*    Implementation of the NuCard
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
* ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Net;

using Topshelf;
using SimpleInjector;
using Topshelf.SimpleInjector;
using Topshelf.SimpleInjector.Quartz;

using Atlas.Common.Interface;
using Atlas.Servers.Common.Logging;
using Atlas.Servers.Common.Config;
using Atlas.Servers.Common.Xpo;
using Atlas.Server.NuCard.WCF.DI;
using Atlas.WCF.QuartzTasks;


namespace Atlas.Server.NuCard
{
  class Program
  {
    /// <summary>
    /// Main entry point
    /// </summary>
    static void Main()
    {
      _log.Information("Starting...");

      // DI
      RegisterDepedencies();
          
      // XPO
      XpoUtils.CreateXpoDomain(_config, _log);

      // AutoMapper
      Domain.DomainMapper.Map();
            
      ServicePointManager.DefaultConnectionLimit = 1000;
      // 2 is default- Gets or sets the maximum number of concurrent 
      // connections allowed by a ServicePoint (connection management 
      // for HTTP connections) object.

      #region Topshelf service hosting
      HostFactory.Run(hc =>
      {
        hc.UseSerilog();
        hc.UseSimpleInjector(_container);
        hc.UseQuartzSimpleInjector(_container); // can't specify delayed start?

        hc.RunAsLocalSystem();
        hc.StartAutomaticallyDelayed();
        hc.SetDisplayName("Atlas NuCard Server");
        hc.SetServiceName("Atlas_NuCard_WCF_Server_V1_1");
        hc.SetDescription("Atlas NuCard WCF Server. This service exposes NuCard core functionality via " +
                "HTTP SOAP and .NET binary WCF services. This service enables core Atlas functionality for calling from any client (Delphi, Harbour, C#). If this service is stopped, this " +
                "functionality will not be available.");

        hc.Service<MainService>(sc =>
        {
          sc.ConstructUsingSimpleInjector();
          sc.WhenStarted((service, control) => service.Start());
          sc.WhenStopped((service, control) => service.Stop());
          
          sc.ScheduleQuartzJob(config => config.WithCronSchedule<NuCardCheckCards>("0 0 0 ? * MON-FRI *", "CheckNuCardsDaily"));
        });
      });
      #endregion
    }


    private static void RegisterDepedencies()
    {
      // Infrastructure 
      // ---------------------------------------
      _container.RegisterSingleton(_log);
      // TODO: Create a config server and use to get connection/?WCF?/?app? settings
      _container.RegisterSingleton(_config);

      // WCF
      // ---------------------------------------
      _container.Register<INuCardAdminServiceHost>(() => new NuCardAdminServiceHost(_container));
      _container.Register<INuCardStockServiceHost>(() => new NuCardStockServiceHost(_container));
    }



    // *Cross-cutting concerns*  we need instances upfront, so create here and register as singletons
    private static readonly ILogging _log = new SerilogLogging("Atlas.NuCard.Server", true, typeof(Program));
    private static readonly IConfigSettings _config = new ConfigFileSettings();

    // DI
    private static readonly Container _container = new Container();

  }
}

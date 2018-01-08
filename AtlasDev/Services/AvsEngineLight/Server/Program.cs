using System;

using Topshelf;
using Topshelf.SimpleInjector;
using Topshelf.SimpleInjector.Quartz;
using SimpleInjector;

using Atlas.Servers.Common.Xpo;
using Atlas.Common.Interface;
using Atlas.Servers.Common.Logging;
using Atlas.Servers.Common.Config;
using AvsEngineLight.QuartzTasks;


namespace AvsEngineLight
{
  class Program
  {
    static void Main()
    {
      try
      {
        // DI
        RegisterDepedencies();

        // XPO
        XpoUtils.CreateXpoDomain(_config, _log);

        // AutoMapper
        Atlas.Domain.DomainMapper.Map();

        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11;

        #region Topshelf service hosting
        HostFactory.Run(hc =>
        {
          hc.Service<MainService>(sc =>
          {
            sc.ConstructUsingSimpleInjector();
            sc.WhenStarted((service, control) => service.Start());
            sc.WhenStopped((service, control) => service.Stop());

            var rnd = new Random();
            // Check XDS ok - operates 03:00 - 20:00
            sc.ScheduleQuartzJob(config => config.WithCronSchedule<CheckXdsResponsive>(
              string.Format("{0} 0/5 7-18 ? * MON-SAT *", 5 + rnd.Next(15)), "CheckXdsResponsive"));

            sc.ScheduleQuartzJob(config => config.WithCronSchedule<CheckNuCardResponsive>(
              string.Format("{0} 0/5 7-18 ? * MON-SAT *", 30 + rnd.Next(15)), "CheckNcResponsive"));
          });

          // Config DI
          hc.UseSerilog();
          hc.UseSimpleInjector(_container);
          hc.UseQuartzSimpleInjector(_container); // can't specify delayed start?

          // Service settings
          hc.RunAsLocalSystem();
          hc.StartAutomatically();

          hc.SetDisplayName("Atlas AVS Engine- Lightweight");
          hc.SetServiceName("Atlas_AVS_Engine");
          hc.SetDescription("This service carries out AVS communications with relevant AVS service providers and monitors AVS responses");          
        });
        #endregion
      }
      catch (Exception err)
      {
        Console.WriteLine("{0}", err.Message);
        Console.ReadKey();
        return;
      }
    }


    /// <summary>
    /// DI registration
    /// </summary>
    private static void RegisterDepedencies()
    {
      // Infrastructure 
      // ---------------------------------------
      _container.RegisterSingleton(_log);
      // TODO: Create a config server and use to get connection/?WCF?/?app? settings
      _container.RegisterSingleton(_config);
      _container.Register<AvsEngineLight.EasyNetQ.IAVSServiceBus, AvsEngineLight.EasyNetQ.AVSServiceBus>(Lifestyle.Singleton);
    }


    // *Cross-cutting concerns*  we need instances upfront, so create here and register as singletons
    private static readonly ILogging _log = new SerilogLogging("Atlas.AVSLite.Server", true, typeof(Program));
    private static readonly IConfigSettings _config = new ConfigFileSettings();

    // DI
    private static readonly Container _container = new Container();

  }
}

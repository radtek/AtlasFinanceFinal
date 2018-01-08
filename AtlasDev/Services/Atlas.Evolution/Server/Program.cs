using System;

using Topshelf;
using SimpleInjector;
using Topshelf.SimpleInjector;
using Topshelf.SimpleInjector.Quartz;

using Atlas.Common.Interface;
using Atlas.Servers.Common.Config;
using Atlas.Servers.Common.Logging;
using Atlas.Servers.Common.Xpo;
using Atlas.Evolution.Server.QuartzTasks;


namespace Atlas.Evolution.Server
{
  class Program
  {
    static void Main()
    {            
      _log.Information("Starting");
      try
      {
        // DI
        RegisterDependencies();

        // XPO
        XpoUtils.CreateXpoDomain(_config, _log);
              
        #region TopShelf service hosting
        HostFactory.Run(hc =>
        {
          // Config DI
          hc.UseSerilog();
          hc.UseSimpleInjector(_container);
          hc.UseQuartzSimpleInjector(_container); // can't specify delayed start?

          hc.RunAsLocalSystem();
          hc.StartAutomaticallyDelayed(); // Give time for system stabilization
          hc.SetServiceName("Atlas_Evolution_Server_V1_0");
          hc.SetDisplayName("Atlas Evolution Service V1.0");
          hc.SetDescription("Atlas Evolution Server. If this service is stopped, no Evolution submissions will be generated and uploaded.");
                    
          hc.Service<MainService>(sc =>
          {
            sc.ConstructUsingSimpleInjector();

            sc.WhenStarted((service, control) => service.Start());
            sc.WhenStopped((service, control) => service.Stop());

            // Daily at 20:00
            sc.ScheduleQuartzJob(config => config.WithCronSchedule<CreateDailyBatch>("0 0 20 1/1 * ? *", "generateDailySched"));

            // Monthly 21:00 on 3rd of the month
            sc.ScheduleQuartzJob(config => config.WithCronSchedule<CreateMonthlyBatch>("0 0 21 3 1/1 ? *", "generateMonthlySched"));

            // Daily every 15 minutes, except 18:00-23:59
            sc.ScheduleQuartzJob(config => config.WithCronSchedule<UploadPendingBatches>("0 0/15 0-18 ? * * *", "sftpUploadSched"));         
          });          
        });
        #endregion
      }
      catch (Exception err)
      {
        Console.WriteLine(err.StackTrace);
      }
    }

    private static void RegisterDependencies()
    {
      // Infrastructure 
      // ---------------------------------------
      _container.RegisterSingleton(_log);
      // TODO: Create a config server and use to get connection/?WCF?/?app? settings
      _container.RegisterSingleton(_config);
    }


    // *Cross-cutting concerns*  we need instances upfront, so create here and register as singletons
    private static readonly ILogging _log = new SerilogLogging("Atlas.Evolution.Server", true, typeof(Program));
    private static readonly IConfigSettings _config = new ConfigFileSettings();

    // DI
    private static readonly Container _container = new Container();

  }
}

using Topshelf;
using SimpleInjector;
using Topshelf.SimpleInjector;
using Topshelf.SimpleInjector.Quartz;

using Atlas.Common.Interface;
using Atlas.Servers.Common.Config;
using Atlas.Servers.Common.Logging;
using Atlas.Servers.Common.Xpo;
using Atlas.Integration.Scheduler.QuartzTasks.Opportunities;
using Atlas.Integration.Scheduler.QuartzTasks;


namespace Atlas.Integration.Scheduler
{
  class Program
  {
    static void Main()
    {
      // DI
      RegisterDependencies();

      // XPO
      XpoUtils.CreateXpoDomain(_config, _log, new[] { typeof(Domain.Model.Opportunity.OPP_CaseDetail) });
      
      #region Topshelf service hosting
      HostFactory.Run(hc =>
      {
        // Config DI
        hc.UseSerilog();
        hc.UseSimpleInjector(_container);
        hc.UseQuartzSimpleInjector(_container); // can't specify delayed start?

        hc.StartAutomaticallyDelayed();
        hc.RunAsLocalSystem();
        hc.SetServiceName("Integration_Scheduler");
        hc.SetDisplayName("Atlas Integration Scheduler");
        hc.SetDescription("Atlas Integration scheduling server for running integration-related Quartz tasks");

        hc.Service<MainService>(sc =>
        {
          sc.ConstructUsingSimpleInjector();

          sc.WhenStarted((service, control) => service.Start());
          sc.WhenStopped((service, control) => service.Stop());

          // Check on closed opportunities @ 06:38:42/09:38:42/12:38:42/14:38:42/16:38:42
          sc.ScheduleQuartzJob(config => config.WithCronSchedule<HandleOpportunitiesTask>("42 38 06,09,12,14,16 ? * MON-SAT", "CheckOpportunities"));

          // Check on new powerloans incoming loan applications via POP3 server, every 5 minutes Mon-Sat 06:00-20:59.
          sc.ScheduleQuartzJob(config => config.WithCronSchedule<HandleEMailsTask>("0 0/5 6-20 ? * MON-SAT", "HandleEMailsTask"));
        });
      });
      #endregion
    }


    /// <summary>
    /// DI registration
    /// </summary>
    private static void RegisterDependencies()
    {
      // Infrastructure 
      // ---------------------------------------
      _container.RegisterSingleton(_log);
      // TODO: Create a config server and use to get connection/?WCF?/?app? settings
      _container.RegisterSingleton(_config);     
    }


    // *Cross-cutting concerns*  we need instances upfront, so create here and register as singletons
    private static readonly ILogging _log = new SerilogLogging("Atlas.Integration.Scheduler", true, typeof(Program));
    private static readonly IConfigSettings _config = new ConfigFileSettings();

    // DI
    private static readonly Container _container = new Container();


  }
}

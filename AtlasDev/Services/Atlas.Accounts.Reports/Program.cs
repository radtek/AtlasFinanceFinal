/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012-2016 Atlas Finance (Pty() Ltd.
*
*  Description:
*  ------------------
*    Main entry- Atlas reporting
*
*
*  Author:
*  ------------------
*     Keith Blows
*
*
*  Revision history:
*  ------------------
*     2016-02  Created
*
*
* ----------------------------------------------------------------------------------------------------------------- */

using Topshelf;
using SimpleInjector;
using Topshelf.SimpleInjector;
using Topshelf.SimpleInjector.Quartz;

using Atlas.Common.Interface;
using Atlas.Servers.Common.Config;
using Atlas.Servers.Common.Logging;
using Atlas.Accounts.Reports.QuartzTasks;
using Atlas.Servers.Common.Xpo;


namespace Atlas.Accounts.Reports
{
  class Program
  {
    static void Main()
    {
      _log.Information("Starting...");

      // DI
      RegisterDependencies();

      // XPO
      XpoUtils.CreateXpoDomain(_config, _log);

      #region Topshelf service hosting
      HostFactory.Run(hc =>
      {
        // Config DI
        hc.UseSerilog();
        hc.UseSimpleInjector(_container);
        hc.UseQuartzSimpleInjector(_container); // can't specify delayed start?

        hc.SetServiceName("Atlas.Accounts.Reports");
        hc.SetDisplayName("Atlas.Accounts.Reports");
        hc.SetDescription("Atlas server for running accounts/sales automated reports");
        hc.RunAsLocalSystem();
        hc.StartAutomaticallyDelayed();

        hc.Service<MainService>(sc =>
        {
          sc.ConstructUsingSimpleInjector();
          sc.WhenStarted((service, control) => service.Start());
          sc.WhenStopped((service, control) => service.Stop());

          // Report on all manual receipts: mon-stat @ 04:15
          sc.ScheduleQuartzJob(config => config.WithCronSchedule<ManualReceipts>("0 15 04 ? * MON-SAT", "ManualReceipts"));

          // AVS SP Report- 1st of the month @ 06:00
          sc.ScheduleQuartzJob(config => config.WithCronSchedule<AVS_SP_Report>("0 0 6 1 1/1 ? *", "AVS"));

          // Day before overdues Daily @ 4am & 6pm
          sc.ScheduleQuartzJob(config => config.WithCronSchedule<DayBeforeOverdues>("0 0 4 1/1 * ? *", "Overdue4am"));
          sc.ScheduleQuartzJob(config => config.WithCronSchedule<DayBeforeOverdues>("0 0 18 1/1 * ? *", "Overdue6pm"));

          // 1st instalment missed for y/day @ 10:00 & 18:00
          sc.ScheduleQuartzJob(config => config.WithCronSchedule<FirstInstallOverdue>("0 0 10,18 1/1 * ? *", "MissedFirstAt10And18"));

          // 85% receipts at 19:15
          sc.ScheduleQuartzJob(config => config.WithCronSchedule<Receipts85Percent>("0 15 19 1/1 * ? *", "Receipts85Percent"));

          // Audit trail at 04:30
          sc.ScheduleQuartzJob(config => config.WithCronSchedule<Audit_Trail>("0 30 04 ? * TUE-SUN", "Audit_Trail"));
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
    private static readonly ILogging _log = new SerilogLogging("Atlas.Accounts.Reports", true, typeof(Program));
    private static readonly IConfigSettings _config = new ConfigFileSettings();

    // DI
    private static readonly Container _container = new Container();

  }
}

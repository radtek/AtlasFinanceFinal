using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using Atlas.Ass.Framework.Repository;
using Atlas.Ass.Repository;
using Atlas.Domain.Ass.Models;
using Atlas.Domain.Model;
using Autofac;
using Autofac.Extras.Quartz;
using AutoMapper;
using AutoMapper.Mappers;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Falcon.Common.Interfaces.Jobs;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Common.Interfaces.Services;
using Falcon.Common.Repository;
using Falcon.Common.Services;
using Falcon.DuckHawk.Jobs.JobBuilder;
using Falcon.DuckHawk.Jobs.QuartzTasks.ASS;
using Falcon.DuckHawk.Jobs.QuartzTasks.Avs;
using Falcon.DuckHawk.Jobs.QuartzTasks.Stream;
using Falcon.DuckHawk.Jobs.QuartzTasks.UserTracking;
using Falcon.TBR.Bureau.Interfaces;
using Falcon.TBR.Bureau.Repository;
using Falcon.TBR.Bureau.Service;
using MassTransit;
using Microsoft.AspNet.SignalR.Client;
using Quartz;
using Serilog;
using Serilog.Events;
using StackExchange.Redis;
using Stream.Domain.Models;
using Stream.Framework.Repository;
using Stream.Framework.Services;
using Stream.Repository;
using Stream.Service;

namespace Falcon.DuckHawk
{
  public static class StartUp
  {
    private static string _signalrHost = string.Empty;
    //private static string LEGACY_DATABASE = string.Empty;
    private static string _rabbitmqAddress = string.Empty;
    private static string _rabbitmqBinding = string.Empty;
    private static string _rabbitmqUsername = string.Empty;
    private static string _rabbitmqPassword = string.Empty;

    public static void Configuration()
    {
      #region DataLayer

      try
      {
        var connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        var dataStore = XpoDefault.GetConnectionProvider(connStr, AutoCreateOption.None);

        using (var dataLayer = new SimpleDataLayer(dataStore))
        {
          using (var session = new Session(dataLayer))
          {
            session.Dictionary.GetDataStoreSchema(typeof(STR_Case).Assembly, typeof(BRN_Branch).Assembly, typeof(ASS_CiReport).Assembly);
            XpoDefault.DataLayer = new ThreadSafeDataLayer(session.Dictionary, dataStore);
          }
        }
        XpoDefault.Session = null;
      }
      catch (Exception err)
      {
        throw new Exception("Error with XPO domain", err);
      }

      #endregion

      #region Statics

      _rabbitmqAddress = ConfigurationManager.AppSettings["rabbitmq-address"];
      _rabbitmqBinding = ConfigurationManager.AppSettings["rabbitmq-binding"];
      _rabbitmqUsername = ConfigurationManager.AppSettings["rabbitmq-username"];
      _rabbitmqPassword = ConfigurationManager.AppSettings["rabbitmq-password"];
      _signalrHost = ConfigurationManager.AppSettings["bindingHost.SignalR.Host"];

      #endregion

      var builder = new ContainerBuilder();

      builder.Register(context => ConnectionMultiplexer.Connect(new ConfigurationOptions()
      {
        EndPoints = { { ConfigurationManager.AppSettings["redis.host"] } },
        AllowAdmin = true,
        AbortOnConnectFail = false,
        SyncTimeout = 90000,
      }).GetDatabase()).SingleInstance();

      builder.Register(context => ServiceBusFactory.New(cfg =>
      {
        cfg.UseRabbitMq(
          r => r.ConfigureHost(new Uri(string.Format("rabbitmq://{0}/{1}", _rabbitmqAddress, _rabbitmqBinding)), h =>
          {
            h.SetUsername(_rabbitmqUsername);
            h.SetPassword(_rabbitmqPassword);
            h.SetRequestedHeartbeat(180);
          }));
        cfg.ReceiveFrom(string.Format("rabbitmq://{0}/{1}", _rabbitmqAddress, _rabbitmqBinding));
        cfg.EnableMessageTracing();
      })).SingleInstance();

      builder.Register(ctx => new ConfigurationStore(new TypeMapFactory(), MapperRegistry.Mappers))
        .AsImplementedInterfaces()
        .SingleInstance();

      builder.RegisterType<MappingEngine>().As<IMappingEngine>();

      // Loggers
      builder.Register(c => SerilogConfig.CreateLogger()).As<ILogger>().SingleInstance();

      // Jobs
      builder.RegisterType<AssCiImport>().As<IAssCiImportJob>().InstancePerLifetimeScope();
      builder.RegisterType<AssStreamAccountImport>().As<IAssStreamAccountImportJob>().InstancePerLifetimeScope();
      builder.RegisterType<AssStreamTransactionImport>().As<IAssStreamTransactionImportJob>().InstancePerLifetimeScope();
      builder.RegisterType<AvsCache>().As<IAvsCacheJob>().InstancePerLifetimeScope();
      builder.RegisterType<UserTrackingReporter>().As<IUserTrackingReporter>().InstancePerLifetimeScope();
      builder.RegisterType<UserTrackingRepository>().As<IUserTrackingRepository>().InstancePerLifetimeScope();
      builder.RegisterType<UserTrackingViolationReset>().As<IUserTrackingViolationReset>().InstancePerLifetimeScope();
      builder.RegisterType<StreamOnceADayJob>().As<IStreamOnceADayJob>().InstancePerLifetimeScope();
      builder.RegisterType<StreamReminder>().As<IStreamReminder>().InstancePerLifetimeScope();

      // Repositories
      builder.RegisterType<StreamRepository>().As<IStreamRepository>().InstancePerLifetimeScope();
      builder.RegisterType<StreamReportRepository>().As<IStreamReportRepository>().InstancePerLifetimeScope();

      builder.RegisterType<AssBureauRepository>().As<IAssBureauRepository>().InstancePerLifetimeScope();
      builder.RegisterType<AssCiRepository>().As<IAssCiRepository>().InstancePerLifetimeScope();
      builder.RegisterType<AssGeneralRepository>().As<IAssGeneralRepository>().InstancePerLifetimeScope();
      builder.RegisterType<AssStreamRepository>().As<IAssStreamRepository>().InstancePerLifetimeScope();

      builder.RegisterType<AssCiReportRepository>().As<IAssCiReportRepository>().InstancePerLifetimeScope();
      builder.RegisterType<CompanyRepository>().As<ICompanyRepository>().InstancePerLifetimeScope();
      builder.RegisterType<AvsRepository>().As<IAvsRepository>().InstancePerLifetimeScope();
      builder.RegisterType<UserTrackingRepository>().As<IUserTrackingRepository>().InstancePerLifetimeScope();
      builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerLifetimeScope();
      builder.RegisterType<BureauRepository>().As<IBureauRepository>().InstancePerLifetimeScope();
      builder.RegisterType<GeneralRepository>().As<IGeneralRepository>().InstancePerLifetimeScope();

      // Services
      builder.RegisterType<ConfigService>().As<IConfigService>().InstancePerLifetimeScope();
      builder.RegisterType<EmailService>().As<IEmailService>().InstancePerLifetimeScope();
      builder.RegisterType<SmsService>().As<ISmsService>().InstancePerLifetimeScope();
      builder.RegisterType<UserTrackingService>().As<IUserTrackingService>().InstancePerLifetimeScope();
      //builder.RegisterType<CollectService>().As<IStreamService>().InstancePerLifetimeScope();
      builder.RegisterType<SmsService>().As<ISmsService>().InstancePerLifetimeScope();
      builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
      builder.RegisterType<PdfService>().As<IPdfService>().InstancePerLifetimeScope();
      builder.RegisterType<BureauService>().As<IBureauService>().InstancePerLifetimeScope();
      builder.RegisterType<StreamService>().As<IStreamService>().InstancePerLifetimeScope();
      builder.RegisterType<CiReportService>().As<ICiReportService>().InstancePerLifetimeScope();


      // Schedular
      builder.RegisterModule(new QuartzAutofacFactoryModule());

      // Job
      builder.RegisterModule(new QuartzAutofacJobsModule(typeof(UserTrackingReporter).Assembly));

      // SignalR
      builder.Register(context => new HubConnection(_signalrHost)).As<HubConnection>().InstancePerLifetimeScope();

      //Build
      var container = builder.Build();

      // Setup Schedules
      var schedularFactory = container.Resolve<ISchedulerFactory>();

      var schedule = schedularFactory.GetScheduler();

      // Get jobs inheriting the IJob interface
      var types =
        Assembly.GetAssembly(typeof(UserTrackingViolationReset))
          .GetTypes()
          .Where(p => p.GetInterfaces().Contains(typeof(IJob)));

      var job = CustomJobBuilder.BuildJob(types);

      foreach (var j in job)
        schedule.ScheduleJob(j.Item1, j.Item2);

      // Start Schedule
      schedule.Start();
    }

    public class SerilogConfig
    {
      public static ILogger CreateLogger()
      {
        var config = new LoggerConfiguration().
          MinimumLevel.Debug().
          WriteTo.ColoredConsole(LogEventLevel.Debug).
          WriteTo.RollingFile(string.Format("{0}/{1}", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs"),
            "Falcon.JobRunner.txt")).
          WriteTo.Email(fromEmail: "falcon@atcorp.co.za", toEmail: "lee@atcorp.co.za", mailServer: "mail.atcorp.co.za", restrictedToMinimumLevel:LogEventLevel.Error);
        InitialiseGlobalContext(config);

        return config.CreateLogger();
      }

      public static LoggerConfiguration InitialiseGlobalContext(LoggerConfiguration configuration)
      {
        return configuration.Enrich.WithMachineName()
          .Enrich.WithProperty("ApplicationName", typeof(SerilogConfig).Assembly.GetName().Name)
          .Enrich.WithProperty("UserName", Environment.UserName)
          .Enrich.WithProperty("AppDomain", AppDomain.CurrentDomain)
          .Enrich.WithProperty("RuntimeVersion", Environment.Version)
          .Enrich.FromLogContext();
      }
    }
  }
}
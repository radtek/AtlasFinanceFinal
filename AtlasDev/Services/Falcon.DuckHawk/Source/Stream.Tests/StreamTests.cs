using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Atlas.Ass.Framework.Repository;
using Atlas.Ass.Repository;
using Autofac;
using AutoMapper;
using AutoMapper.Mappers;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Common.Interfaces.Services;
using Falcon.Common.Repository;
using Falcon.Common.Services;
using Falcon.TBR.Bureau.Interfaces;
using Falcon.TBR.Bureau.Repository;
using Falcon.TBR.Bureau.Service;
using MassTransit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Serilog.Events;
using StackExchange.Redis;
using Stream.Framework.Repository;
using Stream.Framework.Services;
using Stream.Repository;
using Stream.Service;

namespace Stream.Tests
{
  [TestClass]
  public class StreamTests
  {
    private IContainer _container;

    private void SetUp()
    {
      try
      {
        var connStr = "XpoProvider=Postgres;Server=172.31.91.165;Database=atlas_core;User Id=postgres;Password=s1DT81ChqlVkPZMlRO8b";

        var dataStore = XpoDefault.GetConnectionProvider(connStr, AutoCreateOption.None);

        using (var dataLayer = new SimpleDataLayer(dataStore))
        {
          using (var session = new Session(dataLayer))
          {
            XpoDefault.DataLayer = new ThreadSafeDataLayer(session.Dictionary, dataStore);
          }
        }
        XpoDefault.Session = null;
      }
      catch (Exception err)
      {
        throw new Exception("Error with XPO domain", err);
      }


       #region Statics

      var _rabbitmqAddress = "192.168.56.101";
      var _rabbitmqBinding = "streamtests";
      var _rabbitmqUsername =string.Empty;
      var _rabbitmqPassword = string.Empty;
      var redisHost = "192.168.56.101";

      #endregion

      var builder = new ContainerBuilder();

      builder.Register(context => ConnectionMultiplexer.Connect(new ConfigurationOptions()
      {
        EndPoints = { { redisHost } },
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

      //Build
      _container = builder.Build();
    }

    [TestMethod]
    public void SetCaseCategory()
    {
      SetUp();

      var streamService = _container.Resolve<IStreamService>();
      var streamRepository = _container.Resolve<IStreamRepository>();


      streamService.SetCaseCategory(4091777, Framework.Enumerators.Stream.GroupType.Collections);
    }

    [TestMethod]
    public void SetCasePriorityTest()
    {
      SetUp();

      var streamService = _container.Resolve<IStreamService>();

      streamService.SetCasePriority(4113399, Framework.Enumerators.Stream.GroupType.Collections);
    }

    [TestMethod]
    public void BreakPtps()
    {
      SetUp();

      var streamService = _container.Resolve<IStreamService>();

      streamService.CheckAllCollectionPtps();
    }

    [TestMethod]
    public void CompleteCase()
    {
      SetUp();

      var streamRepo = _container.Resolve<IStreamRepository>();
      var casesToClosed = new[]
      {
        1021054,
        1014594,
        1021037,
        1014786,
        1015353
      };

      foreach (var i in casesToClosed)
      {
        streamRepo.CloseCase(i);
      }
    }

    public class SerilogConfig
    {
      public static ILogger CreateLogger()
      {
        var config = new LoggerConfiguration().
          MinimumLevel.Debug().
          WriteTo.ColoredConsole(LogEventLevel.Debug).
          WriteTo.RollingFile(string.Format("{0}/{1}", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs"),
            "Falcon.JobRunner.txt"));
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

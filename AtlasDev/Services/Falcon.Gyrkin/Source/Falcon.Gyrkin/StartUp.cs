using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web.Http;
using Atlas.Ass.Framework.Repository;
using Atlas.Ass.Repository;
using Atlas.Domain.Ass.Models;
using Atlas.Domain.Model;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Autofac;
using Autofac.Integration.SignalR;
using Autofac.Integration.WebApi;
using AutoMapper;
using AutoMapper.Mappers;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Owin;
using Serilog;
using Serilog.Events;

using Falcon.Common.Interfaces.Repositories;
using Falcon.Common.Interfaces.Services;
using Falcon.Common.Repository;
using Falcon.Common.Services;
using Falcon.Gyrkin.ESB;
using Falcon.Gyrkin.Library.Handler;
using Falcon.TBR.Bureau.Interfaces;
using Falcon.TBR.Bureau.Repository;
using Falcon.TBR.Bureau.Service;
using MassTransit;
using Serilog.Formatting.Json;
using Serilog.Sinks.RollingFile;
using Stream.Domain.Models;
using Stream.Framework.Repository;
using Stream.Framework.Services;
using Stream.Repository;
using Stream.Service;

namespace Falcon.Gyrkin
{
  public class StartUp
  {
    private static string _rabbitmqAddress = string.Empty;
    private static string _rabbitmqBinding = string.Empty;
    private static string _rabbitmqUsername = string.Empty;
    private static string _rabbitmqPassword = string.Empty;
    private static string _legacyDatabase = string.Empty;

    public void Configuration(IAppBuilder appBuilder)
    {
      #region DataLayer

      try
      {
        var connStr = ConfigurationManager.ConnectionStrings["AtlasCore"].ConnectionString;

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
      _legacyDatabase = ConfigurationManager.ConnectionStrings["AssConnection"].ConnectionString;

      #endregion

      #region Owin

      #region Controller Caching

      //var assembly = Assembly.GetAssembly(typeof(ControllerDiscoveryeSelector));
      //var controllerTypes = assembly.GetTypes(); //GetExportedTypes doesn't work with dynamic assemblies
      //var matchedTypes = controllerTypes.Where(i => typeof(IHttpController).IsAssignableFrom(i)).ToList();

      //foreach (var ctrl in matchedTypes)
      //  ControllerCacher.Set(ctrl.Name.Substring(0, ctrl.Name.IndexOf("Controller")).ToLower(), ctrl);

      #endregion

      HttpConfiguration config = new HttpConfiguration();

      //config.Services.Replace(typeof(ITraceWriter), new SeriLogTracer());
      //config.Services.Replace(typeof(IHttpControllerSelector), new ControllerDiscoveryeSelector(config));

      //config.EnableSystemDiagnosticsTracing();

      //config.MessageHandlers.Add(new OwinTokenValidationHandler());

      config.Routes.MapHttpRoute("DefaultHttpRoute", "api/{controller}/{action}/{id}",
        defaults: new {id = RouteParameter.Optional});


      var json = config.Formatters.JsonFormatter;
      json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
      json.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
      config.Formatters.Remove(config.Formatters.XmlFormatter);


      #endregion

      #region Autofac Registrations

      var builder = new ContainerBuilder();

      builder.Register(ctx => new ConfigurationStore(new TypeMapFactory(), MapperRegistry.Mappers))
        .AsImplementedInterfaces()
        .SingleInstance();

      builder.RegisterType<MappingEngine>().As<IMappingEngine>().SingleInstance();

      // Register API Controllers
      builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
      builder.Register(context => new Npgsql.NpgsqlConnection(_legacyDatabase)).SingleInstance();

      // Register Repositories
      builder.RegisterType<StreamRepository>().As<IStreamRepository>().InstancePerLifetimeScope();
      builder.RegisterType<StreamReportRepository>().As<IStreamReportRepository>().InstancePerLifetimeScope();
      builder.RegisterType<AssCiReportRepository>().As<IAssCiReportRepository>().InstancePerRequest();
      builder.RegisterType<AssBureauRepository>().As<IAssBureauRepository>().InstancePerLifetimeScope();
      builder.RegisterType<AssCiRepository>().As<IAssCiRepository>().InstancePerLifetimeScope();
      builder.RegisterType<AssGeneralRepository>().As<IAssGeneralRepository>().InstancePerLifetimeScope();
      builder.RegisterType<AssStreamRepository>().As<IAssStreamRepository>().InstancePerLifetimeScope();

      builder.RegisterType<NotificationRepository>().As<INotificationRepository>().InstancePerRequest();
      builder.RegisterType<DebitOrderRepository>().As<IDebitOrderRepository>().InstancePerRequest();
      builder.RegisterType<AvsRepository>().As<IAvsRepository>().InstancePerRequest();
      builder.RegisterType<UserTrackingRepository>().As<IUserTrackingRepository>().InstancePerRequest();
      builder.RegisterType<CompanyRepository>().As<ICompanyRepository>().InstancePerRequest();
      builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerRequest();
      builder.RegisterType<BureauRepository>().As<IBureauRepository>().InstancePerRequest();
      builder.RegisterType<TargetRepository>().As<ITargetRepository>().InstancePerRequest();

      builder.RegisterType<GeneralRepository>().As<IGeneralRepository>().InstancePerLifetimeScope();

      // Register Services
      builder.RegisterType<ConfigService>().As<IConfigService>().InstancePerRequest();
      builder.RegisterType<CampaignService>().As<ICampaignService>().InstancePerRequest();
      builder.RegisterType<DebitOrderService>().As<IDebitOrderService>().InstancePerRequest();
      builder.RegisterType<UserTrackingService>().As<IUserTrackingService>().InstancePerRequest();
      //builder.RegisterType<CollectService>().As<IStreamService>().InstancePerRequest();
      builder.RegisterType<SmsService>().As<ISmsService>().InstancePerRequest();
      builder.RegisterType<UserService>().As<IUserService>().InstancePerRequest();
      builder.RegisterType<PdfService>().As<IPdfService>().InstancePerRequest();
      builder.RegisterType<BureauService>().As<IBureauService>().InstancePerRequest();
      builder.RegisterType<CiReportService>().As<ICiReportService>().InstancePerRequest();
      builder.RegisterType<TargetService>().As<ITargetService>().InstancePerRequest();
      builder.RegisterType<StreamService>().As<IStreamService>().InstancePerRequest();
      builder.RegisterType<EmailService>().As<IEmailService>().InstancePerLifetimeScope();

      // Register Hubs
      //builder.RegisterType<StreamHub>().ExternallyOwned().InstancePerRequest();
      //builder.RegisterHubs(Assembly.GetAssembly(typeof(StreamHub))).ExternallyOwned();

      // Register Logger
      builder.Register(c => SerilogConfig.CreateLogger()).As<ILogger>().SingleInstance();

      #endregion

      #region MassTransit

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

      #endregion

      #region Redis

      builder.Register(
        context => StackExchange.Redis.ConnectionMultiplexer.Connect(new StackExchange.Redis.ConfigurationOptions()
        {
          EndPoints = {{ConfigurationManager.AppSettings["redis.host"]}},
          AllowAdmin = true,
          AbortOnConnectFail = false,
          SyncTimeout = 90000
        }).GetDatabase()).SingleInstance();

      #endregion

      var container = builder.Build();


      #region Middlware Wiring

      appBuilder.UseCors(CorsOptions.AllowAll);

      #region SignalR

      appBuilder.Map("/signalr", map =>
      {
        map.UseCors(CorsOptions.AllowAll);

        var hubConfiguration = new HubConfiguration
        {
          EnableDetailedErrors = true, // debugging only,
          Resolver = new AutofacDependencyResolver(container)
        };

        map.RunSignalR(hubConfiguration);
      });

      #endregion

      var resolver = new AutofacWebApiDependencyResolver(container);
      config.DependencyResolver = resolver;

      // Placed here to exclude signalR from the compression
      config.MessageHandlers.Add(new CompressHandler());

      // AutoFac to Owin middleware
      appBuilder.UseAutofacMiddleware(container);

      // auto fac Web API pass through
      appBuilder.UseAutofacWebApi(config);

      // Standard Web api
      appBuilder.UseWebApi(config);

      #endregion

      // Start ESB interface
      EsbBootstrapConfigurator.Start(container);
    }

    public class SerilogConfig
    {
      public static ILogger CreateLogger()
      {
        var config = new LoggerConfiguration().
          MinimumLevel.Debug().
          WriteTo.ColoredConsole(LogEventLevel.Error).
          WriteTo.Sink(new RollingFileSink("log-debug-{Date}.json", new JsonFormatter(renderMessage: true), null, null), LogEventLevel.Debug).
          WriteTo.RollingFile(string.Format("{0}/{1}", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs"),
            "Falcon.Gyrkin.txt"));
        //WriteTo.Sink(new RollingFileSink("log-{Date}.json", new JsonFormatter(renderMessage: true), null, null), LogEventLevel.Warning);

        InitialiseGlobalContext(config);

        return config.CreateLogger();
      }

      public static LoggerConfiguration InitialiseGlobalContext(LoggerConfiguration configuration)
      {
        return configuration.Enrich.WithMachineName()
          .Enrich.WithProperty("ApplicationName", typeof (SerilogConfig).Assembly.GetName().Name)
          .Enrich.WithProperty("UserName", Environment.UserName)
          .Enrich.WithProperty("AppDomain", AppDomain.CurrentDomain)
          .Enrich.WithProperty("RuntimeVersion", Environment.Version)
          .Enrich.FromLogContext();
      }
    }
  }
}
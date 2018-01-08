using System;
using System.Configuration;
using System.IO;
using Atlas.Colony.Integration.Service.Map;
using Atlas.Colony.Integration.Service.Saga;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using MassTransit;
using MassTransit.NHibernateIntegration.Saga;
using NHibernate;
using Ninject;
using Ninject.Modules;
using Serilog;
using MassTransit;
using NHibernate.Tool.hbm2ddl;
using Ninject.Extensions.Conventions;
using MassTransit.Saga;

namespace Atlas.Colony.Integration.Service
{
  public class CouponRegistry : NinjectModule
  {
    private string RABBITMQ_ADDRESS = string.Empty;
    private string RABBITMQ_BINDING = string.Empty;
    private string RABBITMQ_USERNAME = string.Empty;
    private string RABBITMQ_PASSWORD = string.Empty;
    private string SAGA_CONNECTIONSTRING = string.Empty;

    public override void Load()
    {
      RABBITMQ_ADDRESS = ConfigurationManager.AppSettings["rabbitmq-address"];
      RABBITMQ_BINDING = ConfigurationManager.AppSettings["rabbitmq-binding"];
      RABBITMQ_USERNAME = ConfigurationManager.AppSettings["rabbitmq-username"];
      RABBITMQ_PASSWORD = ConfigurationManager.AppSettings["rabbitmq-password"];
      SAGA_CONNECTIONSTRING = ConfigurationManager.AppSettings["Saga.ConnectionString"];

      Bind<Engine>().To<Engine>().InSingletonScope();

      Kernel.Bind(x =>
                     x.FromThisAssembly()
                         .SelectAllClasses()
                             .InheritedFrom<IBusService>().BindAllInterfaces());

      Bind<ISagaRepository<CouponIssueSaga>>()
               .To(typeof(NHibernateSagaRepository<CouponIssueSaga>))
                  .InSingletonScope();

      Bind<ISagaRepository<SmsSendSaga>>()
              .To(typeof (NHibernateSagaRepository<SmsSendSaga>))
                 .InSingletonScope();

      Bind<ISessionFactory>().ToMethod((cfg) => Fluently.Configure()
       .Database(PostgreSQLConfiguration.PostgreSQL82
       .ConnectionString(c => c.Is(SAGA_CONNECTIONSTRING)))
       .Mappings(m =>
       {
         m.FluentMappings.AddFromAssemblyOf<CouponIssueSagaMap>();
       })
       .ExposeConfiguration(c => new SchemaUpdate(c).Execute(true, true))
       .BuildSessionFactory()).InSingletonScope();

      Bind<IServiceBus>().ToMethod(
        context =>
        {
          return ServiceBusFactory.New(config =>
          {
            config.UseRabbitMq(
              r =>
                r.ConfigureHost(new Uri(string.Format("rabbitmq://{0}/{1}", RABBITMQ_ADDRESS, RABBITMQ_BINDING)), h =>
                {
                  h.SetRequestedHeartbeat(120);
                  h.SetUsername(RABBITMQ_USERNAME);
                  h.SetPassword(RABBITMQ_PASSWORD);
                }));
            config.ReceiveFrom(string.Format("rabbitmq://{0}/{1}", RABBITMQ_ADDRESS, RABBITMQ_BINDING));
            config.UseControlBus();
            config.UseNHibernateSubscriptionStorage(Kernel.Get<ISessionFactory>());
            config.UseSerilog(new LoggerConfiguration()
              .MinimumLevel
              .Debug()
              .WriteTo.ColoredConsole()
              .WriteTo.RollingFile(string.Format("{0}/{1}", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs"),
                "MQ.txt"))
              .CreateLogger()
              );
          });
        }).InSingletonScope();
    }

    protected virtual INinjectSettings CreateSettings()
    {
      var settings = new NinjectSettings();
      settings.LoadExtensions = false;
      return settings;
    }
  }
}

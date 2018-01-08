using System.Reflection;
using Atlas.Online.Transaction.Processor.Maps;
using Atlas.Online.Transaction.Processor.Sagas;
using MassTransit;
using MassTransit.NHibernateIntegration.Saga;
using MassTransit.Saga;
using NHibernate.Tool.hbm2ddl;
using Ninject;
using Ninject.Modules;
using System;
using System.Configuration;
using Ninject.Extensions.Conventions;
using Serilog;
using System.IO;
using NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;


namespace Atlas.Online.Transaction.Processor
{
  public class TransactionProcessorRegistry : NinjectModule
  {
    #region Privates

    private string RABBITMQ_ADDRESS = string.Empty;
    private string RABBITMQ_BINDING = string.Empty;
    private string RABBITMQ_USERNAME = string.Empty;
    private string RABBITMQ_PASSWORD = string.Empty;
    private string SAGA_CONNECTIONSTRING = string.Empty;


    #endregion

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

      Bind<ISagaRepository<ClientCreationSaga>>()
                  .To(typeof(NHibernateSagaRepository<ClientCreationSaga>))
                     .InSingletonScope();

      Bind<ISagaRepository<AccountCreationSaga>>()
                        .To(typeof(NHibernateSagaRepository<AccountCreationSaga>))
                           .InSingletonScope();

      Bind<ISagaRepository<FraudPreventionSaga>>()
                        .To(typeof(NHibernateSagaRepository<FraudPreventionSaga>))
                           .InSingletonScope();

      Bind<ISagaRepository<CreditCheckSaga>>()
        .To(typeof(NHibernateSagaRepository<CreditCheckSaga>))
              .InSingletonScope();

      Bind<ISagaRepository<AccountVerificationSaga>>()
        .To(typeof(NHibernateSagaRepository<AccountVerificationSaga>))
              .InSingletonScope();

      Bind<ISagaRepository<AffordabilitySaga>>()
        .To(typeof(NHibernateSagaRepository<AffordabilitySaga>))
              .InSingletonScope();

      Bind<ISagaRepository<DecisionSaga>>()
                       .To(typeof(NHibernateSagaRepository<DecisionSaga>))
                          .InSingletonScope();

      Bind<ISessionFactory>().ToMethod((cfg) => Fluently.Configure()
        .Database(PostgreSQLConfiguration.PostgreSQL82
        .ConnectionString(c => c.Is(SAGA_CONNECTIONSTRING)))
        .Mappings(m =>
        {
          m.FluentMappings.AddFromAssemblyOf<AccountCreationSagaMap>();
        })
        .ExposeConfiguration(c => new SchemaUpdate(c).Execute(false, true))
        .BuildSessionFactory()).InSingletonScope();

      

      Bind<IServiceBus>().ToMethod(
      context =>
      {
        return ServiceBusFactory.New(config =>
        {
          config.UseRabbitMq(r => r.ConfigureHost(new Uri(string.Format("rabbitmq://{0}/{1}", RABBITMQ_ADDRESS, RABBITMQ_BINDING)), h =>
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
                                .WriteTo.RollingFile(string.Format("{0}/{1}",Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs"), "MQ.txt"))
                                .CreateLogger()
                            );
        });
      }).InSingletonScope();
    }

    protected virtual INinjectSettings CreateSettings()
    {
      var settings = new NinjectSettings();
      settings.LoadExtensions = true;
      return settings;
    }

    /*public static ISessionFactory BuildSessionFactory()
    {
      return Fluently.Configure()
        .Database(MsSqlConfiguration.MsSql2012
          .AdoNetBatchSize(100)
          .ConnectionString(s => s.Is("Data Source=localhost;User Id=sa;Password=fabian832007;Initial Catalog=sagaRepository;")))
        .Mappings(m =>
        {
          m.FluentMappings.Add<ClientCreationSagaMap>();
          m.FluentMappings.Add<AccountCreationSaga>();
          m.FluentMappings.Add<FraudPreventionSagaMap>();
        })
        .ExposeConfiguration(c => new SchemaUpdate(c).Execute(false, true))
        .BuildSessionFactory();
    }*/
  }
}

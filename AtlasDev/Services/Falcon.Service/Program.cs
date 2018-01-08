using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using Atlas.Ass.Framework.Repository;
using Atlas.Ass.Repository;
using Autofac;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Falcon.Service.Core;
using MassTransit;
using Serilog;
using Serilog.Enrichers;
using Topshelf;
using Topshelf.Autofac;

using Atlas.Domain;
using Atlas.Domain.Model;
using Autofac.Extras.Quartz;
using Falcon.Common.Interfaces.Services;
using Falcon.Common.Services;
using Falcon.Service.Tasks;
using Stream.Domain.Models;


namespace Falcon.Service
{
  internal class Program
  {
    private static string _rabbitmqAddress = string.Empty;
    private static string _rabbitmqBinding = string.Empty;
    private static string _rabbitmqUsername = string.Empty;
    private static string _rabbitmqPassword = string.Empty;
    private static string _legacyDatabase;

    [STAThread]
    private static void Main()
    {
      _rabbitmqAddress = ConfigurationManager.AppSettings["rabbitmq-address"];
      _rabbitmqBinding = ConfigurationManager.AppSettings["rabbitmq-binding"];
      _rabbitmqUsername = ConfigurationManager.AppSettings["rabbitmq-username"];
      _rabbitmqPassword = ConfigurationManager.AppSettings["rabbitmq-password"];
      _legacyDatabase = ConfigurationManager.ConnectionStrings["AssConnection"].ConnectionString;

      try
      {
        Log.Logger = new LoggerConfiguration()
          .WriteTo.ColoredConsole().Enrich.With(new ThreadIdEnricher()).Enrich.WithProperty("SourceContext", "")
          .WriteTo.RollingFile(string.Format("{0}/{1}", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs"),
            "Falcon.txt"))
          .CreateLogger();

        var builder = new ContainerBuilder();

        // Schedular
        builder.RegisterModule(new QuartzAutofacFactoryModule());

         //Job
        builder.RegisterModule(new QuartzAutofacJobsModule(typeof(AssCiReportTask).Assembly));

        builder.Register(context => new Npgsql.NpgsqlConnection(_legacyDatabase)).SingleInstance();
        builder.RegisterType<ConfigService>().As<IConfigService>().InstancePerRequest();
        builder.RegisterType<AssBureauRepository>().As<IAssBureauRepository>().InstancePerLifetimeScope();
        builder.RegisterType<AssCiRepository>().As<IAssCiRepository>().InstancePerLifetimeScope();

        builder.RegisterType<Engine>();
        builder.RegisterType<TaskSpooler>();

        builder.Register(context => ServiceBusFactory.New(config =>
        {
          config.UseRabbitMq(
            r => r.ConfigureHost(new Uri(string.Format("rabbitmq://{0}/{1}", _rabbitmqAddress, _rabbitmqBinding)), h =>
            {
              h.SetUsername(_rabbitmqUsername);
              h.SetPassword(_rabbitmqPassword);
              h.SetRequestedHeartbeat(180);
            }));
          config.ReceiveFrom(string.Format("rabbitmq://{0}/{1}", _rabbitmqAddress, _rabbitmqBinding));
          config.EnableMessageTracing();
          config.EnableRemoteIntrospection();
        }));
        var container = builder.Build();

        #region Start XPO- Create a thread-safe data layer

        var domainConstruction = ConfigurationManager.AppSettings["Atlas.Domain.Construct"];

        try
        {
          // Create thread-safe- load and build the domain!
          var connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
          IDataStore dataStore = null;

          if (domainConstruction.Equals("true"))
          {
            dataStore = XpoDefault.GetConnectionProvider(connStr, AutoCreateOption.SchemaOnly);
          }
          else
          {
            dataStore = XpoDefault.GetConnectionProvider(connStr, AutoCreateOption.None);
          }

          using (var dataLayer = new SimpleDataLayer(dataStore))
          {
            using (var session = new Session(dataLayer))
            {
              if (domainConstruction.Equals("true"))
              {
                session.UpdateSchema();
                session.UpdateSchema(Assembly.GetAssembly(typeof(PER_Person)), Assembly.GetAssembly(typeof(STR_Case)));
                session.CreateObjectTypeRecords();
              }
              XpoDefault.DataLayer = new ThreadSafeDataLayer(session.Dictionary, dataStore);
            }
          }
          XpoDefault.Session = null;
        }
        catch (Exception err)
        {
          throw new Exception("Error with XPO domain: " + err.Message);
        }

        DomainMapper.Map();

        #endregion

        HostFactory.Run(c =>
        {
          c.UseAssemblyInfoForServiceInfo();

          c.RunAsLocalSystem();
          c.UseAutofacContainer(container);

          c.Service<Engine>(s =>
          {
            s.ConstructUsing(() => container.Resolve<Engine>());
            s.WhenStarted(o => o.Start(container));
            s.WhenStopped(o => o.Stop());
          });
        });
      }
      catch (Exception ex)
      {
        Console.WriteLine(@"ERROR: {0}  {1}", ex.Message, ex.StackTrace);
        Console.ReadKey();
      }
    }
  }
}
using MassTransit;
using Ninject.Modules;
using Ninject;
using Atlas.Online.Node.Core;
using MassTransit.Log4NetIntegration;
using System.Configuration;
using DevExpress.Xpo.DB;
using DevExpress.Xpo;
using Atlas.Online.Data.Models.Definitions;

namespace Atlas.Online.Node.EmailNode
{
  public class EmailNodeRegistry : NinjectModule
  {
    private string RABBITMQ_ADDRESS = string.Empty;
    private string RABBITMQ_BINDING = string.Empty;
    public override void Load()
    {
      RABBITMQ_ADDRESS = ConfigurationManager.AppSettings["rabbitmq-address"];
      RABBITMQ_BINDING = ConfigurationManager.AppSettings["rabbitmq-binding"];

      Bind<EmailServiceNode>().To<EmailServiceNode>().InSingletonScope();
      Bind<IPulse>().To<Pulse>().InSingletonScope();
      // Create thread-safe- load and build the domain!
      var connStr = ConfigurationManager.ConnectionStrings["AtlasOnline"].ConnectionString;
      var dataStore = XpoDefault.GetConnectionProvider(connStr, AutoCreateOption.None);
      using (var dataLayer = new SimpleDataLayer(dataStore))
      {
        using (var session = new Session(dataLayer))
        {
          //session.UpdateSchema();
          //session.UpdateSchema(typeof(Address));
          //session.CreateObjectTypeRecords();
          XpoDefault.DataLayer = new ThreadSafeDataLayer(session.Dictionary, dataStore);
        }
      }
      XpoDefault.Session = null;

      Bind<IServiceBus>().ToMethod(context =>
      {
        return ServiceBusFactory.New(config =>
        {
          config.UseRabbitMq();
          config.ReceiveFrom(string.Format("rabbitmq://{0}/{1}", RABBITMQ_ADDRESS, RABBITMQ_BINDING));
          config.UseControlBus();
          config.EnableMessageTracing();
          //config.UseHealthMonitoring(5);
          config.UseLog4Net();
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

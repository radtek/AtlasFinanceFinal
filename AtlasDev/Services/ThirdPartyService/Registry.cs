using MassTransit;
using MassTransit.Log4NetIntegration;
using Ninject;
using Ninject.Modules;
using System;
using System.Configuration;

namespace Atlas.ThirdParty.Service
{
  public class ThirdPartyServiceRegistry : NinjectModule
  {
    private string RABBITMQ_ADDRESS = string.Empty;
    private string RABBITMQ_BINDING = string.Empty;
    private string RABBITMQ_USERNAME = string.Empty;
    private string RABBITMQ_PASSWORD = string.Empty;

    public override void Load()
    {
      RABBITMQ_ADDRESS = ConfigurationManager.AppSettings["rabbitmq-address"];
      RABBITMQ_BINDING = ConfigurationManager.AppSettings["rabbitmq-binding"];
      RABBITMQ_USERNAME = ConfigurationManager.AppSettings["rabbitmq-username"];
      RABBITMQ_PASSWORD = ConfigurationManager.AppSettings["rabbitmq-password"];
      Bind<Engine>().To<Engine>().InSingletonScope();


      Bus.Instance(ServiceBusFactory.New(config =>
      {
        config.UseRabbitMq(r => r.ConfigureHost(new Uri(string.Format("rabbitmq://{0}/{1}", RABBITMQ_ADDRESS, RABBITMQ_BINDING)),
          h =>
          {
            h.SetUsername(RABBITMQ_USERNAME);
            h.SetPassword(RABBITMQ_PASSWORD);
            h.SetRequestedHeartbeat(125);
          }));
        config.ReceiveFrom(string.Format("rabbitmq://{0}/{1}", RABBITMQ_ADDRESS, RABBITMQ_BINDING));
        config.UseControlBus();
        config.EnableMessageTracing();
        config.EnableRemoteIntrospection();
        config.UseLog4Net();
      }));
    }

    protected virtual INinjectSettings CreateSettings()
    {
      var settings = new NinjectSettings();
      settings.LoadExtensions = false;
      return settings;
    }
  }
}
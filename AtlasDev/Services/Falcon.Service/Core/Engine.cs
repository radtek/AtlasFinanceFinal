using System;
using System.IO;
using System.ServiceModel;
using Atlas.RabbitMQ.Messages.Push;
using Autofac;
using Falcon.Service.Interface;
using Falcon.Service.WCF.Implementation;
using MassTransit;
using Serilog;

namespace Falcon.Service.Core
{
  public sealed class Engine : IService
  {
    private static ILogger _logger = Log.Logger;
    private static IContainer _container = null;
    private ServiceHost _webService = null;
    //private IXSocketServerContainer _xSocket = Composable.GetExport<IXSocketServerContainer>();

    public Engine()
    {
    }

    public void Start(IContainer container)
    {
      _container = container;
      Console.Write(File.ReadAllText("falcon.asc") + "\n\r");
      _webService = new ServiceHost(typeof(FalconService));
      _webService.Open();
      _logger.Information("[FalconService] - Successfully loaded 'FalconService' service");
      _logger.Information("[FalconService] - WCF Started");
      _logger.Information("[FalconService] - Setting up queue handler");
      //container.Resolve<IServiceBus>().SubscribeHandler<PushMessage>(RabbitMessageHandler.PushMessageHandler);
      MqBus.Instance(container.Resolve<IServiceBus>());
      _logger.Information("[FalconService] - Queue handler setup");
      //_logger.Information("[FalconService] - Starting XSocket Server...");
      ////_xSocket.StartServers();
      //_logger.Information("[FalconService] - Started XSocket Server.");
      _container.Resolve<TaskSpooler>().Start();
    }

    public void Stop()
    {
      _logger.Information("[FalconService] - Cleaning up resources.");
      //_xSocket.StopServers();
      //_xSocket.Dispose();
      _container.Dispose();
      _logger.Information("[FalconService] - Shutting Down.");
      
    }
  }
}

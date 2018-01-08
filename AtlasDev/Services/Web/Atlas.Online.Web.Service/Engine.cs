using MassTransit;
using Ninject;
using Ninject.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Configuration;
using DevExpress.Xpo.DB;
using DevExpress.Xpo;
using Atlas.Online.Web.Service.Interface.Atlas.Online.Node.Core;
using System.ServiceModel;
using Atlas.Online.Web.Service.EasyNetQ;
using Atlas.Online.Web.Service.WCF.Implementation;
using Atlas.RabbitMQ.Messages.Online;


namespace Atlas.Online.Web.Service
{
  public sealed class Engine : IService
  {
    private static ILogger _logger = null;
    private static IKernel _kernal = null;
    private ServiceHost _webService = null;

    public Engine(ILogger ilogger, IKernel ikernal)
    {
      _logger = ilogger;
      _kernal = ikernal;
    }

    public void Start()
    {
      _logger.Info("[Online] - Starting Engine");
      try
      {
        _logger.Info("[Online] - Engine Started");
        _logger.Info("[Online] - Start WCF Channel...");
        try
        {
          _webService = new ServiceHost(typeof(WebServer));
          _webService.Open();
          _logger.Info("[Online] - Successfully loaded 'WebServer' service");
        }
        catch (Exception err)
        {
          _webService = null;
          _logger.Fatal("[Online] -Failed to load 'Config' service", err);
        }

        _logger.Info("[Online] - WCF Started");
        _logger.Info("[Online] - Setting up queue handler");

        var atlasOnlineServiceBus = _kernal.Get<AtlasOnlineServiceBus>();

        atlasOnlineServiceBus.GetServiceBus().Subscribe<UpdateApplicationInformation>("queue_UpdateApplicationInformation",
          msg =>
          {
            new SubscriptionHandlers().UpdateApplicationInformation(msg);
          });

        _logger.Info("[Online] - Queue handler setup");

        _kernal.Get<TaskSpooler>().Start();

        ServiceLocator.SetServiceLocator(_kernal);
      }
      catch (Exception exception)
      {
        _logger.Error(string.Format("[Online] - Engine out of fuel /r/n Message: {0} Inner Exception: {1} Stack: {2}",
          exception.Message + Environment.NewLine, exception.InnerException + Environment.NewLine, exception.StackTrace + Environment.NewLine));
      }
    }

    public void Stop()
    {
      _logger.Info("[Online] - Shutting Down.");
    }
  }
}

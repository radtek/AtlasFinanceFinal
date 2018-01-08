using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.Online.Node.Core;
using Atlas.RabbitMQ.Messages;
using DevExpress.Xpo;
using Ninject;
using Ninject.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Atlas.Payout.Engine.EasyNetQ;
using EasyNetQ;

namespace Atlas.Payout.Engine
{
  public class Engine : IService
  {
    private static ILogger _logger;
	  private readonly IKernel _kernel;
	  public Dictionary<int, Service> Services;
		private IBus _bus;

		public Engine(ILogger ilogger, IKernel kernel)
    {
      _logger = ilogger;
	    _kernel = kernel;
	    Services = new Dictionary<int, Service>();
    }

    public void Start()
    {
      _logger.Info("Engine is spooling up, performing warm up procedures...");
			var atlasServiceBus = _kernel.Get<AtlasServiceBus>();
			_bus = atlasServiceBus.GetServiceBus();

			var messageHandler = new Handlers.PayoutMessageHandler();

			_bus.Subscribe<InitiatePayoutMessage>("queuein_InitiatePayoutMessage", messageHandler.HandleMessage);

      List<PYT_ServiceDTO> servicesDto;

      using (var uow = new UnitOfWork())
      {
        var services = new XPQuery<PYT_Service>(uow).Where(s => s.Enabled).ToList();
        servicesDto = AutoMapper.Mapper.Map<List<PYT_Service>, List<PYT_ServiceDTO>>(services);
      }

      foreach (var service in servicesDto)
      {
        Services.Add(service.ServiceId, new Service(service, _logger));
      }

      _logger.Info("Engine is spooling up, finished preparing services...");

      _logger.Info("Starting services...");

      foreach (var service in Services)
      {
        _logger.Info($"Starting service {service.Value.ServiceName}...");

        service.Value.Start();

        _logger.Info($"Started service {service.Value.ServiceName}...");
      }

      _logger.Info("All services Started");

      var watcher = new FileWatchers.PayoutFileWatcher(_kernel);
      watcher.StartWatching(_logger);
    }

    public void Stop()
    {
    }
  }
}

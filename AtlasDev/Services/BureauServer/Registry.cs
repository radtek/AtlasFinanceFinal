using MassTransit;
using Ninject;
using Ninject.Modules;
using System;
using System.Configuration;
using Atlas.Bureau.Service.EasyNetQ;
using Atlas.RabbitMQ.Messages.Credit;
using Atlas.RabbitMQ.Messages.Fraud;

namespace Atlas.Bureau.Service
{
  public class BureauServerRegistry : NinjectModule
  {
	  public override void Load()
	  {
		  Bind<Engine>().To<Engine>().InSingletonScope();

		  var atlasServiceBus = new AtlasServiceBus();
		  atlasServiceBus.Start();
		  Bind<AtlasServiceBus>().ToConstant(atlasServiceBus);

			atlasServiceBus.GetServiceBus().Subscribe<CreditResponse>("queue_CreditResponse", Engine.AddMsg);
		  atlasServiceBus.GetServiceBus().Subscribe<ResponseBatchX>("queue_ResponseBatchX", Engine.AddMsg);
			atlasServiceBus.GetServiceBus().Subscribe<FraudResponse>("queue_FraudResponse", Engine.AddMsg);
			atlasServiceBus.GetServiceBus().Subscribe<ReportResponse>("queue_ReportResponse", Engine.AddMsg);
		}

	  protected virtual INinjectSettings CreateSettings()
    {
      var settings = new NinjectSettings();
      settings.LoadExtensions = false;
      return settings;
    }
  }
}
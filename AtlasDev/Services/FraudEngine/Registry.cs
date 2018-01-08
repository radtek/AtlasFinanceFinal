namespace Atlas.Fraud.Engine
{
	using Atlas.RabbitMQ.Messages.Fraud;
	using EasyNetQ;
	using Ninject;
	using Ninject.Modules;
	using System;
	using System.Configuration;

	public class FraudEngineRegistry : NinjectModule
  {
		public override void Load()
		{
			Bind<Engine>().To<Engine>().InSingletonScope();

			var atlasServiceBus = new AtlasServiceBus();
			atlasServiceBus.Start();
			Bind<AtlasServiceBus>().ToConstant(atlasServiceBus);

			atlasServiceBus.GetServiceBus().Subscribe<FraudRequest>("queue_FraudRequest", a =>
			{
				new Handle().HandleReq(a, atlasServiceBus.GetServiceBus());
			});
		}

		protected virtual INinjectSettings CreateSettings()
    {
      var settings = new NinjectSettings();
      settings.LoadExtensions = false;
      return settings;
    }
  }
}

using System;
using System.Configuration;
using Atlas.Credit.Engine.EasyNetQ;
using Ninject.Modules;
using Serilog;
using Ninject;

namespace Atlas.Credit.Engine
{
	public class CreditEngineRegistry : NinjectModule
	{
		public override void Load()
		{
			Bind<Engine>().To<Engine>().InSingletonScope();

			var _logger = Log.Logger.ForContext<Engine>();

			var atlasServiceBus = new AtlasServiceBus(_logger);
			atlasServiceBus.Start();
			Bind<AtlasServiceBus>().ToConstant(atlasServiceBus);
		}

		protected virtual INinjectSettings CreateSettings()
		{
			var settings = new NinjectSettings { LoadExtensions = false };
			return settings;
		}
	}
}

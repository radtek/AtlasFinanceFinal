using Ninject.Modules;
using Ninject;
using Atlas.Payout.Engine.EasyNetQ;

namespace Atlas.Payout.Engine
{
	public class PayoutEngineRegistry : NinjectModule
	{
		public override void Load()
		{
			Bind<Engine>().To<Engine>().InSingletonScope();

			var atlasServiceBus = new AtlasServiceBus();
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

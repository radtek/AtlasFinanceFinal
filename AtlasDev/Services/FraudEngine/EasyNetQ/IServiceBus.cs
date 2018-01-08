using EasyNetQ;

namespace Atlas.Fraud.Engine.EasyNetQ
{
	interface IServiceBus
	{
		void Start();
		void Stop();

		IBus GetServiceBus();
	}
}

using EasyNetQ;

namespace Atlas.Payout.Engine.EasyNetQ
{
	public interface IServiceBus
	{
		void Start();
		void Stop();

		IBus GetServiceBus();
	}
}

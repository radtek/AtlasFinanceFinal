using Atlas.Fraud.Engine.EasyNetQ;
using Atlas.RabbitMQ.Messages.Fraud;
using Atlas.ThirdParty.Fraud;
using EasyNetQ;
using Ninject;

namespace Atlas.Fraud.Engine
{
	public sealed class Handle
	{
		public Handle()
		{
		}
		
		public void HandleReq(FraudRequest req, IBus bus)
		{
			using (var fraud = new Functions(bus))
			{
				fraud.Do(req);
			}
		}
	}
}

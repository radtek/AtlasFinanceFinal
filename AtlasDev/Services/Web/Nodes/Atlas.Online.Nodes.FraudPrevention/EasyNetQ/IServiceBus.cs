using EasyNetQ;

namespace Atlas.Online.Node.FraudPrevention.EasyNetQ
{
    public interface IServiceBus
    {
        void Start();
        void Stop();

        IBus GetServiceBus();
    }
}
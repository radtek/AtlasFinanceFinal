using EasyNetQ;

namespace Atlas.Online.Node.CreditNode.EasyNetQ
{
    public interface IServiceBus
    {
        void Start();
        void Stop();

        IBus GetServiceBus();
    }
}
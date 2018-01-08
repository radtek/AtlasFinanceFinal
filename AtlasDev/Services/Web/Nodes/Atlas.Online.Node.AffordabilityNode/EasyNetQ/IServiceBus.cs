using EasyNetQ;

namespace Atlas.Online.Node.AffordabilityNode.EasyNetQ
{
    public interface IServiceBus
    {
        void Start();
        void Stop();

        IBus GetServiceBus();
    }
}
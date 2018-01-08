using EasyNetQ;

namespace Atlas.Online.Node.ClientNode.EasyNetQ
{
    public interface IServiceBus
    {
        void Start();
        void Stop();

        IBus GetServiceBus();
    }
}
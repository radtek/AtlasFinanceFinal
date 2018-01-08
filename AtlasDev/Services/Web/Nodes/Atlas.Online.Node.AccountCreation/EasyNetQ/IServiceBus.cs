using EasyNetQ;

namespace Atlas.Online.Node.AccountCreation.EasyNetQ
{
    public interface IServiceBus
    {
        void Start();
        void Stop();

        IBus GetServiceBus();
    }
}
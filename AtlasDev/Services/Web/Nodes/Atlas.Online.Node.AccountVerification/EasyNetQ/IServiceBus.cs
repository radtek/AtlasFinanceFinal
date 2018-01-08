using EasyNetQ;

namespace Atlas.Online.Node.AccountVerificationNode.EasyNetQ
{
    public interface IServiceBus
    {
        void Start();
        void Stop();

        IBus GetServiceBus();
    }
}
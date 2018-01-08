using EasyNetQ;

namespace Atlas.Online.Web.Service.EasyNetQ
{
    public interface IServiceBus
    {
        void Start();
        void Stop();

        IBus GetServiceBus();
    }
}
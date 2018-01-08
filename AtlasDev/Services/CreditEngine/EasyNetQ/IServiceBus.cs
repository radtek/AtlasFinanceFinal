using EasyNetQ;

namespace Atlas.Credit.Engine.EasyNetQ
{
  public interface IServiceBus
  {
    void Start();
    void Stop();

    IBus GetServiceBus();
  }
}

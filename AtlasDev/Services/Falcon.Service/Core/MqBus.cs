using System;
using MassTransit;

namespace Falcon.Service.Core
{
  public static class MqBus
  {
    private static IServiceBus _bus;

    public static void Instance(IServiceBus bus)
    {
      _bus = bus;
    }

    public static IServiceBus Bus()
    {
      if (_bus == null)
        throw new Exception("Bus must be set to instance of IServiceBus");

      return _bus;
    }
  }
}

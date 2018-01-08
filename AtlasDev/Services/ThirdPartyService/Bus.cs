using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.ThirdParty.Service
{
  public static class Bus
  {
    private static IServiceBus _bus = null;

    public static void Instance(IServiceBus bus)
    {
      _bus = bus;
    }

    public static IServiceBus Instance()
    {
      if (_bus == null)
        throw new Exception("Bus must be set to instance of IServiceBus");

      return _bus;
    }
  }
}

using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Online.Web.Service.EasyNetQ;

namespace Atlas.Online.Web.Service
{
  public static class BusInstance
  {
    private static IBus _bus;
      private static IAVSServiceBus _avsServiceBus;

    public static void Instance(IBus bus, IAVSServiceBus avsServiceBus)
    {
      _bus = bus;
        _avsServiceBus = avsServiceBus;
        }

        public static IBus GetBus()
        {
            if (_bus == null)
                throw new Exception("Bus must be set to instance of IBus");

            return _bus;
        }

        public static IAVSServiceBus GetAvsServiceBus()
        {
            if (_avsServiceBus == null)
                throw new Exception("Bus must be set to instance of IAVSServiceBus");

            return _avsServiceBus;
        }
    }
}

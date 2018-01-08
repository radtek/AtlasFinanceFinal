using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Bureau.Service
{
	public static class BusStaticInstance
	{
		private static IServiceBus _bus = null;

		public static void SetBus(IServiceBus bus)
		{
			_bus = bus;
		}

		public static IServiceBus GetBus()
		{
			if (_bus == null)
				throw new Exception("Bus must be set to instance of IServiceBus");

			return _bus;
		}
	}
}

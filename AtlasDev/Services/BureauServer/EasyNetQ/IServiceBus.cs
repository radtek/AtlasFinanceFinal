using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;

namespace Atlas.Bureau.Service.EasyNetQ
{
	interface IServiceBus
	{
		void Start();
		void Stop();

		IBus GetServiceBus();
	}
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using log4net;

namespace Atlas.Bureau.Service.EasyNetQ
{
	public class AtlasServiceBus : IServiceBus
	{
		/// <summary>
		/// EasyNetQ bus
		/// </summary>
		private IBus _bus;

		private static readonly ILog _log = LogManager.GetLogger(typeof(AtlasServiceBus));

		public void Start()
		{
			_log.Info("Service starting...");

			#region Start RabbitMQ to process AVS requests

			var address = ConfigurationManager.AppSettings["rabbitmq-address"];
			var virtualHost = ConfigurationManager.AppSettings["rabbitmq-virtualhost-atlas"];
			var userName = ConfigurationManager.AppSettings["rabbitmq-username"];
			var password = ConfigurationManager.AppSettings["rabbitmq-password"];

			// FP messages do not need to be durable- they are short-lived, time-out is quick - avoid overloading
			var connectionString =
				$"host={address};virtualHost={virtualHost};username={userName};password={password};timeout=20;product=atlasbureauservice;requestedHeartbeat=120";

			_log.Info("Starting Atlas Bureau Service message bus");
			try
			{
				_bus = RabbitHutch.CreateBus(connectionString);

				_log.Info("Message bus started");
			}
			catch (Exception err)
			{
				_log.Error(err);
			}
			#endregion

			_log.Info("Service started");
		}


		public void Stop()
		{
			_log.Info("Service stopping...");

			if (_bus != null)
			{
				_log.Info("Message bus stopping...");
				_bus.Dispose();
				_bus = null;
				_log.Info("Message bus stopped");
			}

			_log.Info("Service stopped");
		}

		public IBus GetServiceBus()
		{
			return _bus;
		}
	}
}

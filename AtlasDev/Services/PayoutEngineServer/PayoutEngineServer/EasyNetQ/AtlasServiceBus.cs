using System;
using System.Configuration;
using EasyNetQ;
using log4net;

namespace Atlas.Payout.Engine.EasyNetQ
{
	public class AtlasServiceBus : IServiceBus
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(AtlasServiceBus));

		/// <summary>
		/// EasyNetQ bus
		/// </summary>
		private IBus _bus;

		public void Start()
		{
			Log.Info("Service starting...");

			#region Start RabbitMQ to process AVS requests

			var address = ConfigurationManager.AppSettings["rabbitmq-address"];
			var virtualHost = ConfigurationManager.AppSettings["rabbitmq-virtualhost-atlas"];
			var userName = ConfigurationManager.AppSettings["rabbitmq-username"];
			var password = ConfigurationManager.AppSettings["rabbitmq-password"];

			// FP messages do not need to be durable- they are short-lived, time-out is quick - avoid overloading
			var connectionString =
				$"host={address};virtualHost={virtualHost};username={userName};password={password};timeout=20;product=atlascreditengine;requestedHeartbeat=120";

			Log.Info("Starting Atlas Notification Server message bus");
			try
			{
				_bus = RabbitHutch.CreateBus(connectionString);

				Log.Info("Message bus started");
			}
			catch (Exception err)
			{
				Log.Error($"Error creating message bus {err.Message} = {err.StackTrace}");
			}
			#endregion

			Log.Info("Service started");
		}


		public void Stop()
		{
			Log.Info("Service stopping...");

			if (_bus != null)
			{
				Log.Info("Message bus stopping...");
				_bus.Dispose();
				_bus = null;
				Log.Info("Message bus stopped");
			}

			Log.Info("Service stopped");
		}

		public IBus GetServiceBus()
		{
			return _bus;
		}
	}
}

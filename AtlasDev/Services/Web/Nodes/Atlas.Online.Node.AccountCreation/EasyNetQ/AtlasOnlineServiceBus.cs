using System;
using System.Configuration;
using EasyNetQ;
using log4net;

namespace Atlas.Online.Node.AccountCreation.EasyNetQ
{
    internal class AtlasOnlineServiceBus : IServiceBus
    {
        /// <summary>
        /// EasyNetQ bus
        /// </summary>
        private IBus _bus;

        private static readonly ILog _log = LogManager.GetLogger(typeof(AtlasOnlineServiceBus));

        public void Start()
        {
            _log.Info("Service starting...");

            #region Start RabbitMQ to process AVS requests
            
            var address = ConfigurationManager.AppSettings["rabbitmq-address"];
            var virtualHost = ConfigurationManager.AppSettings["rabbitmq-virtualhost-atlas-online"];
            var userName = ConfigurationManager.AppSettings["rabbitmq-username"];
            var password = ConfigurationManager.AppSettings["rabbitmq-password"];

            // FP messages do not need to be durable- they are short-lived, time-out is quick - avoid overloading
            var connectionString = string.Format("host={0};virtualHost={1};username={2};password={3};timeout=20;product=atlasnotificationserver;requestedHeartbeat=120",
              address, virtualHost, userName, password);

            _log.Info("Starting Atlas Notification Server message bus");
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

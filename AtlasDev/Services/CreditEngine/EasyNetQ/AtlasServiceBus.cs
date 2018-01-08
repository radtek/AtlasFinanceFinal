using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using Serilog;

namespace Atlas.Credit.Engine.EasyNetQ
{
  public class AtlasServiceBus:IServiceBus
  {
    private readonly ILogger _log;

    /// <summary>
       /// EasyNetQ bus
       /// </summary>
    private IBus _bus;

    public AtlasServiceBus(ILogger log)
    {
      _log = log;
    }

    public void Start()
    {
      _log.Information("Service starting...");

      #region Start RabbitMQ to process AVS requests

      var address = ConfigurationManager.AppSettings["rabbitmq-address"];
      var virtualHost = ConfigurationManager.AppSettings["rabbitmq-virtualhost-atlas"];
      var userName = ConfigurationManager.AppSettings["rabbitmq-username"];
      var password = ConfigurationManager.AppSettings["rabbitmq-password"];

      // FP messages do not need to be durable- they are short-lived, time-out is quick - avoid overloading
      var connectionString = string.Format("host={0};virtualHost={1};username={2};password={3};timeout=20;product=atlascreditengine;requestedHeartbeat=120",
        address, virtualHost, userName, password);

      _log.Information("Starting Atlas Notification Server message bus");
      try
      {
        _bus = RabbitHutch.CreateBus(connectionString);

        _log.Information("Message bus started");
      }
      catch (Exception err)
      {
        _log.Error($"Error creating message bus {err.Message} = {err.StackTrace}");
      }
      #endregion

      _log.Information("Service started");
    }


    public void Stop()
    {
      _log.Information("Service stopping...");

      if (_bus != null)
      {
        _log.Information("Message bus stopping...");
        _bus.Dispose();
        _bus = null;
        _log.Information("Message bus stopped");
      }

      _log.Information("Service stopped");
    }

    public IBus GetServiceBus()
    {
      return _bus;
    }
  }
}

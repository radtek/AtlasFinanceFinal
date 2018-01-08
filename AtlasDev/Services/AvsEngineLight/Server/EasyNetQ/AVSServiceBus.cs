using System;
using System.Configuration;
using System.Threading.Tasks;

using EasyNetQ;

using BankVerification.EasyNetQ;
using AvsEngineLight.EasyNetQ.Consumers;

using Atlas.Common.Interface;


namespace AvsEngineLight.EasyNetQ
{
  internal interface IAVSServiceBus
  {
    void Start();
    void Stop();
  }


  /// <summary>
  /// Class to handle AVS receive bus
  /// </summary>
  internal class AVSServiceBus : AvsEngineLight.EasyNetQ.IAVSServiceBus
  {
    public AVSServiceBus(ILogging log, IConfigSettings config)
    {
      _log = log;
      _config = config;
    }


    public void Start()
    {
      _log.Information("Service starting...");

      #region Start RabbitMQ to process AVS requests

      // TODO: Use _config
      // Request/receive via RabbitMQ...
      var address = ConfigurationManager.AppSettings["rabbitmq-address"];
      var virtualHost = ConfigurationManager.AppSettings["rabbitmq-virtualhost"];
      var userName = ConfigurationManager.AppSettings["rabbitmq-username"];
      var password = ConfigurationManager.AppSettings["rabbitmq-password"];

      // FP messages do not need to be durable- they are short-lived, time-out is quick - avoid overloading
      var connectionString = string.Format("host={0};virtualHost={1};username={2};password={3};timeout=20;product=avsenginelight;requestedHeartbeat=120",
        address, virtualHost, userName, password);

      _log.Information("Starting AVS message bus");
      try
      {        
        _bus = RabbitHutch.CreateBus(connectionString);
        _bus.RespondAsync<AddAVSRequest, AVSResponse>(request => Task.Run<AVSResponse>(() => AddAVSTransactionConsumer.Handle(_log, request)));
        _bus.RespondAsync<CheckAVSRequest, AVSResponse>(request => Task.Run<AVSResponse>(() => QueryAVSMessageConsumer.Handle(_log, request)));

        _log.Information("Message bus started");
      }
      catch (Exception err) 
      {
        _log.Error(err, "CreateBus");
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


    #region Private fields

    /// <summary>
    /// EasyNetQ bus
    /// </summary>
    private IBus _bus;  
      
    // Injected
    private readonly ILogging _log;
    private readonly IConfigSettings _config;

    #endregion
  }
}

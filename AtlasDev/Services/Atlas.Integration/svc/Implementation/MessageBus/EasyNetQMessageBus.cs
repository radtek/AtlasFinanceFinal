using System;

using EasyNetQ;

using Atlas.Common.Interface;
using Atlas.NotificationENQ.Dto;


namespace Atlas.Server.Implementation.MessageBus
{
  internal class EasyNetQMessageBus : IMessageBusHandler
  {
    public EasyNetQMessageBus(IConfigSettings config, ILogging log)
    {
      _config = config;
      _log = log;

      if (_bus == null)
      {
        StartBus();
      }
    }


    /// <summary>
    /// Deliver a SMS using the message queue
    /// </summary>
    /// <param name="cellNum"></param>
    /// <param name="smsMessage"></param>
    public void SendSMS(string cellNum, string smsMessage)
    {
      var methodName = "SendSMS";

      try
      {
        if (_bus == null)
        {
          _log.Error(new NullReferenceException("SendSMS called while _bus is null"), methodName);
          StartBus();
        }

        var id = _bus.Request<SendSmsMessageRequest, SendSmsMessageResponse>(new SendSmsMessageRequest
        {
          To = cellNum,
          Body = smsMessage
        });
      }
      catch (Exception err)
      {
        _log.Error(err, methodName);
      }
    }


    ~EasyNetQMessageBus()
    {
      ShutdownBus();
    }


    #region Private methods

    /// <summary>
    /// Start the RabbitMQ bus
    /// </summary>
    private void StartBus()
    {
      var methodName = "StartBus";

      try
      {
        var address = _config.GetRabbitMQServerHost();
        var virtualHost = _config.GetRabbitMQVirtualHost();
        var userName = _config.GetRabbitMQServerUsername();
        var password = _config.GetRabbitMQServerPassword();

        _log.Information("Starting bus...");
        var connectionString = string.Format("host={0};virtualHost={1};username={2};password={3};timeout=20;requestedHeartbeat=120",
          address, virtualHost, userName, password);
        _bus = RabbitHutch.CreateBus(connectionString);
        _log.Information("Bus started");
      }
      catch (Exception err)
      {
        _log.Error(err, methodName);
      }
    }


    /// <summary>
    /// Shut-down the service bus
    /// </summary>
    private void ShutdownBus()
    {
      if (_bus != null)
      {
        _bus.Dispose();
        _bus = null;
      }
    }

    #endregion


    #region Private fields

    private readonly ILogging _log;
    private readonly IConfigSettings _config;
    private static IBus _bus;
    
    #endregion

  }
}
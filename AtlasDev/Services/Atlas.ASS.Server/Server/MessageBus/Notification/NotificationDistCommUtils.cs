using System;
using System.Threading.Tasks;

using EasyNetQ;

using Atlas.Common.Interface;
using Atlas.NotificationENQ.Dto;


namespace Atlas.Server.MessageBus.Notification
{
  //TODO: Inject/abstract
  internal static class NotificationDistCommUtils
  {   
    internal static void Start(ILogging log, IConfigSettings config)
    {
      _config = config;
      _log = log;
      _log.Information("Notification comms: Starting Bus...");
      // Request/receive via RabbitMQ...
      var address = config.GetCustomSetting(null, "rabbitmq-notification-address", false);
      var virtualHost = config.GetCustomSetting(null, "rabbitmq-notification-vhost", false);
      var userName = config.GetCustomSetting(null, "rabbitmq-notification-username", false);
      var password = config.GetCustomSetting(null, "rabbitmq-notification-password", false);

      var connectionString = string.Format("host={0};virtualHost={1};username={2};password={3};timeout=5;product=assserver;requestedHeartbeat=120",
        address, virtualHost, userName, password);
      _bus = RabbitHutch.CreateBus(connectionString);
    }


    internal static void SendSMS(string cellNumber, string message)
    {
      _log.Information("SendSMS: Starting {@Request}", new { cellNumber, message });

      try
      {
        var task = Task.Run<SendSmsMessageResponse>(async () =>
        {
          return await _bus.RequestAsync<SendSmsMessageRequest, SendSmsMessageResponse>(new SendSmsMessageRequest { Body = message, To = cellNumber });
        });

        if (task.Wait(5500))
        {
          _log.Information("SendSMS: {@response}", task.Result);
        }
        else
        {
          _log.Error("SendSMS: Timed out waiting for task");
        }      
      }
      catch (Exception err)
      {
        _log.Error(err, "SendSMS()");       
      }
    }


    internal static void Stop()
    {
      try
      {
        if (_bus != null)
        {
          _bus.Dispose();
          _bus = null;
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "Stop()");
      }
    }


    private static IConfigSettings _config;

    public static ILogging _log;

    private static IBus _bus;

    
  }
}

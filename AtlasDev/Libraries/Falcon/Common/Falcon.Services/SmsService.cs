using System;
using Atlas.RabbitMQ.Messages.Notification;
using Falcon.Common.Interfaces.Services;
using Magnum;
using MassTransit;

namespace Falcon.Common.Services
{
  public class SmsService : ISmsService
  {
    private readonly IServiceBus _bus;
    public SmsService(IServiceBus bus)
    {
      _bus = bus;
    }

    public void Send(string to, string message, Atlas.Enumerators.Notification.NotificationPriority priority)
    {
      _bus.Publish(new SMSNotifyMessage(CombGuid.Generate()) { ActionDate = DateTime.Now, Body = message, To = to, Priority = priority, Provider = Provider.SMSPORTAL });
    }

    public void Send(string[] to, string message, Atlas.Enumerators.Notification.NotificationPriority priority)
    {
      throw new NotImplementedException();
    }
  }
}

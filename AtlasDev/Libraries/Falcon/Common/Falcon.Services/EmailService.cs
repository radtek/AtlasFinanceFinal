using Falcon.Common.Interfaces.Services;
using MassTransit;
using System;

namespace Falcon.Common.Services
{
  public class EmailService : IEmailService
  {
    private readonly IServiceBus _bus;
    public EmailService(IServiceBus bus)
    {
      _bus = bus;
    }
    public void Send(string to, string message, bool isHtml, Atlas.Enumerators.Notification.NotificationPriority priority)
    {
      throw new NotImplementedException();
    }

    public void Send(string[] to, string message, bool isHtml, Atlas.Enumerators.Notification.NotificationPriority priority)
    {
      throw new NotImplementedException();
    }
  }
}

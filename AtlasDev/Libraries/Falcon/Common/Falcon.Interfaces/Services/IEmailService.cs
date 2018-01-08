using Atlas.Enumerators;

namespace Falcon.Common.Interfaces.Services
{
  public interface IEmailService
  {
    void Send(string to, string message, bool isHtml, Notification.NotificationPriority priority);
    void Send(string[] to, string message, bool isHtml, Notification.NotificationPriority priority);
  }
}

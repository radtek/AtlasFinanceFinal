using Atlas.Enumerators;

namespace Falcon.Common.Interfaces.Services
{
  public interface ISmsService
  {
    void Send(string to, string message, Notification.NotificationPriority priority);
    void Send(string[] to, string message, Notification.NotificationPriority priority);
  }
}

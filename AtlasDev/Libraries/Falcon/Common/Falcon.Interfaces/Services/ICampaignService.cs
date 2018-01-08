using Atlas.Enumerators;

namespace Falcon.Common.Interfaces.Services
{
  public interface ICampaignService
  {
    void EnqueueSms(string to, string message, Notification.NotificationPriority priority);
  }
}

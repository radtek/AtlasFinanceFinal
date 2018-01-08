using Atlas.Enumerators;
using Falcon.Common.Interfaces.Services;

namespace Falcon.Common.Services
{
  public sealed class CampaignService : ICampaignService
  {
    private readonly ISmsService _smsService;
    public CampaignService(ISmsService smsService)
    {
      _smsService = smsService;
    }
    public void EnqueueSms(string to, string message, Notification.NotificationPriority priority)
    {
      _smsService.Send(to, message, priority);
    }
  }
}

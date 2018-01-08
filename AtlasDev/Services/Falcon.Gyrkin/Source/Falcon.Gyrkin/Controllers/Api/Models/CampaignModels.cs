using System;

namespace Falcon.Gyrkin.Controllers.Api.Models
{
  public class CampaignModels
  {
    public class SMSQueryModel
    {
      public long? NotificationId { get; set; }
      public DateTime? StartDate { get; set; }
      public DateTime? EndDate { get; set; }
    }

    public class ResendModel
    {
      public long NotificationId { get; set; }
    }
  }
}

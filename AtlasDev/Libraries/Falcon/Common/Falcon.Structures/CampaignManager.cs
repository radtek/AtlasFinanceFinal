using System;
using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Structures
{
  public sealed class CampaignManager
  {
   
  }
  public sealed class SmsNotification : ISmsNotification
  {
    public long NotificationId { get; set; }
    public string To { get; set; }
    public string Body { get; set; }
    public string Status { get; set; }
    public DateTime CreateDate { get; set; }
    public long? EventId { get; set; }
    public long? ReplyId { get; set; }
    public string Reply { get; set; }
  }
}

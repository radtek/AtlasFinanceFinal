using System;

namespace Falcon.Common.Interfaces.Structures
{
  public interface ISmsNotification
  {
    long NotificationId { get; set; }
    string To { get; set; }
    string Body { get; set; }
    string Status { get; set; }
    DateTime CreateDate { get; set; }
    long? EventId { get; set; }
    long? ReplyId { get; set; }
    string Reply { get; set; }
  }
}

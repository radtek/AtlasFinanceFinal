using System;

namespace Atlas.NotificationENQ.Dto
{
  public class SendSmsMessageRequest
  {
    public string To { get; set; }
    public string Body { get; set; }
    public DateTime LastAttempt { get; set; }
    public long RecId { get; set; }
    public long UserPersonId { get; set; }
    public long RecipientPersonId { get; set; }
  }

}
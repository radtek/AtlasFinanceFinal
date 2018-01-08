using System;
using System.Collections.Generic;


namespace Atlas.NotificationENQ.Dto
{
  public class SendEmailMessageRequest
  {
    public string From { get; set; }
    public string To { get; set; }

    public string Subject { get; set; }
    public string Body { get; set; }
    public bool BodyIsHtml { get; set; }

    public List<Tuple<string, byte[]>> Attachments { get; set; }

    public DateTime LastAttempt { get; set; }
    public long RecId { get; set; }
    public long UserPersonId { get; set; }
    public long RecipientPersonId { get; set; }

  }
}

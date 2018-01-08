using System;
using System.Diagnostics;


namespace Atlas.RabbitMQ.Messages.Notification
{
  [Serializable]
  public class EventLogNotifyMessage
  {
    public EventLogNotifyMessage(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }

    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }

    public string Source { get; set; }
    public string Log { get; set; }
    public string Event { get; set; }
    public EventLogEntryType EntryType { get; set; }

  }
}

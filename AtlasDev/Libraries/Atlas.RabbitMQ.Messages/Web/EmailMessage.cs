using System;

using Atlas.Enumerators;
using System.Net.Mail;


namespace Atlas.RabbitMQ.Messages.Online
{
  public class EmailMessage : IMessage
  {
    public EmailMessage(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }

    protected EmailMessage()
    {
    }

    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? RouteHistoryId { get; set; }
    public long? MessageId { get; set; }
    public NodeType.Nodes Source { get; set; }
    public NodeType.Nodes Destination { get; set; }
    public string To { get; set; }
    public string From { get; set; }
    public string Subject { get; set; }
    public MailPriority Priority { get; set; }
    public bool IsBodyHTML { get; set; }
    public string Body { get; set; }
    public bool IsActionDateTriggered { get; set; }
    public DateTime? ActionDate { get; set; }

  }
}
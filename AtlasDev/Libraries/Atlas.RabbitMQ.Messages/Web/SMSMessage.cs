using System;

using Atlas.Enumerators;


namespace Atlas.RabbitMQ.Messages.Online
{
  public class SMSMessage : IMessage
  {
    public SMSMessage(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }

    protected SMSMessage()
    {
    }

    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? RouteHistoryId { get; set; }
    public long? MessageId { get; set; }
    public NodeType.Nodes Source { get; set; }
    public NodeType.Nodes Destination { get; set; }
    public string CellNo { get; set; }
    public string Message { get; set; }

  }
}
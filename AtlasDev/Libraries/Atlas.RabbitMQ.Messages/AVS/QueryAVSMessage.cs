using System;

namespace Atlas.RabbitMQ.Messages.AVS
{
  [Serializable]
  public class QueryAVSMessage : IMessage
  {
    public QueryAVSMessage(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }
    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? MessageId { get; set; }
    public long TransactionId { get; set; }
  }
}

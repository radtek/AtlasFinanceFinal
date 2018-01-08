using System;


namespace Atlas.RabbitMQ.Messages
{
  [Serializable]
  public class InitiatePayoutMessage : IMessage
  {
    public InitiatePayoutMessage(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }

    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? MessageId { get; set; }
    public long AccountId { get; set; }
  }
}

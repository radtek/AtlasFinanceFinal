using System;


namespace Atlas.RabbitMQ.Messages.DebitOrder
{
  public class UnbatchResponseMessage
  {
    public UnbatchResponseMessage(Guid correlationId, long? messageId = null)
    {
      CorrelationId = correlationId;
      MessageId = messageId;
      CreatedAt = DateTime.Now;
    }
    public long? MessageId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CorrelationId { get; set; }
    public long BatchId { get; set; }
    public bool SuccessfullyUnbatched { get; set; }
    public string ErrorMessage { get; set; }
  }
}

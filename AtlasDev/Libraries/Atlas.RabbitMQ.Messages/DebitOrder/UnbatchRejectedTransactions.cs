using System;


namespace Atlas.RabbitMQ.Messages.DebitOrder
{
  public class UnbatchRejectedTransactions : IMessage
  {
    public UnbatchRejectedTransactions(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.Now;
    }


    public long? MessageId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CorrelationId { get; set; }
    public long BatchId { get; set; }

  }
}

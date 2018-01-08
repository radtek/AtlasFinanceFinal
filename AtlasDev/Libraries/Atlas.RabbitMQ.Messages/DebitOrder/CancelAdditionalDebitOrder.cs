using System;


namespace Atlas.RabbitMQ.Messages.DebitOrder
{
  public class CancelAdditionalDebitOrder : IMessage
  {
    public CancelAdditionalDebitOrder(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.Now;
    }


    public long? MessageId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CorrelationId { get; set; }
    public long ControlId { get; set; }
    public long TransactionId { get; set; }

  }
}

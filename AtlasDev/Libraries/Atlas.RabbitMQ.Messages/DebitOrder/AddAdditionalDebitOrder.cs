using System;


namespace Atlas.RabbitMQ.Messages.DebitOrder
{
  public class AddAdditionalDebitOrder : IMessage
  {
    public AddAdditionalDebitOrder(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.Now;
    }

    public long? MessageId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CorrelationId { get; set; }
    public long ControlId { get; set; }
    public decimal Instalment { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime InstalmentDate { get; set; }

  }
}

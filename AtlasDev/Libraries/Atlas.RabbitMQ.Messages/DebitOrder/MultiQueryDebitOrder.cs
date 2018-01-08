using System;


namespace Atlas.RabbitMQ.Messages.DebitOrder
{
  public class MultiQueryDebitOrder : IMessage
  {
    public MultiQueryDebitOrder(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.Now;
    }

    public long? MessageId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CorrelationId { get; set; }
    public Atlas.Enumerators.General.Host? Host { get; set; }
    public long? BranchId { get; set; }
    public bool ControlOnly { get; set; }
    public DateTime? StartRange { get; set; }
    public DateTime? EndRange { get; set; }

  }
}
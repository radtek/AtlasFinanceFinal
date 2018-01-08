using System;


namespace Atlas.RabbitMQ.Messages.DebitOrder
{
  public class QueryNaedoBatch : IMessage
  {
    public QueryNaedoBatch(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.Now;
    }


    public long? MessageId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CorrelationId { get; set; }
    public long? BatchId { get; set; }
    public bool BatchOnly { get; set; }
    public DateTime? StartRange { get; set; }
    public DateTime? EndRange { get; set; }
    public Atlas.Enumerators.Debit.BatchStatus? BatchStatus { get; set; }

  }
}
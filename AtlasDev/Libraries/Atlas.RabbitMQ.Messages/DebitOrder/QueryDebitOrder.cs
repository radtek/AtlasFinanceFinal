using System;


namespace Atlas.RabbitMQ.Messages.DebitOrder
{
  public class QueryDebitOrder : IMessage
  {
    public QueryDebitOrder(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.Now;
    }


    public long? MessageId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CorrelationId { get; set; }
    public long ControlId { get; set; }
    public int? SpecifiedRepetition { get; set; } // This will allow to query only transactions with this repetition 

  }
}

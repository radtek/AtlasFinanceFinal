using System;
using System.Collections.Generic;


namespace Atlas.RabbitMQ.Messages.DebitOrder
{
  public class MultiResponseMessage
  {
    public MultiResponseMessage(Guid correlationId, long? messageId = null)
    {
      CorrelationId = correlationId;
      MessageId = messageId;
      CreatedAt = DateTime.Now;
    }


    public long? MessageId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CorrelationId { get; set; }
    public string RequestMessage { get; set; } //Json object of request message
    public bool HasError { get; set; }
    public string ErrorMessage { get; set; }
    public List<ResponseControl> Responses { get; set; }

  }
}

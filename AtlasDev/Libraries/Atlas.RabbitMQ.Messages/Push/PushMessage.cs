using System;
using System.Collections.Generic;


namespace Atlas.RabbitMQ.Messages.Push
{
  public class PushMessage 
  {
    public enum PushType
    {
      AVS,
      Payout,
      NAEDO,
      FingerPrint
    }
    public PushMessage(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
      Parameters = new Dictionary<string, object>();
    }


    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public PushType Type { get; set; }
    public Dictionary<string, object> Parameters { get; set; }

  }
}

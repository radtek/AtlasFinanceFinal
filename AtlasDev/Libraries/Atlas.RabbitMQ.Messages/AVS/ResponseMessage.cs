using System;

namespace Atlas.RabbitMQ.Messages.AVS
{
  [Serializable]
  public class ResponseMessage : IMessage
  {
    public ResponseMessage(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }
    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? MessageId { get; set; }

    public dynamic RequestMessage { get; set; }
    public bool AccountExists { get; set; }
    public bool IdNumberMatch { get; set; }
    public bool InitialsMatch { get; set; }
    public bool LastNameMatch { get; set; }
    public bool AccountOpen { get; set; }
    public bool AccountAcceptsCredits { get; set; }
    public bool AccountAcceptsDebits { get; set; }
    public bool AccountOpen90days { get; set; }
    public bool WaitingReply { get; set; }
    public long TransactionId { get; set; }
    public bool Error { get; set; }
    public Atlas.Enumerators.AVS.Result? FinalResult { get; set; }
  }
}

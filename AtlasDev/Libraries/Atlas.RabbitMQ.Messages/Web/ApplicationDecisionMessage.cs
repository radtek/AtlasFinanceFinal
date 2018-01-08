using System;

using Atlas.Enumerators;


namespace Atlas.RabbitMQ.Messages.Online
{
  public class ApplicationDecisionMessage : IMessage
  {
    public ApplicationDecisionMessage(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }


    public ApplicationDecisionMessage()
    {
    }

    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? RouteHistoryId { get; set; }
    public long? MessageId { get; set; }
    public NodeType.Nodes Source { get; set; }
    public NodeType.Nodes Destination { get; set; }
    public long AccountId { get; set; }
    public Account.AccountStatus Status { get; set; }
    public Account.AccountStatusReason Reason { get; set; }
    public Account.AccountStatusSubReason SubReason { get; set; }

  }
}
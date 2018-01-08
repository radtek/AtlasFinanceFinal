using System;
using Atlas.Enumerators;


namespace Atlas.RabbitMQ.Messages.Online
{
  #region Fraud Prevention Request Message
  [Serializable]
  public class FraudPreventionRequestMessage 
  {
    public Guid CorrelationId { get; set; }
    public long PersonId { get; set; }
  }
  #endregion


  #region Fraud Prevention Start Request Message
  public class FraudPreventionStartRequestMessage 
  {
    public Guid CorrelationId { get; set; }

    public long PersonId { get; set; }
  }

  #endregion


  #region Fraud Prevention Request Completed Message
  public class FraudPreventionCompletedRequestMessage 
  {
    public Guid CorrelationId { get; set; }

    public long PersonId { get; set; }
  }

  #endregion


  #region Redudant
  public class FraudPreventionMessage : IMessage
  {
    public FraudPreventionMessage(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }

    protected FraudPreventionMessage()
    {
    }

    public long ApplicationId { get; set; }
    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? RouteHistoryId { get; set; }
    public long? MessageId { get; set; }
    public NodeType.Nodes Source { get; set; }
    public NodeType.Nodes Destination { get; set; }
    public int RetryCount { get; set; }
    public long PersonId { get; set; }
    public long? AccountId { get; set; }
    public long ClientId { get; set; }
  }

  #endregion

}
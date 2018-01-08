using System;
using Atlas.Enumerators;


namespace Atlas.RabbitMQ.Messages.Online
{
  #region Credit Request Message

  [Serializable]
  public class CreditCheckRequestMessage
  {
    public Guid CorrelationId { get; set; }
    public long PersonId { get; set; }
  }

  #endregion


  #region Credit Start Request Message

  [Serializable]
  public class CreditCheckStartRequestMessage 
  {
    public Guid CorrelationId { get; set; }
    public long PersonId { get; set; }
  }

  #endregion


  #region Credit Completed Message

  [Serializable]
  public class CreditCheckedCompletedRequestMessage 
  {
    public Guid CorrelationId { get; set; }
    public long PersonId { get; set; }
    public long AccountId { get; set; }
  }


  #endregion


  #region Redundant
  public class CreditMessage : IMessage
  {
    public CreditMessage(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }

    protected CreditMessage()
    {
    }

    public long AccountId { get; set; }
    public long ApplicationId { get; set; }
    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public long ClientId { get; set; }
    public long? RouteHistoryId { get; set; }
    public long? MessageId { get; set; }
    public NodeType.Nodes Source { get; set; }
    public NodeType.Nodes Destination { get; set; }
  }

  #endregion

}
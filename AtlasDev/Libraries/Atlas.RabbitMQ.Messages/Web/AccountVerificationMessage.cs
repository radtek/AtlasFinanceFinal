using System;
using Atlas.Enumerators;


namespace Atlas.RabbitMQ.Messages.Online
{
  #region Account Verification Request Message

  [Serializable]
  public class AccountVerificationRequestMessage 
  {
    public Guid CorrelationId { get; set; }
    public long PersonId { get; set; }
  }

  #endregion


  #region Account Verification Start Request Message

  [Serializable]
  public class AccountVerificationStartRequestMessage 
  {
    public Guid CorrelationId { get; set; }
    public long PersonId { get; set; }
  }

  #endregion


  #region Account Verification Completed Request Message

  [Serializable]
  public class AccountVerificationCompletedRequestMessage 
  {
    public Guid CorrelationId { get; set; }
    public long PersonId { get; set; }
  }

  #endregion


  [Serializable]
  public class AccountVerificationMessage : IMessage
  {
    public AccountVerificationMessage(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }

    public AccountVerificationMessage()
    {
    }

    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? RouteHistoryId { get; set; }
    public long? MessageId { get; set; }
    public NodeType.Nodes Source { get; set; }
    public NodeType.Nodes Destination { get; set; }
    /* Client specific properties */
    public long ClientId { get; set; }
    public long AccountId { get; set; }
  }

}
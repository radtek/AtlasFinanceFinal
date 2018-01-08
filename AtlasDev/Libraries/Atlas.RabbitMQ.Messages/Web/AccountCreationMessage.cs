using System;
using Atlas.Enumerators;


namespace Atlas.RabbitMQ.Messages.Online
{
  #region Account Creation Request Message

  [Serializable]
  public class AccountCreationRequestMessage 
  {
    public Guid CorrelationId { get; set; }
    public long PersonId { get; set; }
  }

  #endregion


  #region Account Creation Start Request Message

  [Serializable]
  public class AccountCreationStartRequestMessage 
  {
    public Guid CorrelationId { get; set; }
    public long PersonId { get; set; }
  }

  #endregion


  #region Account Creation Request Completed Message

  [Serializable]
  public class AccountCreationCompletedRequestMessage
  {
    public Guid CorrelationId { get; set; }
    public long PersonId { get; set; }
    public long AccountId { get; set; }
  }


  #endregion
  

  public class AccountCreationMessage : IMessage
  {
    public AccountCreationMessage(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }

    public AccountCreationMessage()
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
   
  }

}
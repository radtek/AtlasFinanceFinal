using System;

using Atlas.Enumerators;


namespace Atlas.RabbitMQ.Messages.Online
{
  #region Client Creation Request Message

  [Serializable]
  public class ClientCreationRequestMessage 
  {
    public Guid CorrelationId { get; set; }

    public long Id { get; set; }
  }

  #endregion


  #region Client Creation Start Request Message

  [Serializable]
  public class ClientCreationStartRequestMessage 
  {
    public Guid CorrelationId { get; set; }

    public long Id { get; set; }
  }

  #endregion


  #region Client Creation Request Completed Message

  [Serializable]
  public class ClientCreationCompletedRequestMessage 
  {
    public Guid CorrelationId { get; set; }
    public long Id { get; set; }
  }

  #endregion


  #region Redundant
  public class ClientMessage : IMessage
  {
    public ClientMessage(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }

    protected ClientMessage()
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
    public string CheckSum { get; set; }
  }

  #endregion

}
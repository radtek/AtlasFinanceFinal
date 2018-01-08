using System;
using Atlas.Enumerators;


namespace Atlas.RabbitMQ.Messages.Online
{
  #region Affordability Calculation Request Message

  [Serializable]
  public class AffordabilityCalculationRequestMessage 
  {
    public Guid CorrelationId { get; set; }
    public long PersonId { get; set; }
  }

  #endregion


  #region Affordability Calculation Start Request Message

  [Serializable]
  public class AffordabilityCalculationStartRequestMessage 
  {
    public Guid CorrelationId { get; set; }
    public long PersonId { get; set; }
  }

  #endregion


  #region Affordability Calculation Completed Request Message

  [Serializable]
  public class AffordabilityCalculationCompletedRequestMessage 
  {
    public Guid CorrelationId { get; set; }
    public long PersonId { get; set; }
  }

  #endregion


  public class AffordabilityMessage : IMessage
  {
    public AffordabilityMessage(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }

    protected AffordabilityMessage()
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

}
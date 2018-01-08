using System;

namespace Atlas.RabbitMQ.Messages.Online
{
  #region Decision Request Message

  [Serializable]
  public class DecisionRequestMessage
  {
    public Guid CorrelationId { get; set; }
    public long PersonId { get; set; }
  }

  #endregion


  #region Decision Start Request Message

  [Serializable]
  public class DecisionStartRequestMessage 
  {
    public Guid CorrelationId { get; set; }
    public long PersonId { get; set; }
  }

  #endregion


  #region Decision Request Completed Message

  [Serializable]
  public class DecisionRequestCompletedMessage 
  {
    public Guid CorrelationId { get; set; }
    public long PersonId { get; set; }
  }

  #endregion

}
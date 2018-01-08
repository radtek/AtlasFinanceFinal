using System;


namespace Atlas.RabbitMQ.Messages.Credit
{
  [Serializable]
  public sealed class ResponseBatchX
  {
    public ResponseBatchX(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }

    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? MessageId { get; set; }

    #region Properties

    public int Result { get; set; }

    #endregion

  }
}

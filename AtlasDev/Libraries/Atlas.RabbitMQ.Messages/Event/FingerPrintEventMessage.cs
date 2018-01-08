using System;

namespace Atlas.RabbitMQ.Messages.Event.FingerPrint
{
  #region Finger Print Event Message

  [Serializable]
  public class FingerPrintEventMessage 
  {
    public Guid CorrelationId { get; set; }
    public long ApplicationId { get; set; }
    public long PersonId { get; set; }
    public long BranchId { get; set; }
    public DateTime EventDate { get; set; }

    public FingerPrintEventMessage(Guid correlationId)
    {
      this.CorrelationId = correlationId;
    }
  }

  #endregion

}

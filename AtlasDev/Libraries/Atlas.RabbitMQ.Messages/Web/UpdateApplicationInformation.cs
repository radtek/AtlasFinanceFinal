using System;


namespace Atlas.RabbitMQ.Messages.Online
{
  public class UpdateApplicationInformation
  {
    public enum MessageType
    {
      PaymentDateAndAffordabilityRequest = 0,
      AffordabilityRejectionDeclineLoan = 1
    }
    public UpdateApplicationInformation(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }

    protected UpdateApplicationInformation()
    {
    }

    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }

    public long AccountId { get; set; }

    public DateTime? RepaymentDate { get; set; }

    public MessageType Type { get; set; }

  }
}
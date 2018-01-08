using System;

namespace Atlas.RabbitMQ.Messages.AVS
{
  [Serializable]
  public class IncomingResponseMessage
  {
    public IncomingResponseMessage(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }
    public IncomingResponseMessage()
    {
     
    }
    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string AccountNumber { get; set; }
    public string IdNumber { get; set; }
    public string BranchCode { get; set; }
    public string Initials { get; set; }
    public string AccountHolder { get; set; }
    public string Reference { get; set; }
    public string AccountExists { get; set; }
    public string IdNumberMatch { get; set; }
    public string InitialsMatch { get; set; }
    public string LastNameMatch { get; set; }
    public string AccountOpen { get; set; }
    public string AccountCredits { get; set; }
    public string AccountDebits { get; set; }
    public string AccountActivePeriod { get; set; }
    public string HyphenId { get; set; }
    public string ResponseMessage { get; set; }
  }
}

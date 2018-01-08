using System;
using System.Collections.Generic;
using Atlas.Enumerators;

namespace Atlas.RabbitMQ.Messages.Credit
{
  [Serializable]
  public sealed class CreditStreamResponse
  {
    public CreditStreamResponse(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }

    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? MessageId { get; set; }


    #region Properties
    public DateTime? ScoreDate { get; set; }
    public string Score { get; set; }
    public string RiskType { get; set; }
    public decimal TotalNLRAccount { get; set; }
    public decimal TotalCPAAccount { get; set; }
    public string NLREnquiryNo { get; set; }
    public Account.AccountStatus Decision { get; set; }
    public List<string> Reasons { get; set; }
    public string File { get; set; }

    public List<Product> Products { get; set; }
    public string Error { get; set; }
    // Age of the enquiry, not person.
    public int Age { get; set; }
    #endregion
  }
}

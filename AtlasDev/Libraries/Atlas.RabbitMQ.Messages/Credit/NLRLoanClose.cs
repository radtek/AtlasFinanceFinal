using System;



namespace Atlas.RabbitMQ.Messages.Credit
{
  [Serializable]
  public sealed class NLRLoanClose
  {
    public NLRLoanClose(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }

    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? MessageId { get; set; }

    #region Properties

    public string LegacyBranchNo { get; set; }
    public string SequenceNo { get; set; }
    public string NLRLoanCloseCode { get; set; }
    public string NLRLoanRegistrationNo { get; set; }
    public string CountryOfOrigin { get; set; }
    public string ReferenceNo { get; set; }
    public string TransactionCreateDate { get; set; }
    public string TransactionCreateTime { get; set; }

    #endregion

  }
}

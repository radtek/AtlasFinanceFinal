using System;


namespace Atlas.RabbitMQ.Messages.Credit
{
  [Serializable]
  public sealed class RegisterPayment
  {
    public RegisterPayment(Guid correlationId)
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
    public string IDNo { get; set; }
    public string CountryOfOrigin { get; set; }
    public string LoanReferenceNo { get; set; }
    public decimal PaymentAmount { get; set; }
    public string PaymentDate { get; set; }
    public string PaymentReferenceNo { get; set; }
    public string PaymentType { get; set; }
    public string ReferenceNo { get; set; }
    public string TransactionCreateDate { get; set; }
    public string TransactionCreateTime { get; set; }

    #endregion

  }
}

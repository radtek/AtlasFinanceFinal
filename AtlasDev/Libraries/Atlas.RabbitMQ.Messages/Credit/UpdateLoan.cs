using System;


namespace Atlas.RabbitMQ.Messages.Credit
{
  [Serializable]
  public sealed class UpdateLoan
  {
    public UpdateLoan(Guid correlationId)
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
    public string LoanRefNo { get; set; }
    public string LoanStatus { get; set; }
    public string CountryOfOrigin { get; set; }
    public string IDNo { get; set; }
    public string ReferenceNo { get; set; }
    public string Comments { get; set; }
    public string TransactionCreateDate { get; set; }
    public string TransactionCreateTime { get; set; }

    #endregion

  }
}

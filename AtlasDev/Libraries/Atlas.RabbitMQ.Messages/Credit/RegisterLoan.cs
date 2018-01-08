using System;


namespace Atlas.RabbitMQ.Messages.Credit
{
  [Serializable]
  public sealed class RegisterLoan
  {
    public RegisterLoan(Guid correlationId)
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
    public string DueDate { get; set; }
    public decimal InstallmentAmount { get; set; }
    public string IssueDate { get; set; }
    public string LoanReferenceNo { get; set; }
    public decimal TotalAmountRepayable { get; set; }
    public string ReferenceNo { get; set; }
    public string Comment { get; set; }
    public string TransactionCreateDate { get; set; }
    public string TransactionCreateTime { get; set; }

    #endregion

  }
}

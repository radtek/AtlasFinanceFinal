using System;


namespace Atlas.RabbitMQ.Messages.Credit
{
  [Serializable]
  public sealed class RegisterTelephone
  {
    public RegisterTelephone(Guid correlationId)
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
    public string TelephoneNo { get; set; }
    public string TelephoneType { get; set; }
    public string IDNo { get; set; }
    public string CountryOfOrigin { get; set; }
    public string ReferenceNo { get; set; }
    public string TransactionCreateDate { get; set; }
    public string TransactionCreateTime { get; set; }

    #endregion

  }
}

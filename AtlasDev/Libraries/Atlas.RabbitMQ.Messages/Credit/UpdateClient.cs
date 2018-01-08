using System;


namespace Atlas.RabbitMQ.Messages.Credit
{
  [Serializable]
  public sealed class UpdateClient
  {
    public UpdateClient(Guid correlationId)
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
    public string ClientNo { get; set; }
    public string Name { get; set; }
    public string Lastname { get; set; }
    public string CountryOfOrigin { get; set; }
    public string IDNo { get; set; }
    public string ReferenceNo { get; set; }
    public string Comment { get; set; }
    public string TransactionCreateDate { get; set; }
    public string TransactionCreateTime { get; set; }

    public string ClientStatus { get; set; }

    #endregion

  }
}

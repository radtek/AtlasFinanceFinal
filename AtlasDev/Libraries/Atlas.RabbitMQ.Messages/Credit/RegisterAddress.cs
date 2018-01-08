using System;


namespace Atlas.RabbitMQ.Messages.Credit
{
  [Serializable]
  public sealed class RegisterAddress
  {
    public RegisterAddress(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }

    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? MessageId { get; set; }


    #region Properties

    public string SequenceNo { get; set; }
    public string LegacyBranchNo { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string AddressLine3 { get; set; }
    public string AddressLine4 { get; set; }
    public string AddressPostalCode { get; set; }
    public string AddressType { get; set; }
    public string CountryOfOrigin { get; set; }
    public string IDNo { get; set; }
    public string ReferenceNo { get; set; }
    public string TransactionCreateDate { get; set; }
    public string TransactionCreateTime { get; set; }

    #endregion

  }
}

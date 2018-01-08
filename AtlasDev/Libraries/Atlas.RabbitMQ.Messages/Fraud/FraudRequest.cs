using System;


namespace Atlas.RabbitMQ.Messages.Fraud
{
  [Serializable]
  public sealed class FraudRequest : IMessage
  {
    public FraudRequest(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }

    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? MessageId { get; set; }


    #region Properties

    public long AccountId { get; set; }
    public string Firstname { get; set; }
    public string Surname { get; set; }
    public string IDNumber { get; set; }
    public string Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string Suburb { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Province { get; set; }
    public string HomeTelCode { get; set; }
    public string HomeTelNo { get; set; }
    public string WorkTelCode { get; set; }
    public string WorkTelNo { get; set; }
    public string CellNo { get; set; }
    public long RequestUser { get; set; }
    public string Employer { get; set; }
    public string BankAccountNo { get; set; }
    public string BankName { get; set; }
    public string BankBranchCode { get; set; }

    #endregion

  }
}

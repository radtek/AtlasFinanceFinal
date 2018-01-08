using System;
using System.Collections.Generic;


namespace Atlas.RabbitMQ.Messages.Fraud
{
  [Serializable]
  public sealed class FraudResponse
  {
    public FraudResponse(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }


    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? MessageId { get; set; }


    #region Properties

    public long FraudScoreId { get; set; }
    public int Rating { get; set; }
    public List<string> ReasonCodes { get; set; }
    public int EnquiryStatus { get; set; }
    public Enumerators.Account.AccountStatus Status { get; set; }
    public Enumerators.Account.AccountStatusReason StatusReason { get; set; }
    public Enumerators.Account.AccountStatusSubReason SubStatusReason { get; set; }
  }

    #endregion

}
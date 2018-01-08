using System;


namespace Atlas.RabbitMQ.Messages.Credit
{
  [Serializable]
  public sealed class ReportResponse
  {
    public ReportResponse(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }

    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }


    #region Properties

    public long EnquiryId { get; set; }
    public string Report { get; set; }

    #endregion

  }
}

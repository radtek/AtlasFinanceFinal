using System;


namespace Atlas.RabbitMQ.Messages.Credit
{
  [Serializable]
  public sealed class ENQGlobal
  {
    public ENQGlobal(Guid correlationId)
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
    public string ReferenceNo { get; set; }
    public string TransactionCreateDate { get; set; }
    public string TransactionCreateTime { get; set; }
    public string CountryOfOrigin { get; set; }
    public string CcEnquiry { get; set; }   
    public string NlrEnquiry { get; set; }
    public string ConEnquiry { get; set; }
    public string IdentityNo { get; set; }
    public string Surname { get; set; }
    public string Forename1 { get; set; }
    public string Forename2 { get; set; }
    public string Forename3 { get; set; }
    public string Gender { get; set; }
    public string PassportFlag { get; set; }
    public string ConDateOfBirth { get; set; }
    public string ConCurrAdd1 { get; set; }
    public string ConCurrAdd2 { get; set; }
    public string ConCurrAdd3 { get; set; }
    public string ConCurrAdd4 { get; set; }
    public string ConCurrAddPostCode { get; set; }
    public string ConHomeTelCode { get; set; }
    public string ConHomeTelNo { get; set; }
    public string ConWorkTelCode { get; set; }
    public string ConWorkTelNo { get; set; }
    public string ConCellTelNo { get; set; }
    public string NlrLoanAmount { get; set; }

    #endregion

  }
}

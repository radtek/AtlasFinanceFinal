using System;


namespace Atlas.RabbitMQ.Messages.Credit
{
  [Serializable]
  public sealed class RegisterNLRLoan2
  {
    public RegisterNLRLoan2(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }

    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? MessageId { get; set; }

    #region Properties

    public string IdentityNo { get; set; }
    public string LegacyBranchNo { get; set; }
    public string SequenceNo { get; set; }
    public string AccountNo { get;set;}
    public decimal AnnaulRateForTotalChargeOfCredit { get;set;}
    public string CountryOfOrigin { get;set;}
    public decimal CurrentBalance { get;set;}
    public string CurrentBalanceIndicator { get;set;}
    public string LoanDisbursed { get; set; }
    public string InterestRateType { get;set;}
    public decimal LoanAmount { get;set;}
    public string LoanAmountIndicator { get;set;}
    public string LoanPurpose { get;set;}
    public string LoanType { get;set;}
    public decimal MonthlyInstalment { get;set;}
    public string NLREnquiryReferenceNo { get;set;}
    public string NLRLoanRegistrationNo { get;set;}
    public decimal InterestCharges { get;set;}
    public decimal TotalChargeOfCredit { get;set;}
    public string RepaymentPeriod { get;set;}
    public decimal SettlementAmount { get;set;}
    public string SubAccountNo { get;set;}
    public decimal TotalAmountRepayable { get;set;}
    public string ReferenceNo { get;set;}
    public string TransactionCreateDate { get; set; }
    public string TransactionCreateTime { get; set; }

    #endregion

  }
}

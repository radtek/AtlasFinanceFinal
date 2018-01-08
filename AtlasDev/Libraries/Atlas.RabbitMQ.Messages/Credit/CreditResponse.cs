using System;
using System.Collections.Generic;

using Atlas.Enumerators;


namespace Atlas.RabbitMQ.Messages.Credit
{
  [Serializable]
  public sealed class CreditResponse
  {
    public CreditResponse(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }

    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? MessageId { get; set; }


    #region Properties
    public DateTime? ScoreDate { get; set; }
    public string Score { get; set; }
    public string RiskType { get; set; }
    public decimal TotalNLRAccount { get; set; }
    public decimal TotalCPAAccount { get; set; }
    public string NLREnquiryNo { get; set; }
    public Account.AccountStatus Decision { get; set; }
    public List<string> Reasons { get; set; }
    public string File { get; set; }

    public List<Product> Products { get; set; }
    public string Error { get; set; }
    public int Age { get; set; }
    public List<NLRCPAAccount> NLRCPAAccounts { get; set; }
    #endregion
  }

  public sealed class Reason
  {
    public string Description { get; set; }
  }

  public sealed class Product
  {
    public string Description { get; set; }
    public string Outcome { get; set; }
    public List<Reason> Reasons { get; set; }
  }

  public sealed class NLRCPAAccount
  {
    public Enumerators.Risk.BureauAccountType AccountType { get; set; }
    public string Subscriber { get; set; }
    public string AccountNo { get; set; }
    public string SubAccountNo { get; set; }
    public string OwnershipTypeCode { get; set; }
    public int? JoinLoanParticpants { get; set; }
    public string AccountTypeCode { get; set; }
    public string StatusCode { get; set; }
    public string Status { get; set; }
    public DateTime? StatusDate { get; set; }
    public DateTime? OpenDate { get; set; }
    public string OpenBalance { get; set; }
    public string CreditLimit { get; set; }
    public string HighLimit { get; set; }
    public string CurrentBalance { get; set; }
    public DateTime? BalanceDate { get; set; }
    public string LastPayment { get; set; }
    public DateTime? LastPaymentDate { get; set; }
    public DateTime? LastUpdateDate { get; set; }
    public string Installment { get; set; }
    public string PaymentStatus { get; set; }
    public string OverdueAmount { get; set; }
    public string BureauFlag { get; set; }
    public string LoanIndicator { get; set; }
    public string LoanType { get; set; }
    public string EndUseCode { get; set; }
    public DateTime? SettlementDate { get; set; }
    public string InterestRateType { get; set; }
    public string RepaymentPeriodType { get; set; }
    public string RepaymentPeriod { get; set; }
    public string DataSubmissionCategory { get; set; }
  }
}


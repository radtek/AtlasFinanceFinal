using System;


namespace Atlas.ThirdParty.CS.Enquiry
{
  [Serializable]
  public class Account
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
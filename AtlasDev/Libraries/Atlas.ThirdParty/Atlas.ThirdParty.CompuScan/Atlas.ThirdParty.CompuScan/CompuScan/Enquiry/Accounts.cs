using System;
using System.Collections.Generic;

namespace Atlas.ThirdParty.CompuScan.Enquiry
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


  [Serializable]
  public class Telephone
  {
    public string Type { get; set; }
    public string Number { get; set; }
    public DateTime? CreateDate { get; set; }
  }

  #region Fraud Prevention

  #region Hawk Alerts

  public class HawkAlerts
  {
    public string HawkNo { get; set; }
    public string HawkCode { get; set; }
    public string HawkDesc { get; set; }
    public string HawkFound { get; set; }
    public string DeceasedDate { get; set; }
    public string SubscriberName { get; set; }
    public string SubscriberRef { get; set; }
    public string ContactName { get; set; }
    public string ContactTelCode { get; set; }
    public string ContactTelNo { get; set; }
    public string VictimReference { get; set; }
    public string VictimTelCode { get; set; }
    public string VictimTelNo { get; set; }
  }

  #endregion

  #region Hawk IDV

  public class HawkIDV
  {
    public string IDVerifiedCode { get; set; }
    public string IDVerifiedDesc { get; set; }
    public string IDWarning { get; set; }
    public string IDDesc { get; set; }
    public string VerifiedSurname { get; set; }
    public string VerifiedForename1 { get; set; }
    public string VerifiedForename2 { get; set; }
    public string DeceasedDate { get; set; }
  }
  #endregion

  #region Fraud Score

  public class FraudScore
  {
    public string RecordSequence { get; set; }
    public string Part { get; set; }
    public string PartSequence { get; set; }
    public string ConsumerNo { get; set; }
    public string Rating { get; set; }
    public string RatingDescription { get; set; }
    public List<string> ReasonCode { get; set; }
    public List<string> ReasonDescription { get; set; }
  }

  #endregion

  #region Address Verification

  public class AddressVerfication
  {
    public string Last24Hours { get; set; }
    public string Last48Hours { get; set; }
    public string Last96Hours { get; set; }
    public string Last30Days { get; set; }
    public string AddressMessage { get; set; }
  }
  #endregion

  #region Consumer Number Frequency

  public class ConsumerNumberFrequency
  {
    public string ConsumerNo { get; set; }
    public string TelephoneCode { get; set; }
    public string TelephoneNo { get; set; }
    public string TelephoneTotal24Hrs { get; set; }
    public string TelephoneTotal48Hrs { get; set; }
    public string TelephoneTotal96Hrs { get; set; }
    public string TelephoneTotal30Days { get; set; }
    public string CellNo { get; set; }
    public string CellNoTotal24Hrs { get; set; }
    public string CellNoTotal48Hrs { get; set; }
    public string CellNoTotal96Hrs { get; set; }
    public string CellNoTotal30Days { get; set; }
  }
  #endregion

  #region Consumer CellPhone Validation

  public class ConsumerCellPhoneValidation
  {
    public string ConsumerNo { get; set; }
    public string CellNo { get; set; }
    public string CellVerificationDesc { get; set; }
    public string CellDateFirstUsed { get; set; }
  }
  #endregion

  #endregion

}

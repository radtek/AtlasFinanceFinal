using System;
using System.ComponentModel;

namespace Atlas.Enumerators
{
  public class GeneralLedger
  {
    public enum TransactionType
    {
      [Description("Loan Disbursement")]
      LoanDisbursement = 1,
      [Description("Default Admin Fee")]
      DefaultAdminFee = 2,
      [Description("Initiation Fee")]
      InitiationFee = 3,
      [Description("Service Fee")]
      ServiceFee = 4,
      [Description("Insurance Premium")]
      InsurancePremium = 5,
      [Description("NAEDO Debit Order")]
      NAEDODebitOrder = 6,
      [Description("EFT Payment")]
      EFTPayment = 7,
      [Description("Interest")]
      Interest = 8,
      [Description("NAEDO Debit Order Reversal")]
      NAEDODebitOrderReversal = 9,
      [Description("NAEDO Debit Order Unsuccessful")]
      NAEDODebitOrderUnsuccessful = 10,
      [Description("EFT Payment Reversal")]
      EFTPaymentReversal = 11,
      [Description("Refund")]
      Refund = 12,
      [Description("Debit Adjustment")]
      DebitAdjustment = 13,
      [Description("Credit Adjustment")]
      CreditAdjustment = 14,
      [Description("Settlement")]
      Settlement = 15,
      [Description("Inter-Account Settlement")]
      InterAccountSettlement = 16
    }

    public enum TransactionTypeGroup
    {
      [Description("Disbursement")]
      Disbursement = 1,
      [Description("Fee")]
      Fee = 2,
      [Description("Payment")]
      Payment = 3,
      [Description("Interest")]
      Interest = 4,
      [Description("Debit Adjustment")]
      DebitAdjustment = 5,
      [Description("Credit Adjustment")]
      CreditAdjustment = 6,
      [Description("Other Debit")]
      OtherDebit = 7,
      [Description("Other Credit")]
      OtherCredit = 8,
      [Description("Reversal/Unsuccessful")]
      Reversal_Unsuccessful = 9,
      [Description("Settlement")]
      Settlement = 10
    }

    public enum Type
    {
      [Description("Debit")]
      Debit = 1,
      [Description("Credit")]
      Credit = 2
    }

    public enum FeeRangeType
    {
      [Description("Amount")]
      Amount = 1,
      [Description("Period")]
      Period = 2
    }
  }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.PayoutEngine.Business.RTC.FileStructure
{
  public class MT101
  {
    [StringLength(16, MinimumLength = 1)]
    private string BlockOne { get; set; }
    [StringLength(16, MinimumLength = 1)]
    private string TransactionReferenceNumber { get; set; }
    [StringLength(11, MinimumLength = 3)]
    private string MessageIndexTotal { get; set; }
    [StringLength(34, MinimumLength = 1)]
    private string OrderingCustomerAccountNo { get; set; }
    [StringLength(35, MinimumLength = 1)]
    private string OrderingCustomerName { get; set; }
    [StringLength(35, MinimumLength = 1)]
    private string OrderingCustomerAddress1 { get; set; }
    [StringLength(35, MinimumLength = 1)]
    private string OrderingCustomerAddress2 { get; set; }
    [StringLength(35, MinimumLength = 1)]
    private string OrderingCustomerAddress3 { get; set; }
    [StringLength(35, MinimumLength = 1)]
    private string OrderingCustomerAddress4 { get; set; }
    [StringLength(6, MinimumLength = 6)]
    private string RequestExecutionDate { get; set; }
    [StringLength(16, MinimumLength = 1)]
    private string CustomerSpecifiedReference { get; set; }
    [StringLength(18, MinimumLength = 1)]
    private string Amount { get; set; }
    [StringLength(8, MinimumLength = 1)]
    private string BeneficiaryBankBicNumber { get; set; }
    [StringLength(34, MinimumLength = 1)]
    private string BeneficiaryAccountNumber { get; set; }
    [StringLength(34, MinimumLength = 1)]
    private string BeneficiaryName { get; set; }
    [StringLength(34, MinimumLength = 1)]
    private string BeneficiaryAddress1 { get; set; }
    [StringLength(34, MinimumLength = 1)]
    private string BeneficiaryAddress2 { get; set; }
    [StringLength(34, MinimumLength = 1)]
    private string BeneficiaryAddress3 { get; set; }
    [StringLength(34, MinimumLength = 1)]
    private string BeneficiaryAddress4 { get; set; }
    [StringLength(35, MinimumLength = 1)]
    private string RemittanceInformation { get; set; }
    [StringLength(3, MinimumLength = 3)]
    private string ChargeDetails { get; set; }

    public MT101(string bic, long payoutId, int messageIndex, int totalMessages,
            string accountNo, string name, string address1, string address2, string address3,
            string address4, DateTime actionDate, long transactionId, decimal amount, string beneficiaryBankBicCode, string beneficiaryUniversalCode,
            string beneficiaryName, string beneficiaryAddress1, string beneficiaryAddress2, string beneficiaryAddress3, string beneficiaryAddress4,
            string beneficiaryBankAccountNo, string loanAccountNo, string statementReference)
    {
      BlockOne = "{1:" + string.Format("F01{0}XXXXXXXXXX", bic) + "}";
      TransactionReferenceNumber = string.Format(":20:{0}", transactionId);
      MessageIndexTotal = string.Format(":28D:{0}/{1}", messageIndex, totalMessages);
      OrderingCustomerAccountNo = string.Format(":50H:/{0}", accountNo);
      OrderingCustomerName = name;
      OrderingCustomerAddress1 = address1;
      OrderingCustomerAddress2 = address2;
      OrderingCustomerAddress3 = address3;
      OrderingCustomerAddress4 = address4;
      RequestExecutionDate = string.Format(":30:{0}", actionDate.ToString("yyMMdd"));
      CustomerSpecifiedReference = string.Format(":21:{0}", payoutId);
      Amount = string.Format(":32B:ZAR{0}", string.Format("{0:0.00}", amount).Replace(".", ","));
      BeneficiaryBankBicNumber = string.Format(":57A://ZA{0}XXX",
                                               beneficiaryUniversalCode +
                                               (string.IsNullOrEmpty(beneficiaryBankBicCode)
                                                    ? string.Empty
                                                    : (Environment.NewLine + beneficiaryBankBicCode)));
      BeneficiaryAccountNumber = string.Format(":59:/{0}", beneficiaryBankAccountNo);
      BeneficiaryName = beneficiaryName;
      BeneficiaryAddress1 = beneficiaryAddress1;
      BeneficiaryAddress2 = beneficiaryAddress2;
      BeneficiaryAddress3 = beneficiaryAddress3;
      BeneficiaryAddress4 = beneficiaryAddress4;
      RemittanceInformation = string.Format(":70:{0} {1}", loanAccountNo, statementReference);
      ChargeDetails = ":71A:OUR";
    }

    public string BuildString()
    {
      string exportTransaction = BlockOne + ("{2:I101ABSAZAJJXXXXU}{3:{108:CBP}}{4:" +
                                  Environment.NewLine +
                                  TransactionReferenceNumber + Environment.NewLine +
                                  MessageIndexTotal + Environment.NewLine +
                                  OrderingCustomerAccountNo + Environment.NewLine +
                                  OrderingCustomerName + Environment.NewLine +
                                  (string.IsNullOrEmpty(OrderingCustomerAddress1) ? string.Empty : (OrderingCustomerAddress1 + Environment.NewLine)) +
                                  (string.IsNullOrEmpty(OrderingCustomerAddress2) ? string.Empty : (OrderingCustomerAddress2 + Environment.NewLine)) +
                                  (string.IsNullOrEmpty(OrderingCustomerAddress3) ? string.Empty : (OrderingCustomerAddress3 + Environment.NewLine)) +
                                  (string.IsNullOrEmpty(OrderingCustomerAddress4) ? string.Empty : (OrderingCustomerAddress4 + Environment.NewLine)) +
                                  RequestExecutionDate + Environment.NewLine +
                                  CustomerSpecifiedReference + Environment.NewLine +
                                  Amount + Environment.NewLine +
                                  BeneficiaryBankBicNumber + Environment.NewLine +
                                  BeneficiaryAccountNumber + Environment.NewLine +
                                  BeneficiaryName + Environment.NewLine +
                                  (string.IsNullOrEmpty(BeneficiaryAddress1) ? string.Empty : (BeneficiaryAddress1 + Environment.NewLine)) +
                                  (string.IsNullOrEmpty(BeneficiaryAddress2) ? string.Empty : (BeneficiaryAddress2 + Environment.NewLine)) +
                                  (string.IsNullOrEmpty(BeneficiaryAddress3) ? string.Empty : (BeneficiaryAddress3 + Environment.NewLine)) +
                                  (string.IsNullOrEmpty(BeneficiaryAddress4) ? string.Empty : (BeneficiaryAddress4 + Environment.NewLine)) +
                                  RemittanceInformation + Environment.NewLine +
                                  ChargeDetails + Environment.NewLine +
                                  "-}{5:}}");

      return exportTransaction;
    }
  }
}

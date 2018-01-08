/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    CompuScan enquiry interface
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 *     Fabian Franco-Roldan
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2012-04-12- Skeleton created
 *     2012-10-10- Updated with functionality.
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

namespace Atlas.Bureau.Service.WCF.Interface
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.ServiceModel;

  #endregion

  [ServiceContract]
  public interface IBatchServer
  {
    [OperationContract]
    void RegisterClient(string legacyBranchNum, string seqNo, string Idnumber, string clientReference, string name, string surname, string clientNo, string countryOfOrigin, string comment,
                        string createdDate, string createdTime, out string error, out int result);

    [OperationContract]
    void RegisterLoan(string legacyBranchNum, string seqNo, string clientReference, string Idnumber, string dueDate, string installAmt, string issuedDate, string loanRefNo, string totalAmtRepayble,
                     string countryOfOrigin, string comment, string createdDate, string createdTime, out string error, out int result);

    [OperationContract]
    void RegisterPayment(string legacyBranchNum, string seqNo, string Idnumber, string loanRefNo, string paymentAmt, string paymentDate, string paymentRefNo, string paymentType,
                        string countryOfOrigin, string createdDate, string createdTime, out string error, out int result);

    [OperationContract]
    void RegisterAddress(string legacyBranchNum, string seqNo, string IdNumber, string addressLine1, string addressLine2, string addressLine3, string addressLine4, string postalCode, string addressType,
                        string countryOfOrigin, string createdDate, string createdTime, out string error, out int result);

    [OperationContract]
    void RegisterTelephone(string legacyBranchNum, string seqNo, string IdNumber, string telephoneNo, string telephoneType, string countryOfOrigin, string createdDate, 
                          string createdTime, out string error, out int result);

    [OperationContract]
    void RegisterEmployer(string legacyBranchNum, string seqNo, string IdNumber, string employeeName, string employerName, string occupation, string payslipRefNo, string salaryFrequency, string employmentType,
                         string countryOfOrigin, string createdDate, string createdTime, out string error, out int result);

    [OperationContract]
    void UpdateClient(string legacyBranchNum, string seqNo, string IdNumber, string name, string surname, string clientNo, string comment, string countryOfOrigin,
                             string createdDate, string createdTime, string clientStatus, out string error, out int result);

    [OperationContract]
    void UpdateLoan(string legacyBranchNum, string seqNo, string loanRefNo, string IdNumber, string loanStatus, string comment,
                           string countryOfOrigin, string createdDate, string createdTime, out string error, out int result);

    [OperationContract]
    void NLRRegisterLoan(string legacyBranchNum, string seqNo, string accountNo, string annualRateForTotChargeOfCredit, string currentBalance, string currentBalanceIndicator,
                        string dateLoanDisbursed, string interestRateType, string loanAmount, string loanAmountIndicator, string loanPurpose, string loanType,
                        string monthlyInstalment, string nlrEnqRefNo, string nlrLoanRegNo, string randValueInterestCharges, string randValueTotChargeOfCredit,
                        string repaymentPeriod, string settlementPeriod, string subAccountNo, string totalAmountRepayable, string countryOfOrigin, string createdDate, 
                        string createdTime, out string error, out int result);

    [OperationContract]
    void NLRRegisterLoan2(string legacyBranchNum, string seqNo, string accountNo, string annualRateForTotChargeOfCredit, string currentBalance, string currentBalanceIndicator,
                        string dateLoanDisbursed, string interestRateType, string loanAmount, string loanAmountIndicator, string loanPurpose, string loanType,
                        string monthlyInstalment, string identityNo, string randValueInterestCharges, string randValueTotChargeOfCredit,
                        string repaymentPeriod, string settlementPeriod, string subAccountNo, string totalAmountRepayable, string countryOfOrigin, string createdDate,
                        string createdTime, out string error, out int result);

    [OperationContract]
    void NLRLoanClose(string legacyBranchNum, string seqNo, string nlrLoanCloseCode, string nlrLoanRegNo, string countryOfOrigin, string createdDate, 
                      string createdTime, out string error, out int result);

    [OperationContract]
    void NLRBatb2(string legacyBranchNum, string seqNo, string idNumber, string accountNo, string annualRateForTotChargeOfCredit, string balanceOverdue, string balanceOverdueIndicator,
                  string branchCode, string currentBalance, string currentBalanceIndicator, string dateAccountOpened, string dateLastPayment, string dateofBirth,
                  string employeeNo, string employerName, string employerPayslipRef, string employmentType, string endUseCode, string title, string forename1, string forename2,
                  string forename3, string surname, string gender, string homeTelCellNo, string workTelCellNo, string interestRateType, string loadIndicator, string loanAmt, string loanType,
                  string monthlyInstalment, string monthInArrears, string nlrEnqRefNo, string nlrLoanRegNo, string nonSaIdentityNumber, string oldAccountNo,
                  string oldSubAccountNo, string oldSupplierBranchCode, string oldSupplierRefNo, string openingBalanceIndicator, string ownerTenant,
                  string postalAddressLine1, string postalAddressLine2, string postalAddressLine3, string postalAddressLine4, string postalPostCode,
                  string randValueInterestCharges, string randValueTotchargeOfCredit, string repaymentPeriod, string residentialAddressLine1,
                  string residentialAddressLine2, string residentialAddressLine3, string residentialAddressLine4, string residentialPostCode, string salaryFrequency, string settlementAmt,
                  string statusCode, string statusDate, string subAccountNo, string totalAmountRepayable, string countryOfOrigin, string createdDate, string createdTime,
                  out string error, out int result);


        [OperationContract]
    void ENQGlobal(string legacyBranchNum, string seqNo, string identityNo, string countryoforigin, string ccenquiry, string conCellTelNo, string conCurrAdd1, string conCurrAdd2,
                  string conCurrAdd3, string conCurrAdd4, string conCurrAddPostCode, string conDateOfBirth, string conEnquiry, string conHomeTelCode,
                  string conHomeTelNo, string conWorkTelCode, string conWorkTelNo, string forename1, string forename2, string forename3,
                  string gender, string nlrEnquiry, string nlrLoanAmt, string passportFlag, string surname, string referenceNo,
                  string transCreatedDate, string transCreatedTime, out string error, out int result);

    [OperationContract]
    string GetSubmissionItemResult(string legacyBranchNum, string seqNo, out string error, out int result);
  } 
}

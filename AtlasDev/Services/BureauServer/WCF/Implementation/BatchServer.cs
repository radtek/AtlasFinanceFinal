using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading;

using DevExpress.Xpo;
using log4net;
using Magnum;

using Atlas.Bureau.Server.Cache;
using Atlas.Bureau.Service.EasyNetQ;
using Atlas.Bureau.Service.WCF.Interface;
using Atlas.Common.ExceptionBase;
using Atlas.Common.Utils;
using Atlas.Domain.Model;
using Atlas.Enumerators;
using Atlas.RabbitMQ.Messages.Credit;
using Atlas.ThirdParty.CompuScan.Batch;
using Atlas.ThirdParty.CompuScan.Batch.XML.Response;

namespace Atlas.Bureau.Service.WCF.Implemenation
{
  /// <summary>
  /// Implementation of CompuScan batch WCF
  /// </summary>
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class BatchServer : IBatchServer
  {
    #region Private Members

    // Log4net
    private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    #endregion

    /// <summary>
    /// Registers a client through the credit bureau
    /// </summary>
    public void RegisterClient(string legacyBranchNum, string seqNo, string Idnumber, string clientReference, string name, string surname,
                               string clientNo, string countryOfOrigin, string comment, string createdDate, string createdTime, out string error, out int result)
    {

      Guid guid = CombGuid.Generate();
      bool timedOut = false;
      error = null;
      result = -1;

      _log.Info(string.Format("RegisterClient() - CorrelationId {0} for IDNo {1}", guid, Idnumber));

			ServiceLocator.Get<AtlasServiceBus>().GetServiceBus().Publish(new RegisterClient(guid)
      {
        LegacyBranchNo = legacyBranchNum,
        SequenceNo = seqNo,
        IDNo = Idnumber,
        ReferenceNo = clientReference,
        Name = name,
        Lastname = surname,
        ClientNo = clientNo,
        CountryOfOrigin = countryOfOrigin,
        Comment = comment,
        TransactionCreateDate = createdDate,
        TransactionCreateTime = createdTime
      });

      Stopwatch timeout = new Stopwatch();
      timeout.Start();

      _log.Info(string.Format("RegisterClient() - Waiting for CorrelationId {0} for IDNo {1}", guid, Idnumber));

      while (ResponseMessageCache.CheckItem(guid) == null)
      {
        if (timeout.Elapsed.Minutes >= 2)
        {
          timedOut = true;
          timeout.Stop();
          break;
        }
        Thread.Sleep(50);
      }

      if (timedOut)
        return;

      if (ResponseMessageCache.CheckItem(guid) != null)
      {
        var msg = ResponseMessageCache.CheckItem(guid);

        _log.Info(string.Format("RegisterClient() -  CorrelationId {0}.", guid));

        ResponseMessageCache.Remove(guid);
      }
    }


    /// <summary>
    /// Register a loan on the credit bureau
    /// </summary>
    public void RegisterLoan(string legacyBranchNum, string seqNo, string clientReference, string Idnumber, string dueDate, string installAmt, string issuedDate, string loanRefNo,
                             string totalAmtRepayble, string countryOfOrigin, string comment, string createdDate, string createdTime, out string error, out int result)
    {
      Guid guid = CombGuid.Generate();
      bool timedOut = false;
      error = null;
      result = -1;

      _log.Info(string.Format("RegisterLoan() - CorrelationId {0} for IDNo {1}", guid, Idnumber));

			ServiceLocator.Get<AtlasServiceBus>().GetServiceBus().Publish(new RegisterLoan(guid)
      {
        LegacyBranchNo = legacyBranchNum,
        SequenceNo = seqNo,
        IDNo = Idnumber,
        ReferenceNo = clientReference,
        DueDate = dueDate,
        InstallmentAmount = Decimal.Parse(installAmt),
        IssueDate = issuedDate,
        LoanReferenceNo = loanRefNo,
        TotalAmountRepayable = Decimal.Parse(totalAmtRepayble),
        CountryOfOrigin = countryOfOrigin,
        Comment = comment,
        TransactionCreateDate = createdDate,
        TransactionCreateTime = createdTime
      });

      Stopwatch timeout = new Stopwatch();
      timeout.Start();

      _log.Info(string.Format("RegisterLoan() - Waiting for CorrelationId {0} for IDNo {1}", guid, Idnumber));

      while (ResponseMessageCache.CheckItem(guid) == null)
      {
        if (timeout.Elapsed.Minutes >= 2)
        {
          timedOut = true;
          timeout.Stop();
          break;
        }
        Thread.Sleep(50);
      }

      if (timedOut)
        return;

      if (ResponseMessageCache.CheckItem(guid) != null)
      {
        var msg = ResponseMessageCache.CheckItem(guid);

        _log.Info(string.Format("RegisterLoan() -  CorrelationId {0}.", guid));

        ResponseMessageCache.Remove(guid);
      }
    }

    /// <summary>
    /// Register a payment on the credit bureau
    /// </summary>
    public void RegisterPayment(string legacyBranchNum, string seqNo, string Idnumber, string loanRefNo, string paymentAmt, string paymentDate, string paymentRefNo, string paymentType,
                                string countryOfOrigin, string createdDate, string createdTime, out string error, out int result)
    {
      Guid guid = CombGuid.Generate();
      bool timedOut = false;
      error = null;
      result = -1;

      _log.Info(string.Format("RegisterPayment() - CorrelationId {0} for IDNo {1}", guid, Idnumber));

			ServiceLocator.Get<AtlasServiceBus>().GetServiceBus().Publish(new RegisterPayment(guid)
      {
        LegacyBranchNo = legacyBranchNum,
        SequenceNo = seqNo,
        IDNo = Idnumber,
        LoanReferenceNo = loanRefNo,
        PaymentAmount = Decimal.Parse(paymentAmt),
        PaymentDate = paymentDate,
        PaymentReferenceNo = paymentRefNo,
        PaymentType = paymentType,
        CountryOfOrigin = countryOfOrigin,
        TransactionCreateDate = createdDate,
        TransactionCreateTime = createdTime
      });

      Stopwatch timeout = new Stopwatch();
      timeout.Start();

      _log.Info(string.Format("RegisterPayment() - Waiting for CorrelationId {0} for IDNo {1}", guid, Idnumber));

      while (ResponseMessageCache.CheckItem(guid) == null)
      {
        if (timeout.Elapsed.Minutes >= 2)
        {
          timedOut = true;
          timeout.Stop();
          break;
        }
        Thread.Sleep(50);
      }

      if (timedOut)
        return;

      if (ResponseMessageCache.CheckItem(guid) != null)
      {
        var msg = ResponseMessageCache.CheckItem(guid);

        _log.Info(string.Format("RegisterLoan() -  CorrelationId {0}.", guid));

        ResponseMessageCache.Remove(guid);
      }
    }

    /// <summary>
    /// Register address on credit bureau
    /// </summary>
    public void RegisterAddress(string legacyBranchNum, string seqNo, string IdNumber, string addressLine1, string addressLine2, string addressLine3, string addressLine4, string postalCode,
                                string addressType, string countryOfOrigin, string createdDate, string createdTime, out string error, out int result)
    {
      Guid guid = CombGuid.Generate();
      bool timedOut = false;
      error = null;
      result = -1;

      _log.Info(string.Format("RegisterAddress() - CorrelationId {0} for IDNo {1}", guid, IdNumber));

			ServiceLocator.Get<AtlasServiceBus>().GetServiceBus().Publish(new RegisterAddress(guid)
      {
        LegacyBranchNo = legacyBranchNum,
        SequenceNo = seqNo,
        IDNo = IdNumber,
        AddressLine1 = addressLine1,
        AddressLine2 = addressLine2,
        AddressLine3 = addressLine3,
        AddressLine4 = addressLine4,
        AddressPostalCode = postalCode,
        AddressType = addressType,
        CountryOfOrigin = countryOfOrigin,
        TransactionCreateDate = createdDate,
        TransactionCreateTime = createdTime
      });

      Stopwatch timeout = new Stopwatch();
      timeout.Start();

      _log.Info(string.Format("RegisterAddress() - Waiting for CorrelationId {0} for IDNo {1}", guid, IdNumber));

      while (ResponseMessageCache.CheckItem(guid) == null)
      {
        if (timeout.Elapsed.Minutes >= 2)
        {
          timedOut = true;
          timeout.Stop();
          break;
        }
        Thread.Sleep(50);
      }

      if (timedOut)
        return;

      if (ResponseMessageCache.CheckItem(guid) != null)
      {
        var msg = ResponseMessageCache.CheckItem(guid);

        _log.Info(string.Format("RegisterAddress() -  CorrelationId {0}.", guid));

        ResponseMessageCache.Remove(guid);
      }
    }

    /// <summary>
    /// Register telphone on credit bureau
    /// </summary>
    public void RegisterTelephone(string legacyBranchNum, string seqNo, string IdNumber, string telephoneNo, string telephoneType, string countryOfOrigin, string createdDate,
                                  string createdTime, out string error, out int result)
    {
      Guid guid = CombGuid.Generate();
      bool timedOut = false;
      error = null;
      result = -1;

      _log.Info(string.Format("RegisterTelephone() - CorrelationId {0} for IDNo {1}", guid, IdNumber));

			ServiceLocator.Get<AtlasServiceBus>().GetServiceBus().Publish(new RegisterTelephone(guid)
      {
        LegacyBranchNo = legacyBranchNum,
        SequenceNo = seqNo,
        IDNo = IdNumber,
        TelephoneNo = telephoneNo,
        TelephoneType = telephoneType,
        CountryOfOrigin = countryOfOrigin,
        TransactionCreateDate = createdDate,
        TransactionCreateTime = createdTime
      });

      Stopwatch timeout = new Stopwatch();
      timeout.Start();

      _log.Info(string.Format("RegisterTelephone() - Waiting for CorrelationId {0} for IDNo {1}", guid, IdNumber));

      while (ResponseMessageCache.CheckItem(guid) == null)
      {
        if (timeout.Elapsed.Minutes >= 2)
        {
          timedOut = true;
          timeout.Stop();
          break;
        }
        Thread.Sleep(50);
      }

      if (timedOut)
        return;

      if (ResponseMessageCache.CheckItem(guid) != null)
      {
        var msg = ResponseMessageCache.CheckItem(guid);

        _log.Info(string.Format("RegisterTelephone() -  CorrelationId {0}.", guid));

        ResponseMessageCache.Remove(guid);
      }
    }


    /// <summary>
    /// Register employer on the credit bureau
    /// </summary>
    public void RegisterEmployer(string legacyBranchNum, string seqNo, string IdNumber, string employerName, string occupation, string payslipRefNo,
                                 string salaryFrequency, string employmentType, string employeeNo, string countryOfOrigin, string createdDate, string createdTime,
                                out string error, out int result)
    {
      Guid guid = CombGuid.Generate();
      bool timedOut = false;
      error = null;
      result = -1;

      _log.Info(string.Format("RegisterEmployer() - CorrelationId {0} for IDNo {1}", guid, IdNumber));

			ServiceLocator.Get<AtlasServiceBus>().GetServiceBus().Publish(new RegisterEmployer(guid)
      {
        LegacyBranchNo = legacyBranchNum,
        SequenceNo = seqNo,
        IDNo = IdNumber,
        EmployerName = employerName,
        EmployeeOccupation = occupation,
        EmployeePayslipReference = payslipRefNo,
        EmployeeSalaryFrequency = salaryFrequency,
        EmploymentType = employmentType,
        EmployeeNo = employeeNo,
        CountryOfOrigin = countryOfOrigin,
        TransactionCreateDate = createdDate,
        TransactionCreateTime = createdTime
      });

      Stopwatch timeout = new Stopwatch();
      timeout.Start();

      _log.Info(string.Format("RegisterEmployer() - Waiting for CorrelationId {0} for IDNo {1}", guid, IdNumber));

      while (ResponseMessageCache.CheckItem(guid) == null)
      {
        if (timeout.Elapsed.Minutes >= 2)
        {
          timedOut = true;
          timeout.Stop();
          break;
        }
        Thread.Sleep(50);
      }

      if (timedOut)
        return;

      if (ResponseMessageCache.CheckItem(guid) != null)
      {
        var msg = ResponseMessageCache.CheckItem(guid);

        _log.Info(string.Format("RegisterEmployer() -  CorrelationId {0}.", guid));

        ResponseMessageCache.Remove(guid);
      }
    }

    /// <summary>
    /// Update client data on credit bureau
    /// </summary>
    public void UpdateClient(string legacyBranchNum, string seqNo, string IdNumber, string name, string surname, string clientNo, string comment, string countryOfOrigin,
                             string createdDate, string createdTime, string clientStatus, out string error, out int result)
    {
      Guid guid = CombGuid.Generate();
      bool timedOut = false;
      error = null;
      result = -1;

      _log.Info(string.Format("UpdateClient() - CorrelationId {0} for IDNo {1}", guid, IdNumber));

			ServiceLocator.Get<AtlasServiceBus>().GetServiceBus().Publish(new UpdateClient(guid)
      {
        LegacyBranchNo = legacyBranchNum,
        SequenceNo = seqNo,
        IDNo = IdNumber,
        Name = name,
        Lastname = surname,
        ClientNo = clientNo,
        Comment = comment,
        CountryOfOrigin = countryOfOrigin,
        TransactionCreateDate = createdDate,
        TransactionCreateTime = createdTime,
        ClientStatus = clientStatus
      });

      Stopwatch timeout = new Stopwatch();
      timeout.Start();

      _log.Info(string.Format("UpdateClient() - Waiting for CorrelationId {0} for IDNo {1}", guid, IdNumber));

      while (ResponseMessageCache.CheckItem(guid) == null)
      {
        if (timeout.Elapsed.Minutes >= 2)
        {
          timedOut = true;
          timeout.Stop();
          break;
        }
        Thread.Sleep(50);
      }

      if (timedOut)
        return;

      if (ResponseMessageCache.CheckItem(guid) != null)
      {
        var msg = ResponseMessageCache.CheckItem(guid);

        _log.Info(string.Format("UpdateClient() -  CorrelationId {0}.", guid));

        ResponseMessageCache.Remove(guid);
      }
    }

    /// <summary>
    /// Update loan details on the credit bureau
    /// </summary>
    public void UpdateLoan(string legacyBranchNum, string seqNo, string loanRefNo, string IdNumber, string loanStatus, string comment,
                           string countryOfOrigin, string createdDate, string createdTime, out string error, out int result)
    {
      Guid guid = CombGuid.Generate();
      bool timedOut = false;
      error = null;
      result = -1;

      _log.Info(string.Format("UpdateLoan() - CorrelationId {0} for IDNo {1}", guid, IdNumber));

			ServiceLocator.Get<AtlasServiceBus>().GetServiceBus().Publish(new UpdateLoan(guid)
      {
        LegacyBranchNo = legacyBranchNum,
        SequenceNo = seqNo,
        IDNo = IdNumber,
        LoanRefNo = loanRefNo,
        LoanStatus = loanStatus,
        CountryOfOrigin = countryOfOrigin,
        TransactionCreateDate = createdDate,
        TransactionCreateTime = createdTime
      });

      Stopwatch timeout = new Stopwatch();
      timeout.Start();

      _log.Info(string.Format("UpdateLoan() - Waiting for CorrelationId {0} for IDNo {1}", guid, IdNumber));

      while (ResponseMessageCache.CheckItem(guid) == null)
      {
        if (timeout.Elapsed.Minutes >= 2)
        {
          timedOut = true;
          timeout.Stop();
          break;
        }
        Thread.Sleep(50);
      }

      if (timedOut)
        return;

      if (ResponseMessageCache.CheckItem(guid) != null)
      {
        var msg = ResponseMessageCache.CheckItem(guid);

        _log.Info(string.Format("UpdateLoan() -  CorrelationId {0}.", guid));

        ResponseMessageCache.Remove(guid);
      }
    }

    /// <summary>
    /// Register loan on the NLR 
    /// </summary>
    public void NLRRegisterLoan(string legacyBranchNum, string seqNo, string accountNo, string annualRateForTotChargeOfCredit, string currentBalance, string currentBalanceIndicator,
                                string dateLoanDisbursed, string interestRateType, string loanAmount, string loanAmountIndicator, string loanPurpose, string loanType,
                                string monthlyInstalment, string nlrEnqRefNo, string nlrLoanRegNo, string randValueInterestCharges, string randValueTotChargeOfCredit,
                                string repaymentPeriod, string settlementPeriod, string subAccountNo, string totalAmountRepayable, string countryOfOrigin, string createdDate,
                                string createdTime, out string error, out int result)
    {
      Guid guid = CombGuid.Generate();
      bool timedOut = false;
      error = null;
      result = -1;

      _log.Info(string.Format("NLRRegisterLoan() - CorrelationId {0}", guid.ToString()));

			ServiceLocator.Get<AtlasServiceBus>().GetServiceBus().Publish(new RegisterNLRLoan(guid)
      {
        LegacyBranchNo = legacyBranchNum,
        SequenceNo = seqNo,
        AccountNo = accountNo,
        AnnaulRateForTotalChargeOfCredit = Decimal.Parse(annualRateForTotChargeOfCredit),
        CurrentBalance = Decimal.Parse(currentBalance),
        CurrentBalanceIndicator = currentBalanceIndicator,
        LoanDisbursed = dateLoanDisbursed,
        InterestRateType = interestRateType,
        LoanAmount = Decimal.Parse(loanAmount),
        LoanAmountIndicator = loanAmountIndicator,
        LoanPurpose = loanPurpose,
        LoanType = loanType,
        MonthlyInstalment = Decimal.Parse(monthlyInstalment),
        NLREnquiryReferenceNo = nlrEnqRefNo,
        NLRLoanRegistrationNo = nlrLoanRegNo,
        InterestCharges = Decimal.Parse(randValueInterestCharges),
        TotalChargeOfCredit = Decimal.Parse(randValueTotChargeOfCredit),
        RepaymentPeriod = repaymentPeriod,
        SubAccountNo = subAccountNo,
        TotalAmountRepayable = Decimal.Parse(totalAmountRepayable),
        CountryOfOrigin = countryOfOrigin,
        TransactionCreateDate = createdDate,
        TransactionCreateTime = createdTime
      });

      Stopwatch timeout = new Stopwatch();
      timeout.Start();

      _log.Info(string.Format("NLRRegisterLoan() - Waiting for CorrelationId {0}", guid));

      while (ResponseMessageCache.CheckItem(guid) == null)
      {
        if (timeout.Elapsed.Minutes >= 2)
        {
          timedOut = true;
          timeout.Stop();
          break;
        }
        Thread.Sleep(50);
      }

      if (timedOut)
        return;

      if (ResponseMessageCache.CheckItem(guid) != null)
      {
        var msg = ResponseMessageCache.CheckItem(guid);

        _log.Info(string.Format("NLRRegisterLoan() -  CorrelationId {0}.", guid));

        ResponseMessageCache.Remove(guid);
      }
    }

    public void ENQGlobal(string legacyBranchNum, string seqNo, string identityNo, string countryoforigin, string ccenquiry, string conCellTelNo, string conCurrAdd1, string conCurrAdd2,
                  string conCurrAdd3, string conCurrAdd4, string conCurrAddPostCode, string conDateOfBirth, string conEnquiry, string conHomeTelCode,
                  string conHomeTelNo, string conWorkTelCode, string conWorkTelNo, string forename1, string forename2, string forename3,
                  string gender, string nlrEnquiry, string nlrLoanAmt, string passportFlag, string surname, string referenceNo,
                  string transCreatedDate, string transCreatedTime, out string error, out int result)
    {
      Guid guid = CombGuid.Generate();
      bool timedOut = false;
      error = null;
      result = -1;

      _log.Info(string.Format("ENQGlobal() - CorrelationId {0}", guid.ToString()));

			ServiceLocator.Get<AtlasServiceBus>().GetServiceBus().Publish(new ENQGlobal(guid)
      {
        LegacyBranchNo = legacyBranchNum,
        SequenceNo = seqNo,
        CcEnquiry = ccenquiry,
        ConCellTelNo = conCellTelNo,
        ConCurrAdd1 = conCurrAdd1,
        ConCurrAdd2 = conCurrAdd2,
        ConCurrAdd3 = conCurrAdd3,
        ConCurrAdd4 = conCurrAdd4,
        ConCurrAddPostCode = conCurrAddPostCode,
        ConDateOfBirth = conDateOfBirth,
        ConEnquiry = conEnquiry,
        ConHomeTelCode = conHomeTelCode,
        ConHomeTelNo = conHomeTelNo,
        ConWorkTelCode = conWorkTelCode,
        ConWorkTelNo = conWorkTelNo,
        CountryOfOrigin = countryoforigin,
        CreatedAt = DateTime.Now,
        Forename1 = forename1,
        Forename2 = forename2,
        Forename3 = forename3,
        Gender = gender,
        IdentityNo = identityNo,
        NlrEnquiry = nlrEnquiry,
        NlrLoanAmount = nlrLoanAmt,
        PassportFlag = passportFlag,
        ReferenceNo = referenceNo,
        Surname = surname,
        TransactionCreateDate = transCreatedDate,
        TransactionCreateTime = transCreatedTime
      });

      Stopwatch timeout = new Stopwatch();
      timeout.Start();

      _log.Info(string.Format("ENQGlobal() - Waiting for CorrelationId {0}", guid));

      while (ResponseMessageCache.CheckItem(guid) == null)
      {
        if (timeout.Elapsed.Minutes >= 2)
        {
          timedOut = true;
          timeout.Stop();
          break;
        }
        Thread.Sleep(50);
      }

      if (timedOut)
        return;

      if (ResponseMessageCache.CheckItem(guid) != null)
      {
        var msg = ResponseMessageCache.CheckItem(guid);

        _log.Info(string.Format("ENQGlobal() -  CorrelationId {0}.", guid));

        ResponseMessageCache.Remove(guid);
      }

    }

    /// <summary>
    /// Register loan on the NLR 2
    /// </summary>
    public void NLRRegisterLoan2(string legacyBranchNum, string seqNo, string accountNo, string annualRateForTotChargeOfCredit, string currentBalance, string currentBalanceIndicator,
                                string dateLoanDisbursed, string interestRateType, string loanAmount, string loanAmountIndicator, string loanPurpose, string loanType,
                                string monthlyInstalment, string IdentityNo, string randValueInterestCharges, string randValueTotChargeOfCredit,
                                string repaymentPeriod, string settlementPeriod, string subAccountNo, string totalAmountRepayable, string countryOfOrigin, string createdDate,
                                string createdTime, out string error, out int result)
    {
      Guid guid = CombGuid.Generate();
      bool timedOut = false;
      error = null;
      result = -1;

      _log.Info(string.Format("NLRRegisterLoan2() - CorrelationId {0}", guid.ToString()));

			ServiceLocator.Get<AtlasServiceBus>().GetServiceBus().Publish(new RegisterNLRLoan2(guid)
      {
        LegacyBranchNo = legacyBranchNum,
        SequenceNo = seqNo,
        AccountNo = accountNo,
        AnnaulRateForTotalChargeOfCredit = Decimal.Parse(annualRateForTotChargeOfCredit),
        CurrentBalance = Decimal.Parse(currentBalance),
        CurrentBalanceIndicator = currentBalanceIndicator,
        LoanDisbursed = dateLoanDisbursed,
        InterestRateType = interestRateType,
        LoanAmount = Decimal.Parse(loanAmount),
        LoanAmountIndicator = loanAmountIndicator,
        LoanPurpose = loanPurpose,
        LoanType = loanType,
        MonthlyInstalment = Decimal.Parse(monthlyInstalment),
        IdentityNo = IdentityNo,
        InterestCharges = Decimal.Parse(randValueInterestCharges),
        TotalChargeOfCredit = Decimal.Parse(randValueTotChargeOfCredit),
        RepaymentPeriod = repaymentPeriod,
        SubAccountNo = subAccountNo,
        TotalAmountRepayable = Decimal.Parse(totalAmountRepayable),
        CountryOfOrigin = countryOfOrigin,
        TransactionCreateDate = createdDate,
        TransactionCreateTime = createdTime
      });

      Stopwatch timeout = new Stopwatch();
      timeout.Start();

      _log.Info(string.Format("NLRRegisterLoan2() - Waiting for CorrelationId {0}", guid));

      while (ResponseMessageCache.CheckItem(guid) == null)
      {
        if (timeout.Elapsed.Minutes >= 2)
        {
          timedOut = true;
          timeout.Stop();
          break;
        }
        Thread.Sleep(50);
      }

      if (timedOut)
        return;

      if (ResponseMessageCache.CheckItem(guid) != null)
      {
        var msg = ResponseMessageCache.CheckItem(guid);

        _log.Info(string.Format("NLRRegisterLoan2() -  CorrelationId {0}.", guid));

        ResponseMessageCache.Remove(guid);
      }
    }


    /// <summary>
    /// Close a loan on the NLR
    /// </summary>
    public void NLRLoanClose(string legacyBranchNum, string seqNo, string nlrLoanCloseCode, string nlrLoanRegNo, string countryOfOrigin, string createdDate,
                             string createdTime, out string error, out int result)
    {
      Guid guid = CombGuid.Generate();
      bool timedOut = false;
      error = null;
      result = -1;

      _log.Info(string.Format("NLRLoanClose() - CorrelationId {0}", guid.ToString()));

			ServiceLocator.Get<AtlasServiceBus>().GetServiceBus().Publish(new NLRLoanClose(guid)
      {
        LegacyBranchNo = legacyBranchNum,
        SequenceNo = seqNo,
        NLRLoanCloseCode = nlrLoanCloseCode,
        NLRLoanRegistrationNo = nlrLoanRegNo,
        CountryOfOrigin = countryOfOrigin,
        TransactionCreateDate = createdDate,
        TransactionCreateTime = createdTime
      });

      Stopwatch timeout = new Stopwatch();
      timeout.Start();

      _log.Info(string.Format("NLRLoanClose() - Waiting for CorrelationId {0}", guid));

      while (ResponseMessageCache.CheckItem(guid) == null)
      {
        if (timeout.Elapsed.Minutes >= 2)
        {
          timedOut = true;
          timeout.Stop();
          break;
        }
        Thread.Sleep(50);
      }

      if (timedOut)
        return;

      if (ResponseMessageCache.CheckItem(guid) != null)
      {
        var msg = ResponseMessageCache.CheckItem(guid);

        _log.Info(string.Format("NLRLoanClose() -  CorrelationId {0}.", guid));

        ResponseMessageCache.Remove(guid);
      }
    }


    /// <summary>
    /// Submit BatB2 to NLR Registry
    /// </summary>
    public void NLRBatb2(string legacyBranchNum, string seqNo, string idNumber, string accountNo, string annualRateForTotChargeOfCredit, string balanceOverdue, string balanceOverdueIndicator,
                  string branchCode, string currentBalance, string currentBalanceIndicator, string dateAccountOpened, string dateLastPayment, string dateofBirth,
                  string employeeNo, string employerName, string employerPayslipRef, string employmentType, string endUseCode, string title,
                  string forename1, string forename2, string forename3, string surname, string gender, string homeTelCellNo, string workTelCellNo, string interestRateType, string loanIndicator, string loanAmt, string loanType,
                  string monthlyInstalment, string monthInArrears, string nlrEnqRefNo, string nlrLoanRegNo, string nonSaIdentityNumber, string oldAccountNo,
                  string oldSubAccountNo, string oldSupplierBranchCode, string oldSupplierRefNo, string openingBalanceIndicator, string ownerTenant,
                  string postalAddressLine1, string postalAddressLine2, string postalAddressLine3, string postalAddressLine4, string postalPostCode,
                  string randValueInterestCharges, string randValueTotchargeOfCredit, string repaymentPeriod, string residentialAddressLine1,
                  string residentialAddressLine2, string residentialAddressLine3, string residentialAddressLine4, string residentialPostCode, string salaryFrequency, string settlementAmt,
                  string statusCode, string statusDate, string subAccountNo, string totalAmountRepayable, string countryOfOrigin, string createdDate, string createdTime,
                  out string error, out int result)
    {
      Guid guid = CombGuid.Generate();
      bool timedOut = false;
      error = null;
      result = -1;

      _log.Info(string.Format("NLRBatb2() - CorrelationId {0}", guid.ToString()));

			ServiceLocator.Get<AtlasServiceBus>().GetServiceBus().Publish(new BATB2(guid)
      {
        LegacyBranchNo = legacyBranchNum,
        SequenceNo = seqNo,
        IDNo = idNumber,
        AccountNo = accountNo,
        AnnualRateForTotalChargeOfCredit = Decimal.Parse(annualRateForTotChargeOfCredit),
        BalanceOverdue = Decimal.Parse(balanceOverdue),
        BalanceOverdueIndicator = balanceOverdueIndicator,
        BranchCode = branchCode,
        CurentBalanceIndicator = currentBalanceIndicator,
        EmployeeNo = employeeNo,
        EmployerName = employerName,
        EmployerPayslipReferenceNo = employerPayslipRef,
        EmploymentType = employmentType,
        EndUseCode = endUseCode,
        Title = title,
        CurrentBalance = Decimal.Parse(currentBalance),
        AccountOpened = dateAccountOpened,
        LastPayment = dateLastPayment,
        DateOfBirth = dateofBirth,
        Forename1 = forename1,
        Forename2 = forename2,
        Forename3 = forename3,
        Lastname = surname,
        Gender = gender,
        HomeTelCellNo = homeTelCellNo,
        WorkTelCellNo = workTelCellNo,
        InterestRateType = interestRateType,
        LoanIndicator = loanIndicator,
        LoanAmount = Decimal.Parse(loanAmt),
        LoanType = loanType,
        MonthlyInstalment = Decimal.Parse(monthlyInstalment),
        MonthsInArrears = int.Parse(monthInArrears),
        NLREnquiryReferenceNo = nlrEnqRefNo,
        NLRLoanRegistrationNo = nlrLoanRegNo,
        NonSAIDNo = nonSaIdentityNumber,
        OldAccountNo = oldAccountNo,
        OldSubAccountNo = oldSubAccountNo,
        OldSupplierBranchCode = oldSupplierBranchCode,
        OldSupplierReferenceNo = oldSupplierRefNo,
        OpeneingBalanceIndicator = openingBalanceIndicator,
        OwnerTenant = ownerTenant,
        PostalAddressLine1 = postalAddressLine1,
        PostalAddressLine2 = postalAddressLine2,
        PostalAddressLine3 = postalAddressLine3,
        PostalAddressLine4 = postalAddressLine4,
        PostalAddressPostCode = postalPostCode,
        InterestCharges = Decimal.Parse(randValueInterestCharges),
        TotalChargeOfCredit = Decimal.Parse(randValueTotchargeOfCredit),
        RepaymentPeriod = repaymentPeriod,
        ResidentialAddressLine1 = residentialAddressLine1,
        ResidentialAddressLine2 = residentialAddressLine2,
        ResidentialAddressLine3 = residentialAddressLine3,
        ResidentialAddressLine4 = residentialAddressLine4,
        ResidentialAddressPostalCode = residentialPostCode,
        SalaryFrequency = salaryFrequency,
        SettlementAmount = Decimal.Parse(settlementAmt),
        StatusCode = statusCode,
        StatusDate = statusDate,
        SubAccountNo = subAccountNo,
        TotalAmountRepayable = Decimal.Parse(totalAmountRepayable),
        CountryOfOrigin = countryOfOrigin,
        TransactionCreateDate = createdDate,
        TransactionCreateTime = createdTime
      });

      Stopwatch timeout = new Stopwatch();
      timeout.Start();

      _log.Info(string.Format("NLRBatb2() - Waiting for CorrelationId {0}", guid));

      while (ResponseMessageCache.CheckItem(guid) == null)
      {
        if (timeout.Elapsed.Minutes >= 2)
        {
          timedOut = true;
          timeout.Stop();
          break;
        }
        Thread.Sleep(50);
      }

      if (timedOut)
        return;

      if (ResponseMessageCache.CheckItem(guid) != null)
      {
        var msg = ResponseMessageCache.CheckItem(guid);

        _log.Info(string.Format("NLRBatb2() -  CorrelationId {0}.", guid));

        ResponseMessageCache.Remove(guid);
      }
    }


    /// <summary>
    /// returns a string value representation of the respone.
    /// </summary>
    public string GetSubmissionItemResult(string legacyBranchNum, string seqNo, out string error, out int result)
    {
      using (
        var UoW = new UnitOfWork())
      {
        result = 0;
        error = string.Empty;
        string val = string.Empty;

        try
        {
          var branch = new XPQuery<BRN_Branch>(UoW).FirstOrDefault(p => p.LegacyBranchNum.Trim().PadLeft(3, '0') == legacyBranchNum.PadLeft(3, '0'));

          if (branch == null)
            throw new RecordNotFoundException("Branch was not found in the database");

          var associationLink = new XPQuery<BUR_SubmissionAssociation>(UoW).FirstOrDefault(o => o.SubmissionAssociation.SeqNo == seqNo.Trim() && o.SubmissionAssociation.Branch.BranchId ==
                           branch.BranchId);

          if (associationLink == null)
            throw new RecordNotFoundException("Association record was not found in the database");

          var batchItem = new XPQuery<BUR_BatchItem>(UoW).FirstOrDefault(o => o.BatchReferenceNo == associationLink.SubmissionAssociation.UniqueRefNo);

          if (batchItem.Batch.DeliveryDate == null)
            throw new Exception(string.Format("Item {0} has not yet been delivered to the bureau", associationLink.SubmissionAssociation.UniqueRefNo));

          var batchSubmission = new XPQuery<BUR_BatchSubmissionItem>(UoW).FirstOrDefault(o => o.BatchReferenceNo == associationLink.SubmissionAssociation.UniqueRefNo);

          if (batchSubmission.SubmissionBatch.Status == Risk.BatchJobStatus.Job_Cancelled)
            throw new Exception("Batch has been cancelled");

          if (batchSubmission == null)
            throw new RecordNotFoundException("Batch submission item was not found in the database");

          if (batchSubmission.ResponseXML == null)
            throw new Exception(string.Format("No response for sequence {0}", seqNo));

          result = 1;
          return RecordToCSV.Convert(Risk.BatchTransactionType.NLR, Risk.BatchSubTransactionType.Loan, (TRANS_DATARECORD)Xml.DeSerialize<TRANS_DATARECORD>(Compression.Decompress(batchSubmission.ResponseXML)), seqNo);
        }
        catch (Exception ex)
        {
          error = ex.Message;
          result = 0;
        }
        return "No results found.";
      }
    }
  }
}

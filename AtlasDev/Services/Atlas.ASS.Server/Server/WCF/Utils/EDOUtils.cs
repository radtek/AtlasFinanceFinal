using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System.Xml;
using System.Globalization;

using DevExpress.Xpo;

using Atlas.Domain.Model;
using Atlas.Common.Extensions;
using Atlas.Common.Interface;
using Atlas.Business.BankVerification;
using Atlas.Server.Classes.CustomException;


namespace Atlas.Server.WCF.Utils
{
  internal static class EDOUtils
  {
    /// <summary>
    /// Create a new NAEDO contract
    /// </summary>
    /// <param name="log">Logging</param>
    /// <param name="contractNo">The Atlas contract number</param>
    /// <param name="installAmount">Instalment amount</param>
    /// <param name="totInstalments">Total number of instalments</param>
    /// <param name="frequency">'W'/'F'/'M'</param>
    /// <param name="startDate">Start date- must be at least 3 days from now</param>
    /// <param name="branchNumber">The bank branch number</param>
    /// <param name="accountNumber">The bank account number</param>
    /// <param name="accountName">The name of the account</param>
    /// <param name="accountType">Account type</param>
    /// <param name="clientIdentityNumber"></param>
    /// <param name="transactionId"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    internal static bool CreateContract(ILogging log, string contractNo, decimal installAmount,
      int totInstalments, int frequency, DateTime startDate, string branchNumber, string accountNumber,
      string accountName, int accountType, string clientIdentityNumber,
      out int transactionId, out string errorMessage)
    {
      errorMessage = null;
      transactionId = -1;
      var methodName = "CreateContract";
      var request = new
      {
        contractNo,
        installAmount,
        totInstalments,
        frequency,
        startDate,
        branchNumber,
        accountNumber,
        accountName,
        accountType,
        clientIdentityNumber
      };
      log.Information("[{MethodName}]- Request: {@Request}", methodName, request);

      #region Check parameters
      if (DateTime.Now.Subtract(startDate).TotalDays < 3)
      {
        throw new BadParamException(string.Format("Start date must be at least 3 days in advance. startdate: {0:yyyy-MMM-dd}", startDate));
      }

      if (installAmount < 50)
      {
        throw new BadParamException(string.Format("Installment amount must be > R50. installAmount: {0:0.00}", installAmount));
      }

      if (installAmount > 10000)
      {
        throw new BadParamException(string.Format("Installment amount must be < R10,000. installAmount: {0:0.00}", installAmount));
      }

      if (frequency != 1 && frequency != 2 && frequency != 3) // Weekly = 1, Fortnightly = 2, Monthly = 3;
      {
        throw new BadParamException(string.Format("Invalid frequency, must be 1,2 or 3.{0}", frequency));
      }

      if (string.IsNullOrEmpty(contractNo))
      {
        throw new BadParamException("'contractNo' not specified/null");
      }

      if (totInstalments <= 0 || totInstalments > 48)
      {
        throw new BadParamException(string.Format("'totInstalments' must be in range 1-48. totInstalments: {0}", totInstalments));
      }

      if (string.IsNullOrEmpty(branchNumber))
      {
        throw new BadParamException("'branchNumber' not specified/null");
      }

      if (string.IsNullOrEmpty(accountNumber))
      {
        throw new BadParamException("'accountNumber' not specified/null");
      }
      if (string.IsNullOrEmpty(accountName))
      {
        throw new BadParamException("'accountName' not specified/null");
      }

      #region CDV
      #region Determine the bank, using branch code
      long bankId;
      using (var unitOfWork = new UnitOfWork())
      {
        // Try find bank, using the given branch code
        var bankBranch = unitOfWork.Query<BNK_Branch>().FirstOrDefault(s => s.BranchCode == branchNumber);
        if (bankBranch != null)
        {
          bankId = bankBranch.Bank.BankId;
        }
        else
        {
          throw new BadParamException("Unable to determine bank from branch- please use universal branch code");
        }
      }
      #endregion

      var cdv = new AccountCDV();
      if (!cdv.PerformCDV(bankId, accountType, accountNumber, branchNumber))
      {
        throw new BadParamException($"Branch/bank account failed CDV check {branchNumber} - {accountNumber}");
      }
      #endregion

      #endregion

      try
      {
        int merchantId;
        string password;
        string userName;
        Int64 naedoLoginId;
        if (!GetNAEDOAdminLogin(out naedoLoginId, out merchantId, out password, out userName, out errorMessage))
        {
          throw new BadParamException("Failed to locate NAEDO login");
        }

        // TODO: Avoid duplicate successful requests?

        XmlNode upload = null;
        new wsNaedoSoapClient().Using(client =>
          {
            // uploadNaedoTransaction- from manual:
            //   This call is used to register a new Naedo contract when a
            //   client uses the services on offer and merchant requires a
            //   debit order to be loaded against a clients selected account.

            upload = client.uploadNaedoTransaction(merchantId, userName, password, 19 /* NAEDO */, accountName, "ATCORP",
              contractNo, accountNumber, accountName, branchNumber, accountType, frequency, 14 /* NAEDO 3 Day Tracking */,
              totInstalments, startDate, installAmount.ToString("F2", CultureInfo.InvariantCulture), clientIdentityNumber, 0, 0, "", 0, "");

            log.Information("{MethodName}", methodName);
          });

        if (upload == null)
        {
          throw new BadParamException("Empty response from supplier");
        }

        // TODO:
        // ???? Response ???? <transID>1234</transID> 

        // Logging?
        //ALT_NAEDOContractUpload


        return true;
      }
      catch (Exception err)
      {
        // Only return exception message if it is one of ours...
        errorMessage = (err is BadParamException) ? err.Message : "Unexpected server error";
        log.Error(err, "{MethodName}", methodName);
        return false;
      }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="_log"></param>
    /// <param name="atlasBranchNumber"></param>
    /// <param name="transactionId"></param>
    /// <param name="edoType"></param>
    /// <param name="installment"></param>
    /// <param name="newDate"></param>
    /// <param name="frequency"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    internal static bool AlterInstalmentDate(ILogging _log, string atlasBranchNumber, int transactionId, string edoType,
      int installment, DateTime newDate, int frequency, out string errorMessage)
    {
      switch (edoType)
      {
        case "N":
          return AlterInstalmentDateNAEDO(_log, transactionId, newDate, frequency, out errorMessage);

        case "A":
          Int64 aedoLoginId;
          string lendorID;
          string lendorType;
          string userName;
          string password;
          string merchantId;
          if (!GetAEDOAdminLogin(atlasBranchNumber, out aedoLoginId, out lendorID, out lendorType, out userName, out password, out merchantId, out errorMessage))
          {
            _log.Error("Failed to locate AEDO login for branch {Branch}", atlasBranchNumber);
            return false;
          }

          return AlterInstalmentDateAEDO(_log, transactionId, lendorID, lendorType, userName, password, installment, newDate, out errorMessage);

        default:
          errorMessage = "Invalid value for parameter 'edoType'";
          return false;
      }
    }


    /// <summary>
    /// Cancel EDO transactions for edoType. If transactionIds is given, will only cancel those transactions, 
    /// else will use contractNo to cancel all outstanding installments
    /// </summary>
    /// <param name="edoType">The EDO type- 'N' for NAEDO, 'A' for AEDO</param>
    /// <param name="contractNo">The contract number to use for cancellation</param>
    /// <param name="transactionId">The Altech AEDO/NAEDO transaction id</param>
    /// <param name="tccTransactionIds">The Altech transactionIds to use for cancellation</param>
    /// <param name="addToLogging">Must request be logged to ALT_EDOContractCancel, for confirmation task</param>
    /// <param name="log">Logger to use</param>
    /// <returns>error message, else empty string if successful</returns>
    internal static string CancelAllPendingEDOTransactionsFor(ILogging log, string edoType, string contractNo,
      Int64 transactionId, List<Int64> tccTransactionIds, bool addToLogging = true)
    {
      var methodName = "CancelAllPendingEDOTransactionsFor";

      var cancelTransactionIds = new List<Int64>();
      if (tccTransactionIds != null && tccTransactionIds.Count > 0)
      {
        cancelTransactionIds.AddRange(tccTransactionIds);
      }
      if (transactionId > 0)
      {
        cancelTransactionIds.Add(transactionId);
      }

      try
      {
        log.Information("{MethodName} starting- {EDOType}, {ContractNo}, {TransactionId} {@TransactionIds}", methodName, edoType, contractNo, transactionId, tccTransactionIds);

        return (edoType == "N") ?
          CancelNAEDOTransactions(log, contractNo, cancelTransactionIds, addToLogging) :
          CancelAEDOTransactions(log, contractNo, cancelTransactionIds, addToLogging);
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        return "Unexpected server error";
      }
    }


    #region Private methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="log"></param>
    /// <param name="contractNo"></param>
    /// <param name="cancelTransactionIds"></param>
    /// <param name="addToLogging">>Must request be logged to ALT_EDOContractCancel, for confirmation task</param>
    /// <returns>error message, else empty string if successful</returns>
    private static string CancelNAEDOTransactions(ILogging log, string contractNo, List<long> cancelTransactionIds, bool addToLogging = true)
    {
      var methodName = "CancelNAEDOTransactions";
      var result = string.Empty; // Success

      try
      {
        var merchantId = -1;
        string password = null;
        string userName = null;
        Int64 naedoLoginId;
        string errorMessage;
        if (!GetNAEDOAdminLogin(out naedoLoginId, out merchantId, out password, out userName, out errorMessage))
        {
          return errorMessage;
        }

        using (var unitOfWork = new UnitOfWork())
        {
          #region Log
          if (addToLogging)
          {
            var login = unitOfWork.Query<NAEDOLogin>().First(s => s.NAEDOLoginId == naedoLoginId);
            foreach (var cancelTransactionId in cancelTransactionIds)
            {
              new ALT_EDOContractCancel(unitOfWork)
              {
                CreatedDT = DateTime.Now,
                EDOType = "N",
                AltechTransactionId = cancelTransactionId,
                ContractNum = contractNo,
                NAEDOLogin = login
              };
            }
            unitOfWork.CommitChanges();
          }
          #endregion
        }

        foreach (var cancelTransactionId in cancelTransactionIds)
        {
          var inTracking = new ConcurrentBag<int>();
          #region Cancel all possible instalments
          Parallel.For(1, 31, (cancelInstalment) =>
          {
            try
            {
              XmlNode cancelResult = null;
              new wsNaedoSoapClient().Using(client =>
                {
                  cancelResult = client.instalmentCancellation(userName, password, (int)cancelTransactionId, merchantId.ToString(), cancelInstalment);
                });

              if (cancelResult != null)
              {
                if (cancelResult.InnerText == "00" || cancelResult.InnerText == "0")
                {

                }
                else if (cancelResult.InnerText == "70") // Transaction in online run- "Recall" the current installment
                {
                  inTracking.Add(cancelInstalment);
                }
                else
                {
                  log.Warning("Cancel result: {Error}", ErrorCodes.GetNAEDOErrorString(cancelResult.InnerText));
                }
              }
              else
              {
                log.Warning("Cancel result (cancelResult==null)");
              }
            }
            catch (Exception err)
            {
              log.Error(err, "{MethodName}", methodName);
            }
          });
          #endregion

          #region Recall all instalments in tracking
          int recallInstalment;
          while (inTracking.TryTake(out recallInstalment))
          {
            try
            {
              XmlNode recallResult = null;
              new wsNaedoSoapClient().Using(client =>
                {
                  recallResult = client.instalmentRecall(userName, password, (int)cancelTransactionId, merchantId.ToString(), recallInstalment);
                });

              var recallResultStatus = recallResult.SelectSingleNode("/instalmentRecallReportHeader/status");
              if (recallResultStatus != null && (recallResultStatus.InnerText == "00" || recallResultStatus.InnerText == "0")) // success!
              {
                log.Information("NAEDO recall success: {userName}, {password}, {cancelTransactionId}, {merchantId}, {currInstalment}",
                  userName, password, cancelTransactionId, merchantId, recallInstalment);
              }
              else
              {
                log.Error("recallInstalResult: {InnerXML}", recallResult != null ? recallResult.InnerXml : "null");
              }
            }
            catch (Exception err)
            {
              log.Error(err, "{MethodName}", methodName);
            }
          }
          #endregion
        }
      }
      catch (Exception err)
      {
        log.Error(err, "{MethodName}", methodName);
        result = "Unexpected server error";
      }

      return result;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="log"></param>
    /// <param name="contractNo"></param>
    /// <param name="cancelTransactionIds"></param>
    /// <param name="addToLogging">>Must request be logged to ALT_EDOContractCancel, for confirmation task</param>    
    /// <returns>any severe errors, else empty string</returns>
    private static string CancelAEDOTransactions(ILogging log, string contractNo, List<long> cancelTransactionIds, bool addToLogging = true)
    {
      var result = string.Empty;
      var methodName = "CancelAEDOTransactions";

      try
      {
        string password;
        string userName;
        string lendorType;
        string lendorID;
        string legacyBranch = null;
        var contractParts = contractNo.Split(new char[] { 'X', 'x', '-' });
        if (contractParts.Length >= 3)
        {
          legacyBranch = contractParts[0].TrimStart(new char[] { ' ', '0', 'X', 'x' }).PadLeft(3, '0');
        }
        else
        {
          return string.Format("Failed to determine AEDO login from contract: '{0}'", contractNo);
        }

        Int64 aedoLoginId;
        string errorMessage;
        string merchantId;
        if (!GetAEDOAdminLogin(legacyBranch, out aedoLoginId, out lendorID, out lendorType, out userName, out password, out merchantId, out errorMessage))
        {
          log.Error(new Exception(errorMessage), "Failed to locate AEDO login for branch {Branch}", legacyBranch);
          return errorMessage;
        }

        #region Log
        if (addToLogging)
        {
          using (var unitOfWork = new UnitOfWork())
          {
            var logon = unitOfWork.Query<AEDOLogin>().First(s => s.AEDOLoginId == aedoLoginId);
            foreach (var cancelTransactionId in cancelTransactionIds)
            {
              var logCancel = new ALT_EDOContractCancel(unitOfWork)
              {
                CreatedDT = DateTime.Now,
                EDOType = "A",
                AltechTransactionId = cancelTransactionId,
                ContractNum = contractNo,
                AEDOLogin = logon
              };
            }

            unitOfWork.CommitChanges();
          }
        }
        #endregion

        foreach (var cancelTransactionId in cancelTransactionIds)
        {
          var instalmentsCancelled = 0;
          var inTracking = new ConcurrentBag<int>();
          #region Cancel all possible instalments for this contract
          Parallel.For(1, 31, (cancelInstalment) =>
            {
              try
              {
                var request = new { lendorID, lendorType, userName, password, cancelTransactionId, cancelInstalment };
                log.Information("{MethodName}- Canceling instalment: {@Request}", methodName, request);
                string cancelResult = null;
                new NuPayTransactionsServiceSoapClient().Using(client =>
                  {
                    cancelResult = client.InstalmentCancellation_IC(lendorID, lendorType, userName, password, (int)cancelTransactionId, cancelInstalment);
                  });

                if (!string.IsNullOrEmpty(cancelResult))
                {
                  log.Information("{MethodName}- InstalmentCancellation_IC- {@Request}- {Result}", methodName, request, cancelResult);

                  if (cancelResult == "70") // instalment in tracking
                  {
                    inTracking.Add(cancelInstalment);
                  }

                  if (cancelResult == "00" || cancelResult == "0")
                  {
                    Interlocked.Increment(ref instalmentsCancelled);
                  }
                }
                else
                {
                  result = "InstalmentCancellation_IC- null";
                  log.Warning("{MethodName}- InstalmentCancellation_IC-{$Request}- NULL", methodName, request);
                }
              }
              catch (Exception err)
              {
                result = "Unexpected server error";
                log.Error(err, "{MethodName}- InstalmentCancellation_IC", methodName);
              }
            });

          log.Information("{MethodName}- Successfully cancelled: {InstalmentsCancelled}", methodName, instalmentsCancelled);
          #endregion

          #region Anything in tracking- try recall and retry the instalment cancel
          int recallInstalment;
          while (inTracking.TryTake(out recallInstalment))
          {
            new NuPayTransactionsServiceSoapClient().Using(client =>
            {
              try
              {
                var request = new { lendorID, lendorType, userName, password, cancelTransactionId, recallInstalment };
                log.Information("{MethodName}- Recalling instalment: {@Request}", methodName, request);
                string recallResult = null;
                recallResult = client.InstalmentRecall_RE(lendorID, lendorType, userName, password, (int)cancelTransactionId, recallInstalment);

                if (!string.IsNullOrEmpty(recallResult))
                {
                  log.Information("{MethodName}- InstalmentRecall_RE (2)- {@Request}- {Result}", methodName, request, recallResult);

                  if (recallResult == "0" || recallResult == "00")
                  {
                    var cancelResult = client.InstalmentCancellation_IC(lendorID, lendorType, userName, password, (int)cancelTransactionId, recallInstalment);

                    if ((cancelResult ?? "") == "0" || (cancelResult ?? "") == "00")
                    {
                      log.Information("{MethodName}- InstalmentCancellation_IC (2)- {@Request}- {Result}", methodName, request, cancelResult);
                    }
                    else
                    {
                      result = string.Format("Unexpected server error- InstalmentCancellation_IC: {0}", cancelResult);
                      log.Warning("{MethodName}- InstalmentCancellation_IC (2)- {@Request}- {Result}", methodName, request, cancelResult);
                    }
                  }
                }
                else
                {
                  result = "Unexpected server error- InstalmentRecall_RE null";
                  log.Warning("{MethodName}- InstalmentRecall_RE- {@Request}- NULL", methodName, request);
                }
              }
              catch (Exception err)
              {
                result = "Unexpected server error- InstalmentRecall_RE";
                log.Error(err, "{MethodName}- InstalmentRecall_RE", methodName);
              }
            });
          }
          #endregion
        }
      }
      catch (Exception err)
      {
        log.Error(err, "{MethodName}", methodName);
        result = "Unexpected server error";
      }

      return result;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="legacyBranch"></param>
    /// <param name="aedoLoginId"></param>
    /// <param name="lendorID"></param>
    /// <param name="lendorType"></param>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <param name="merchantId"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    private static bool GetAEDOAdminLogin(string legacyBranch, out Int64 aedoLoginId, out string lendorID,
      out string lendorType, out string userName, out string password, out string merchantId, out string errorMessage)
    {
      errorMessage = null;
      aedoLoginId = -1;
      lendorID = null;
      lendorType = null;
      userName = null;
      password = null;
      merchantId = null;

      // TODO: This is a messy/fragile solution- we need to link every branch to an AEDO group merchant:  BRN_Branch.AedoGroupLogin == AEDOLogin  ??
      using (var unitOfWork = new UnitOfWork())
      {
        var branch = unitOfWork.Query<BRN_Branch>().FirstOrDefault(s => s.LegacyBranchNum.PadLeft(3, '0') == legacyBranch);
        if (branch != null)
        {
          var config = unitOfWork.Query<BRN_Config>()
            .FirstOrDefault(s => s.Branch == branch && s.DataType == Atlas.Enumerators.General.BranchConfigDataType.AEDO_Merchant_Id);

          if (config != null)
          {
            merchantId = config.DataValue;
            var merchantNum = !string.IsNullOrEmpty(merchantId) && PowerMerchantNums.Contains(merchantId) ? "406" /* Power */ : "134" /* Atlas */;

            var logon = unitOfWork.Query<AEDOLogin>().FirstOrDefault(s => s.DeletedDT == null && s.MerchantNum == merchantNum);
            if (logon != null)
            {
              aedoLoginId = logon.AEDOLoginId;
              lendorID = logon.MerchantNum;
              lendorType = logon.LendorType;
              userName = logon.UserName;
              // TODO: New column for AEDO logon type: Reporting/Maintenance
              password = (logon.MerchantNum == "134") ? "ATLAS888" : logon.Password;
            }
            return true;
          }
        }
      }

      return false;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="naedoLoginId"></param>
    /// <param name="merchantId"></param>
    /// <param name="password"></param>
    /// <param name="userName"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    private static bool GetNAEDOAdminLogin(out Int64 naedoLoginId, out int merchantId, out string password, out string userName, out string errorMessage)
    {
      merchantId = -1;
      userName = null;
      password = null;
      errorMessage = null;
      naedoLoginId = -1;

      using (var unitOfWork = new UnitOfWork())
      {
        var allLogons = unitOfWork.Query<NAEDOLogin>().Where(s => s.DeletedDT == null && s.MerchantId > 0 && !string.IsNullOrEmpty(s.Password)).ToList();
        var adminCreds = (int)Atlas.Enumerators.Credentials.CredentialPurpose.Administration;
        var logon = allLogons.FirstOrDefault(s => s.CredentialPurposeFlags == adminCreds);

        if (logon != null)
        {
          naedoLoginId = logon.NAEDOLoginId;
          merchantId = logon.MerchantId;
          userName = logon.Username;
          password = logon.Password;
        }
        else
        {
          errorMessage = "Failed to locate NAEDO logon";
          return false;
        }
      }

      return true;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="_log"></param>
    /// <param name="transactionId"></param>
    /// <param name="lendorID"></param>
    /// <param name="lendorType"></param>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <param name="installment"></param>
    /// <param name="newDate"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    private static bool AlterInstalmentDateAEDO(ILogging _log, int transactionId, string lendorID, string lendorType,
      string userName, string password, int installment, DateTime newDate, out string errorMessage)
    {
      errorMessage = null;

      try
      {
        string update = null;
        new NuPayTransactionsServiceSoapClient().Using(client =>
          {
            update = client.InstalmentDateChange_DI(lendorID, lendorType, userName, password, transactionId, installment, newDate);
          });

        if (!string.IsNullOrEmpty(update))
        {
          return false;
        }

        // ???? Response ???? 00 
        return update.Trim() == "00";
      }
      catch (Exception err)
      {
        _log.Error(err, "AlterInstalmentDateAEDO");
        errorMessage = err.Message;
        return false;
      }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="_log"></param>
    /// <param name="transactionId"></param>
    /// <param name="newDate"></param>
    /// <param name="frequency"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    private static bool AlterInstalmentDateNAEDO(ILogging _log, int transactionId, DateTime newDate, int frequency, out string errorMessage)
    {
      try
      {
        int merchantId;
        string password;
        string userName;
        Int64 naedoLoginId;
        if (!GetNAEDOAdminLogin(out naedoLoginId, out merchantId, out password, out userName, out errorMessage))
        {
          return false;
        }

        XmlNode change = null;
        new wsNaedoSoapClient().Using(client =>
        {
          // 7 November 2014 at 13:11 - Brendan van der Mescht <brendanvdm@nupay.co.za>
          //   "As per conversation the card_acceptor is your merchant number, therefor is does pass through the card_acceptor."
          change = client.contractDateChange(userName, password, transactionId, merchantId.ToString(), newDate, frequency.ToString());
        });

        if (change == null)
        {
          errorMessage = "Empty response from supplier";
          return false;
        }

        // ???? Response ???? 00 
        return true;
      }
      catch (Exception err)
      {
        _log.Error(err, "AlterInstalmentDateNAEDO");
        errorMessage = err.Message;
        return false;
      }
    }


    internal static bool GetFutureTrans(string contractRef, string edoType, out DateTime nextInstalmentDate, out decimal nextInstalmentVal, out string errorMessage)
    {
      nextInstalmentDate = DateTime.MinValue;
      nextInstalmentVal = 0;
      errorMessage = string.Empty;

      // TODO:
      return false;
    }

    #endregion


    #region Fields

    private static readonly List<string> PowerMerchantNums = new List<string> { "1504380", "1504398", "1504406","1504414", "1504414", "1504422",
      "1504430", "1504448",  "1504455", "1504463", "1504471", "1504489", "1504497", "1504505", "1504513", "1504521",
      "1504539", "1504547", "1504554", "1504752", "1504760", "1504778", "1504810", "1504828", "1504836", "1504844", "1504851", "1504869",
      "1504877", "1504885", "1504893", "1504901", "1504919", "1504927", "1504935" ,"1504943" ,"1504950", "1504968", "1504976", "1504984", "1509694" };

    #endregion


  }
}

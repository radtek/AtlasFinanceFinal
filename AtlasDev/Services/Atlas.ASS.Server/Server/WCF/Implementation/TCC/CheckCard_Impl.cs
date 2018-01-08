using System;
using System.Linq;
using System.ServiceModel;

using DevExpress.Xpo;
using Newtonsoft.Json;

using Atlas.Domain.Model;
using Atlas.Enumerators;
using Atlas.Common.Extensions;
using Atlas.Common.Utils;
using Atlas.Server.WCF.Utils;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;
using Atlas.Server.Utils;


namespace Atlas.Server.WCF.Implementation.TCC
{
  internal static class CheckCard_Impl
  {
    internal static bool Execute(ILogging log, IConfigSettings config, ICacheServer cache, int terminalID,
      string contractNo,
      string bankShortName, string branchNumber, string accountNumber, int accountType, string panNumber,
      string clientName, string clientIDNumber, int timeoutSeconds,
      out string errorMessage,
      out string responseCodeOut, out string PANNumberOut, out string transactionIDOut, out string accountNumberOut,
      out string accountTypeOut, out string contractTypeOut)
    {
      var methodName = "CheckCard";
      #region Set out params
      errorMessage = string.Empty;
      responseCodeOut = string.Empty;
      PANNumberOut = string.Empty;
      transactionIDOut = string.Empty;
      accountNumberOut = string.Empty;
      accountTypeOut = string.Empty;
      contractTypeOut = string.Empty;
      #endregion
      var result = false;

      try
      {
        var requestStarted = DateTime.Now;
        var inputRequest = JsonConvert.SerializeObject(new
        {
          terminalID,
          contractNo,
          bankShortName,
          branchNumber,
          accountNumber,
          accountType,
          panNumber,
          clientName,
          clientIDNumber,
          timeoutSeconds
        });

        log.Information("{MethodName} started: {@Request}", methodName, inputRequest);

        #region Check parameters
        if (string.IsNullOrEmpty(contractNo))
        {
          errorMessage = "contractNo not specified";
          DbGeneralUtils.LogTCCRequest(terminalID, General.TCCLogRequestType.CheckCard,
              requestStarted, inputRequest, General.TCCLogRequestResult.ApplicationError, errorMessage, DateTime.Now);

          return false;
        }

        if (string.IsNullOrEmpty(panNumber))
        {
          errorMessage = "panNumber not specified";
          DbGeneralUtils.LogTCCRequest(terminalID, General.TCCLogRequestType.CheckCard,
              requestStarted, inputRequest, General.TCCLogRequestResult.ApplicationError, errorMessage, DateTime.Now);

          return false;
        }

        if (string.IsNullOrEmpty(accountNumber))
        {
          errorMessage = "accountNumber not specified";
          DbGeneralUtils.LogTCCRequest(terminalID, General.TCCLogRequestType.CheckCard,
              requestStarted, inputRequest, General.TCCLogRequestResult.ApplicationError, errorMessage, DateTime.Now);

          return false;
        }

        if (string.IsNullOrEmpty(branchNumber))
        {
          errorMessage = "branchNumber not specified";
          DbGeneralUtils.LogTCCRequest(terminalID, General.TCCLogRequestType.CheckCard,
              requestStarted, inputRequest, General.TCCLogRequestResult.ApplicationError, errorMessage, DateTime.Now);

          return false;
        }

        if (branchNumber.Length != 6)
        {
          errorMessage = "Invalid bank branch number- use generic bank branch code";
          DbGeneralUtils.LogTCCRequest(terminalID, General.TCCLogRequestType.CheckCard,
              requestStarted, inputRequest, General.TCCLogRequestResult.ApplicationError, errorMessage, DateTime.Now);

          return false;
        }

        if (string.IsNullOrEmpty(clientName))
        {
          errorMessage = "clientName not specified";
          DbGeneralUtils.LogTCCRequest(terminalID, General.TCCLogRequestType.CheckCard,
              requestStarted, inputRequest, General.TCCLogRequestResult.ApplicationError, errorMessage, DateTime.Now);

          return false;
        }

        if (timeoutSeconds < 10)
        {
          errorMessage = "timeoutSeconds not specified";
          DbGeneralUtils.LogTCCRequest(terminalID, General.TCCLogRequestType.CheckCard,
              requestStarted, inputRequest, General.TCCLogRequestResult.ApplicationError, errorMessage, DateTime.Now);

          return false;
        }
        if (timeoutSeconds > 600)
        {
          errorMessage = "timeoutSeconds cannot exceed 600";
          DbGeneralUtils.LogTCCRequest(terminalID, General.TCCLogRequestType.CheckCard,
              requestStarted, inputRequest, General.TCCLogRequestResult.ApplicationError, errorMessage, DateTime.Now);

          return false;
        }

        // 1- Current, 2- Savings
        if (accountType != 1 && accountType != 2)
        {
          errorMessage = string.Format("Invalid account type: {0}", accountType);
          DbGeneralUtils.LogTCCRequest(terminalID, General.TCCLogRequestType.CheckCard,
              requestStarted, inputRequest, General.TCCLogRequestResult.ApplicationError, errorMessage, DateTime.Now);

          return false;
        }

        clientName = Uri.UnescapeDataString(clientName);
        #endregion

        #region Check card/PAN
        if (panNumber.Length != 15 && panNumber.Length != 16 && panNumber.Length != 18 && panNumber.Length != 20)
        {
          errorMessage = string.Format("Invalid bank card number: '{0}'- the card number has {1} digits- it should only have 16 or 18 digits. Please correct.", panNumber, panNumber.Length);
          return false;
        }

        // We can only validate 16 digit number...18/20 digits do not suport Luhn's checksum from what I can see...
        if (panNumber.Length == 16)
        {
          if (!CCardValidator.Validate(CCardValidator.CardType.Other, panNumber))
          {
            errorMessage = string.Format("Invalid bank card number '{0}'- bank card number fails basic validation. Please correct.", panNumber);
            return false;
          }
        }
        #endregion

        #region Determine the bank, using branch code
        BNK_Bank bank = null;
        using (var uow = new UnitOfWork())
        {
          // Try to find bank, using bank info
          General.BankName bankEnum;
          if (!Enum.TryParse(bankShortName, out bankEnum) || !Enum.IsDefined(typeof(General.BankName), bankEnum))
          {
            errorMessage = string.Format("Invalid bank '{0}'- please contact Atlas IT department.", bankShortName);
            DbGeneralUtils.LogTCCRequest(terminalID, General.TCCLogRequestType.CheckCard,
                  requestStarted, inputRequest, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);
            return false;
          }

          bank = uow.Query<BNK_Bank>().FirstOrDefault(s => s.BankId == (int)bankEnum);
          if (bank == null)
          {
            // Try find bank, using the given branch code
            var bankBranch = uow.Query<BNK_Branch>().FirstOrDefault(s => s.BranchCode == branchNumber);
            if (bankBranch != null)
            {
              bank = bankBranch.Bank;
            }

            if (bank == null)
            {
              errorMessage = string.Format("Invalid/unknown bank branch code: '{0}'. Please use a generic brank branch code", branchNumber);

              DbGeneralUtils.LogTCCRequest(terminalID, General.TCCLogRequestType.CheckCard,
                  requestStarted, inputRequest, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);

              log.Error(new Exception(errorMessage), methodName);

              return false;
            }
          }
        }
        #endregion

        #region Perform CDV
        switch (AtlasCDV.PerformCDV(log, bank.BankId, accountType, accountNumber, branchNumber, out errorMessage))
        {
          case AtlasCDV.CDVResult.CDVPassed:
            log.Information("CDV successful: Bank: {Bank}, Type: {AccountType}, Num: {AccountNumber}, Branch: {BranchNumber}",
              bank.Description, accountType, accountNumber, branchNumber);

            break;

          case AtlasCDV.CDVResult.CDVFailed:
            log.Warning("CDV failed: Bank: {Bank}, Type: {AccountType}, Num: {AccountNumber}, Branch: {BranchNumber}",
              bank.Description, accountType, accountNumber, branchNumber);
            errorMessage = string.Format("Invalid bank account number (CDV failure for bank '{0}'). " +
              "Please verify the bank details.", bank.Description);
            DbGeneralUtils.LogTCCRequest(terminalID, General.TCCLogRequestType.CheckCard,
              requestStarted, inputRequest, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);

            return false;

          case AtlasCDV.CDVResult.ServerError:
            log.Error(new Exception(errorMessage), "CDV exception: Bank: {Bank}, Type: {AccountType}, Num: {AccountNumber}, Branch: {BranchNumber}",
              bank.Description, accountType, accountNumber, branchNumber);

            throw new Exception(errorMessage);
        }
        #endregion

        #region Check terminal is ready
        var terminal = cache.Get<TCCTerminal_Cached>(terminalID);
        if (terminal == null)
        {
          errorMessage = "Specified terminal could not found in system";
          log.Error(new Exception(string.Format("Could not locate terminaID {0}", terminalID)), methodName);
          DbGeneralUtils.LogTCCRequest(terminalID, General.TCCLogRequestType.CheckCard,
              requestStarted, inputRequest, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);

          return false;
        }

        if (terminal.Status != 0)
        {
          switch (terminal.Status)
          {
            case 1:
              errorMessage = "The TCC Terminal is currently in use with another function";
              break;

            case 2:
              errorMessage = "The TCC Terminal is unresponsive- please reset the TCC unit";
              break;

            default:
              errorMessage = "The TCC Terminal is not configured appropriately- please contact Atlas IT department";
              break;
          }

          log.Warning(new Exception(errorMessage), methodName);
          DbGeneralUtils.LogTCCRequest(terminal.TerminalId, General.TCCLogRequestType.CheckCard,
              requestStarted, inputRequest, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);

          return false;
        }
        #endregion

        #region Perform request
        var accountTypeStr = accountType == 1 ? "20" : "10";
        // ASS will give us extra 10 seconds before time-out... give Altech extra 5 seconds of these...
        var aedoTimeOut = TimeSpan.FromSeconds(timeoutSeconds + 5);
        var binding = new BasicHttpBinding("TermRCSoap") { SendTimeout = aedoTimeOut, ReceiveTimeout = aedoTimeOut };
        var clientRef2 = contractNo.Trim();
        var startDate = DateTime.Now.AddDays(15).ToString("yyyyMMdd");
        AFSRsp response = null;
        var exceptionMsg = string.Empty;

        string lastResponseCode = null;
        TccCacheUtils.SetTerminalBusy(cache, terminalID, "aedo_naedo_auth_req");
        try
        {
          try
          {
            var endpointAddress = TccSoapUtils.NPTerminalRC_EP(config);
            new TermRCSoapClient(binding, endpointAddress).Using(client =>
              {
                response = client.AFS_AN_Auth_Req_Test(branch_Code: branchNumber.Trim(),
                  Term_ID: terminal.SupplierTerminalId,
                  clientRef1: terminal.MerchantId,
                  clientRef2: clientRef2,
                  Contract_no: contractNo,
                  start_date: startDate,
                  client_ID: clientIDNumber,
                  clientName: clientName,
                  customScreen: true,
                  Line1: "  Verify card:",
                  Line2: string.Format("  Card: {0}...{1}", panNumber.Substring(0, 4), panNumber.Substring(panNumber.Length - 3, 3)),
                  Line3: "",
                  Line4: "",
                  panIn: panNumber,
                  accountNumberIn: accountNumber,
                  account_Type: accountTypeStr);
              }
            );

            lastResponseCode = response != null && !string.IsNullOrEmpty(response.ResponseCode) ? response.ResponseCode.Trim() : string.Empty;
            result = lastResponseCode == "00";

            #region Process response
            if (response != null && lastResponseCode.Length > 1)
            {
              if (lastResponseCode.StartsWith("00"))
              {
                log.Information("{MethodName} Successful- TransId: {TransactionId}, EDO type: {ContractType}, Acc: {AccountNum}, " +
                  "Acc type: {AccountType}, Tracking: {TrackingId}, Adj: {AdjRule}, Freq: {Frequency}", methodName,
                  response.TransactionID, response.contractType, response.AccountNumber, response.AccountType,
                  response.Tracking, response.AdjRule, response.Frequency);

                // Set output values, if first transaction
                responseCodeOut = "00";
                PANNumberOut = response.PAN;
                transactionIDOut = response.TransactionID;
                accountNumberOut = response.AccountNumber;
                accountTypeOut = response.AccountType;
                contractTypeOut = response.contractType;
                // response.ResponseDescription TODO: ?What?

                DbGeneralUtils.LogTCCRequest(terminal.TerminalId, General.TCCLogRequestType.CheckCard,
                    requestStarted, inputRequest, General.TCCLogRequestResult.Successful,
                    string.Format("TransId: {0}, Type: {1}, Acc: {2}, Acc type: {3}, Tracking: {4}, Adj: {5}, Freq: {6}",
                    response.TransactionID, response.contractType, response.AccountNumber,
                    response.AccountType, response.Tracking, response.AdjRule, response.Frequency), DateTime.Now);
              }
              else
              {
                errorMessage = ErrorCodes.GetAEDOTerminalAuthoriseErrorString(log, lastResponseCode);
                log.Warning(new Exception(errorMessage), methodName);

                exceptionMsg = errorMessage;
                DbGeneralUtils.LogTCCRequest(terminal.TerminalId, General.TCCLogRequestType.CheckCard,
                    requestStarted, inputRequest, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);
              }
            }
            else
            {
              errorMessage = "Empty result was received from TCC";
              exceptionMsg = errorMessage;
              DbGeneralUtils.LogTCCRequest(terminal.TerminalId, General.TCCLogRequestType.CheckCard,
                      requestStarted, inputRequest, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);

              log.Error(new Exception(errorMessage), methodName);
            }
            #endregion
          }
          catch (Exception aedoErr)
          {
            result = false;
            exceptionMsg = aedoErr.Message;
            log.Error(aedoErr, methodName);
            DbGeneralUtils.LogTCCRequest(terminal.TerminalId, General.TCCLogRequestType.CheckCard,
                      requestStarted, inputRequest, General.TCCLogRequestResult.ApplicationError, aedoErr.Message, DateTime.Now);

            errorMessage = "Unexpected server error- please try again and if the same error occurs, please notify Atlas IT department";
          }
        }
        finally
        {
          if (!result && ErrorCodes.DoesAEDOErrCodeMeanTerminalOffline(lastResponseCode))
          {
            // This will force a handshake ASAP to determine actual status    
            TccCacheUtils.SetTerminalError(cache, terminalID, exceptionMsg ?? errorMessage);
          }
          else
          {
            TccCacheUtils.SetTerminalDone(cache, terminalID, lastResponseCode);
          }
        }
        #endregion
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        result = false;
        errorMessage = "Unexpected server error";
      }

      return result;
    }

  }
}

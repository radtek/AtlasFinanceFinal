using System;
using System.Linq;
using System.ServiceModel;
using System.Collections.Generic;

using DevExpress.Xpo;
using Newtonsoft.Json;

using Atlas.Domain.Model;
using Atlas.Enumerators;
using Atlas.Common.Extensions;
using Atlas.Common.Interface;

using Atlas.Cache.Interfaces;
using Atlas.Common.Utils;
using Atlas.Server.WCF.Utils;
using Atlas.Cache.Interfaces.Classes;
using Atlas.Server.Utils;
using Atlas.Server.Classes.CustomException;


namespace Atlas.Server.WCF.Implementation.TCC
{
  internal class AEDOTerminalAuthorise2_Impl
  {
    // TODO: This should return a class instead of so many outs...
    internal static bool Execute(ILogging log, IConfigSettings config, ICacheServer cache, int terminalID,
        string contractNo, decimal installAmount, decimal contractAmount, int totInstalments, string frequency,
        DateTime startDate, string employerCode, string clientIDNumber,
        string bankShortName, string branchNumber, string accountNumber, int accountType, string panNumber,
        string clientName, // Optionals on TCC 
        int timeoutSeconds,
        out string errorMessage,
        out string responseCodeOut, out string PANNumberOut, out string transactionIDOut, out decimal contractAmountOut, out string accountNumberOut,
        out string accountTypeOut, out string contractTrackingOut, out string adjustmentRuleOut, out string frequencyOut, out string contractTypeOut)
    {
      var methodName = "AEDOTerminalAuthorise2";
      #region Set out params
      errorMessage = string.Empty;
      responseCodeOut = string.Empty;
      PANNumberOut = string.Empty;
      transactionIDOut = string.Empty;
      contractAmountOut = 0;
      accountNumberOut = string.Empty;
      accountTypeOut = string.Empty;
      contractTrackingOut = string.Empty;
      adjustmentRuleOut = string.Empty;
      frequencyOut = string.Empty;
      contractTypeOut = string.Empty;
      #endregion

      var result = false;

      var request = new { terminalID, contractNo, installAmount, contractAmount, frequency, startDate, employerCode, clientIDNumber, bankShortName, branchNumber, accountNumber, accountType, panNumber, clientName, timeoutSeconds };
      var inputRequest = JsonConvert.SerializeObject(request);
      var requestStarted = DateTime.Now;
      try
      {
        log.Information("{MethodName} started: {@Request}", methodName, request);

        #region Check parameters
        if (string.IsNullOrEmpty(contractNo))
        {
          throw new BadParamException("contractNo not specified");
        }

        if (installAmount <= 0)
        {
          throw new BadParamException("installAmount not specified");
        }
        if (installAmount > 15000)
        {
          throw new BadParamException("The instalment amount cannot be more than R15,000.00");
        }

        if (contractAmount <= 0)
        {
          throw new BadParamException("contractAmount not specified");
        }
        if (contractAmount > 30000)
        {
          throw new BadParamException("The contract amount cannot be more than R30,000.00");
        }

        if (totInstalments <= 0)
        {
          throw new BadParamException("totInstalments not specified");
        }
        if (totInstalments > 52)
        {
          throw new BadParamException("totInstalments cannot be more than 52");
        }

        // Make sure instalments/contract match
        if (Math.Abs((totInstalments * installAmount) - contractAmount) > 0.24M) // Handle any rounding...
        {
          throw new BadParamException($"The contract amount does not match with number of instalments/instalment amount: {installAmount} x {totInstalments} <> {contractAmount}");
        }

        // Make sure everything balances
        if ((totInstalments * installAmount) != contractAmount)
        {
          contractAmount = totInstalments * installAmount;
        }

        switch (frequency)
        {
          case "F":  // Fortnightly
            frequency = "1";
            break;

          case "W":  // Weekly
            frequency = "0";
            break;

          case "M":
            frequency = "2";
            break;

          default:
            throw new BadParamException($"Unknown frequency parameter: '{frequency}'");
        }

        if (startDate == null || startDate.Subtract(DateTime.Now).TotalDays < 2)
        {
          throw new BadParamException($"Invalid startDate: '{startDate:yyyy-MM-dd}'");
        }

        if (string.IsNullOrEmpty(employerCode))
        {
          throw new BadParamException("employerCode not specified");
        }

        if (string.IsNullOrEmpty(clientIDNumber))
        {
          throw new BadParamException("clientIDNumber not specified");
        }

        if (string.IsNullOrEmpty(panNumber))
        {
          throw new BadParamException("panNumber not specified");
        }

        if (string.IsNullOrEmpty(accountNumber))
        {
          throw new BadParamException("accountNumber not specified");
        }

        if (string.IsNullOrEmpty(branchNumber))
        {
          throw new BadParamException("branchNumber not specified");
        }

        if (branchNumber.Length != 6)
        {
          throw new BadParamException("Invalid bank branch number- use generic bank branch code");
        }

        if (string.IsNullOrEmpty(clientName))
        {
          throw new BadParamException("clientName not specified");
        }

        if (timeoutSeconds < 10)
        {
          throw new BadParamException("timeoutSeconds not specified");
        }
        if (timeoutSeconds > 600)
        {
          throw new BadParamException("timeoutSeconds cannot exceed 600");
        }

        // 1- Current, 2- Savings
        if (accountType != 1 && accountType != 2)
        {
          throw new BadParamException($"Invalid account type: {accountType}");
        }

        clientName = Uri.UnescapeDataString(clientName);
        #endregion

        #region Check card/PAN
        if (panNumber.Length != 15 && panNumber.Length != 16 && panNumber.Length != 18 && panNumber.Length != 20)
        {
          throw new BadParamException($"Invalid bank card number: '{panNumber}'- the card number has {panNumber.Length} digits- " +
            "it should only have 15, 16, 18 or 20 digits. Please correct.");
        }

        // We can only validate 16 digit number...15/18/20 digits do not support Luhn's checksum from what I can see...
        if (panNumber.Length == 16)
        {
          if (!CCardValidator.Validate(CCardValidator.CardType.Other, panNumber))
          {
            throw new BadParamException($"Invalid bank card number '{panNumber}'- bank card number fails basic validation. Please correct.");
          }
        }
        #endregion

        #region Determine the bank, using branch code
        BNK_Bank bank = null;
        using (var unitOfWork = new UnitOfWork())
        {
          // Try to find bank, using bank info
          General.BankName bankEnum;
          if (!Enum.TryParse(bankShortName, out bankEnum) || !Enum.IsDefined(typeof(General.BankName), bankEnum))
          {
            throw new BadParamException($"Invalid bank '{bankShortName}'- please contact Atlas IT department.");
          }

          bank = unitOfWork.Query<BNK_Bank>().FirstOrDefault(s => s.BankId == (int)bankEnum);
          if (bank == null)
          {
            // Try find bank, using the given branch code
            var bankBranch = unitOfWork.Query<BNK_Branch>().FirstOrDefault(s => s.BranchCode == branchNumber);
            if (bankBranch != null)
            {
              bank = bankBranch.Bank;
            }

            if (bank == null)
            {
              throw new BadParamException(string.Format("Invalid/unknown bank branch code: '{0}'. Please use a generic brank branch code", branchNumber));
            }
          }

          #region Ensure the bank account number does not belong to a currently active staff member
          var cleanedAccNum = accountNumber.TrimStart('0').Replace(" ", string.Empty);

          var allActiveStaff = unitOfWork.Query<PER_Person>()
            .Where(s => s.PersonType.TypeId == General.PersonType.Employee.ToInt() &&
                        s.Security != null && s.Security.IsActive)
            .Select(s => s.PersonId).ToList();
          var accounts = unitOfWork.Query<PER_BankDetails>()
            .Where(s => allActiveStaff.Contains(s.Person.PersonId))
            .Select(s => new { s.BankDetail.AccountNum, s.BankDetail.Bank.BankId })
            .ToList();

          if (accounts.Any(s => s.AccountNum.TrimStart('0').Replace(" ", string.Empty) == cleanedAccNum && s.BankId == bank.BankId))
          {
            throw new BadParamException("Cannot use an Atlas staff bank account");
          }

          #endregion
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
            throw new BadParamException($"Invalid bank account number (CDV failure for bank '{bank.Description}'). " +
              "Please verify the bank details.");

          case AtlasCDV.CDVResult.ServerError:
            throw new BadParamException($"CDV exception: Bank: {bank.Description}, Type: {accountType}, Num: {accountNumber}, " +
              $"Branch: {branchNumber}, Error: {errorMessage}");
        }
        #endregion

        #region Check terminal is ready
        var terminal = cache.Get<TCCTerminal_Cached>(terminalID);
        if (terminal == null)
        {
          throw new BadParamException($"Specified terminal {terminalID} could not found in system");
        }

        if (terminal.Status != 0)
        {
          switch (terminal.Status)
          {
            case 1:
              throw new BadParamException("The TCC Terminal is currently in use with another function");

            case 2:
              throw new BadParamException("The TCC Terminal is unresponsive- please reset the TCC unit");

            default:
              throw new BadParamException("The TCC Terminal is not configured appropriately- please contact Atlas IT department");
          }
        }
        #endregion

        #region Get maximum AEDO instalment amount- we need to split
        var maxAEDOContract = 0M;
        if (!Decimal.TryParse(DbGeneralUtils.GetConfigValue(Atlas.Enumerators.Config.CONFIG_AEDO_MAX_INSTALMENT), out maxAEDOContract) || maxAEDOContract == 0)
        {
          throw new BadParamException("Unable to determine system maximum AEDO instalment amount- contact Atlas IT");
        }
        if (maxAEDOContract < 1000)
        {
          throw new BadParamException("Invalid system maximum AEDO instalment amount- contact Atlas IT");
        }
        #endregion

        #region Get TCC display template
        var lineTemplates = new string[4];
        for (var i = 0; i < 4; i++)
        {
          lineTemplates[i] = DbGeneralUtils.GetConfigValue(installAmount <= maxAEDOContract ? Atlas.Enumerators.Config.CONFIG_AEDO_TCC_LINE1_TEMPLATE + i :
              Atlas.Enumerators.Config.CONFIG_AEDO_TCC_LINE1_TEMPLATE_MULTI + i);

          if (string.IsNullOrEmpty(lineTemplates[i]))
          {
            throw new BadParamException($"Line {i} TCC template has not been configured- contact Atlas IT");
          }
        }
        #endregion

        #region Get branch NAEDO
        string branchNAEDO = null;
        using (var unitOfWork = new UnitOfWork())
        {
          var branch = unitOfWork.Query<BRN_Branch>().FirstOrDefault(s => terminal.Branch.PadLeft(3, '0') == s.LegacyBranchNum.PadLeft(3, '0'));
          if (branch == null)
          {
            throw new BadParamException(string.Format("Failed to locate branch: '{0}'", terminal.Branch));
          }

          var branchNAEDOConfig = unitOfWork.Query<BRN_Config>()
            .FirstOrDefault(s => s.Branch == branch && s.DataType == General.BranchConfigDataType.NAEDO_Merchant_Id);
          if (branchNAEDOConfig == null)
          {
            throw new BadParamException($"Failed to locate NAEDO setting for branch: '{branch}");
          }

          branchNAEDO = branchNAEDOConfig.DataValue;
        }
        #endregion

        #region Process call
        // NOTE: Ed Wrede from Altech verbal/email request: 'Time-out should be specified in 60 second increments'
        timeoutSeconds = timeoutSeconds.RoundOff(60);

        string lastResponseCode = null;

        // List of transaction IDs created by TCC
        var transactionIDs = new List<string>();

        var exceptionMsg = string.Empty;
        TccCacheUtils.SetTerminalBusy(cache, terminalID, "aedo_naedo_auth_req");
        try
        {
          var contractsToCreate = (installAmount > maxAEDOContract) ? ((int)Math.Ceiling(installAmount / maxAEDOContract)) : 1;

          // ASS will give us extra 10 seconds before time-out... give Altech extra 5 seconds of these...
          var aedoTimeOut = TimeSpan.FromSeconds(timeoutSeconds + 5);
          var binding = new BasicHttpBinding("TermRCSoap") { SendTimeout = aedoTimeOut, ReceiveTimeout = aedoTimeOut };

          var clientRef2 = contractNo.Trim();

          var currContract = 1;
          var remainingInstalment = installAmount;
          var remainingContract = contractAmount;

          while (remainingInstalment > 0)
          {
            #region Create request parameters
            // Get this instalment- less than max amount
            var thisInstallment = remainingInstalment;
            var thisContract = remainingContract;
            if (thisInstallment >= maxAEDOContract)
            {
              // To avoid Altech instalment de-duplication- randomize the multiple contract instalments
              var randomVal = new int[] { 4500, 4600, 4700, 4800, 4900, 4990 }[currContract - 1];
              thisInstallment = randomVal;
              thisContract = randomVal * totInstalments;
            }

            var displayLines = new string[4];
            for (var i = 0; i < 4; i++)
            {
              displayLines[i] = ConvertTemplate(lineTemplates[i], contractNo: contractNo, installAmount: thisInstallment, contractAmount: thisContract, totInstalments: totInstalments,
                frequency: frequency, startDate: startDate, clientIDNumber: clientIDNumber, panNumber: panNumber, branchNumber: branchNumber,
                accountNumber: accountNumber, clientName: clientName, contractPlanCount: contractsToCreate, contractPlanCurrent: currContract);
            }
            // AccountType == 1 = Current == "20", AccountType == 2 = savings == "10"
            var accountTypeStr = accountType == 1 ? "20" : "10";

            // Add converted info            
            inputRequest = string.Format(
                "{0}\n\rAEDO Merchant: {1}, NAEDO Merchant: {2}, Branch code: {3}, Terminal: {4}, " +
                "Contract No: {5}, Install amount: {6}, Contract amount: {7}, Instalments: {8}, " +
                "Frequency: {9}, StartDate: {10}, Employer: {11}, ClientID: {12}, Custom screen: {13}, " +
                "Line1: {14}, Line2: {15}, Line3: {16}, Line4: {17}, " +
                "Bank: {18}, Branch: {19}, Account Num: {20}, AcctType: {21}, PAN: {22}, " +
                "Timeout: {23}, ClientName: {24}, ClientRef 1: {25}, " +
                "ClientRef 2: {26}, Contracts to create: {27}, This contract: {28}",
                inputRequest, terminal.MerchantId, branchNAEDO, branchNumber, terminal.SupplierTerminalId,
                contractNo, thisInstallment, thisContract, totInstalments,
                frequency, startDate, employerCode, clientIDNumber, true,
                displayLines[0], displayLines[1], displayLines[2], displayLines[3],
                bankShortName, branchNumber, accountNumber, accountTypeStr, panNumber,
                timeoutSeconds, clientName, terminal.MerchantId,
                clientRef2, contractsToCreate, currContract);

            log.Information(inputRequest);
            #endregion

            #region Perform request
            AFSRsp response = null;
            var endpointAddress = TccSoapUtils.NPTerminalRC_EP(config);
            new TermRCSoapClient(binding, endpointAddress).Using(client =>
            {
              response = client.AFS_AN_Auth_Req(
                  AMerchant_ID: terminal.MerchantId,
                  NMerchant_ID: branchNAEDO,
                  branch_Code: branchNumber.Trim(),
                  Term_ID: terminal.SupplierTerminalId,
                  clientRef1: terminal.MerchantId,           // 2012-10-10 - KB
                  clientRef2: clientRef2,
                  Contract_no: contractNo.Trim(),
                  install_amnt: ((int)(thisInstallment * 100)).ToString(),   // Convert amount to cents
                  contract_amnt: ((int)(thisContract * 100)).ToString(), // Convert amount to cents
                  installments: totInstalments.ToString(),
                  frequency: frequency,
                  start_date: startDate.ToString("yyyyMMdd"),
                  employer: employerCode,
                  adj_rule: "04",         // TODO: Config     Move Backward to First Working Day – Weekends Only - changed on advice of Rodney/Johan Bosman- 2013-04-18
                  tracking: "14",         // TODO: Config     3 days tracking                            
                  client_ID: clientIDNumber.Trim(),
                  clientName: clientName.Trim(),
                  customScreen: true,
                  Line1: displayLines[0].Trim(),
                  Line2: displayLines[1].Trim(),
                  Line3: displayLines[2].Trim(),
                  Line4: displayLines[3].Trim(),
                  panIn: panNumber.Trim(),
                  account_Type: accountTypeStr,
                  accountNumberIn: accountNumber.Trim(),
                  aedoGlobalTimeout: (int)timeoutSeconds);
            });
            lastResponseCode = response != null && !string.IsNullOrEmpty(response.ResponseCode) ? response.ResponseCode.Trim() : string.Empty;
            result = lastResponseCode == "00";
            #endregion

            #region Process response
            if (response != null && lastResponseCode.Length > 1)
            {
              if (lastResponseCode.StartsWith("00"))
              {
                log.Information("{MethodName} successful- TransId: {TransId}, EDO type: {ContractType}, Acc: {AccountNumber}, " +
                  "Acc type: {AccountType}, Tracking: {Tracking}, Adj: {AdjRule}, Freq: {Frequency}", methodName,
                    response.TransactionID, response.contractType, response.AccountNumber,
                    response.AccountType, response.Tracking, response.AdjRule, response.Frequency);

                // Set output values, if first transaction
                if (currContract == 1)
                {
                  responseCodeOut = "00";
                  PANNumberOut = response.PAN;
                  transactionIDOut = response.TransactionID;
                  if (!string.IsNullOrEmpty(response.ContractAmount))
                  {
                    contractAmountOut = (Decimal)int.Parse(response.ContractAmount) / 100;
                  }
                  accountNumberOut = response.AccountNumber;
                  accountTypeOut = response.AccountType;
                  contractTrackingOut = response.Tracking;
                  adjustmentRuleOut = response.AdjRule;
                  frequencyOut = response.Frequency;
                  contractTypeOut = response.contractType;
                  // response.ResponseDescription TODO: ?What?
                }

                // Next contract to handle the max outstanding amount
                remainingInstalment -= thisInstallment;
                remainingContract -= thisContract;

                transactionIDs.Add(response.TransactionID);
                currContract++;

                DbGeneralUtils.LogTCCRequest(terminal.TerminalId, General.TCCLogRequestType.AEDONAEDOAuthorise,
                    requestStarted, inputRequest, General.TCCLogRequestResult.Successful,
                    string.Format("TransId: {0}, Type: {1}, Acc: {2}, Acc type: {3}, Tracking: {4}, Adj: {5}, Freq: {6}",
                    response.TransactionID, response.contractType, response.AccountNumber,
                    response.AccountType, response.Tracking, response.AdjRule, response.Frequency), DateTime.Now);
              }
              else
              {
                // Code 108 indicates *we* need to cancel the transaction, because Altech cannot be bothered
                if (lastResponseCode.StartsWith("108"))
                {
                  if (!string.IsNullOrEmpty(response.TransactionID))
                  {
                    transactionIDs.Add(response.TransactionID);
                    result = false;
                  }
                  else
                  {
                    log.Error("Code 108 returned, with an empty transaction ID!");
                  }
                }

                errorMessage = ErrorCodes.GetAEDOTerminalAuthoriseErrorString(log, lastResponseCode);
                exceptionMsg = errorMessage;
                DbGeneralUtils.LogTCCRequest(terminal.TerminalId, General.TCCLogRequestType.AEDONAEDOAuthorise,
                    requestStarted, inputRequest, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);
                log.Warning("{MethodName} Unexpected response: '{LastResponseCode}'- {Error} {Description}", methodName,
                  lastResponseCode, errorMessage, response.ResponseDescription);

                break;
              }
            }
            else
            {
              errorMessage = "Empty result was received from TCC";
              exceptionMsg = errorMessage;
              DbGeneralUtils.LogTCCRequest(terminal.TerminalId, General.TCCLogRequestType.AEDONAEDOAuthorise,
                      requestStarted, inputRequest, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);

              log.Error(new Exception(errorMessage), methodName);

              break;
            }
            #endregion
          }
        }
        catch (Exception aedoErr)
        {
          result = false;
          exceptionMsg = aedoErr.Message;
          log.Error(aedoErr, methodName);
          DbGeneralUtils.LogTCCRequest(terminal.TerminalId, General.TCCLogRequestType.AEDONAEDOAuthorise,
                    requestStarted, inputRequest, General.TCCLogRequestResult.ApplicationError, aedoErr.Message, DateTime.Now);

          errorMessage = "Unexpected server error- please try again and if the same error occurs, please notify Atlas IT department";
        }
        #endregion

        if (!result)
        {
          #region Cancel transactions if failure
          if (transactionIDs.Count > 0) // Failed- we need to cancel all contracts created
          {
            log.Error(new Exception(string.Format("A multiple installment request failed: the following {0} transactions will be cancelled: {1}",
              contractTypeOut, string.Join(", ", transactionIDs))), methodName);
            EDOUtils.CancelAllPendingEDOTransactionsFor(log, contractTypeOut, contractNo, 0, transactionIDs.Select(s => Int64.Parse(s)).ToList());

            errorMessage = string.Format("'{0}'- all previous contracts have been cancelled", errorMessage);
            DbGeneralUtils.LogTCCRequest(terminal.TerminalId, General.TCCLogRequestType.AEDONAEDOAuthorise,
                requestStarted, inputRequest, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);
          }
          #endregion

          if (lastResponseCode != null && ErrorCodes.DoesAEDOErrCodeMeanTerminalOffline(lastResponseCode))
          {
            // This will force a handshake ASAP to determine actual status    
            TccCacheUtils.SetTerminalError(cache, terminalID, exceptionMsg ?? errorMessage);
          }
          else
          {
            TccCacheUtils.SetTerminalDone(cache, terminalID, lastResponseCode);
          }
        }
        else
        {
          TccCacheUtils.SetTerminalDone(cache, terminalID, lastResponseCode);
        }
      }
      catch (Exception err)
      {
        if (err is BadParamException)
        {
          log.Warning(err, methodName);
        }
        else
        {
          log.Error(err, methodName);
        }

        errorMessage = (err is BadParamException) ? err.Message : "Unexpected server error";

        DbGeneralUtils.LogTCCRequest(terminalID, General.TCCLogRequestType.AEDONAEDOAuthorise,
              requestStarted, inputRequest, General.TCCLogRequestResult.ApplicationError, errorMessage, DateTime.Now);

        result = false;
      }

      return result;
    }


    #region Private methods

    /// <summary>
    /// Converts TCC template to final displayable string
    /// </summary>
    /// <param name="template">The template string, fields delimited with {%%}</param>
    /// <param name="contractNo">Contract number</param>
    /// <param name="installAmount">The instalment amount</param>
    /// <param name="contractAmount">The contract amount</param>
    /// <param name="totInstalments">Total number of instalments</param>
    /// <param name="frequency">The instalment frequency</param>
    /// <param name="startDate"></param>
    /// <param name="clientIDNumber"></param>
    /// <param name="panNumber"></param>
    /// <param name="branchNumber"></param>
    /// <param name="accountNumber"></param>
    /// <param name="clientName"></param>
    /// <param name="contractPlanCount"></param>
    /// <param name="contractPlanCurrent"></param>
    /// <returns>Displayable string</returns>
    private static string ConvertTemplate(string template,
        string contractNo, decimal installAmount, decimal contractAmount, int totInstalments, string frequency,
        DateTime startDate, string clientIDNumber,
        string panNumber, string branchNumber, string accountNumber, string clientName,
        int contractPlanCount, int contractPlanCurrent)
    {
      var result = template;
      result = result.Replace("{%CONTRACT_NUMBER%}", contractNo);
      result = result.Replace("{%CLIENTREF%}", contractNo);
      result = result.Replace("{%INSTALMENT_AMOUNT%}", installAmount.ToString("C2"));
      result = result.Replace("{%CONTRACT_AMOUNT%}", contractAmount.ToString("C2"));
      result = result.Replace("{%INSTALMENTS%}", totInstalments.ToString());
      result = result.Replace("{%FREQUENCY%}", frequency);
      result = result.Replace("{%STARTDATE%}", startDate.ToString("yyyy-MMM-dd"));
      result = result.Replace("{%IDNUMBER%}", clientIDNumber);
      result = result.Replace("{%PANNUMBER%}", panNumber);
      result = result.Replace("{%CARDNUMBER%}", panNumber);
      result = result.Replace("{%CARDNO%}", panNumber);
      result = result.Replace("{%BANK_BRANCH_CODE%}", branchNumber);
      result = result.Replace("{%BANK_ACCOUNT_NUMBER%}", accountNumber);
      result = result.Replace("{%CLIENT_NAME%}", clientName);
      result = result.Replace("{%CONTRACT_PLANCOUNT%}", contractPlanCount.ToString());
      result = result.Replace("{%CONTRACT_PLANCURR%}", contractPlanCurrent.ToString());

      return result;
    }

    #endregion

  }
}

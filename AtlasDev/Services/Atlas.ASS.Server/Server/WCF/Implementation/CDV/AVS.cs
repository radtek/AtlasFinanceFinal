/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
* 
*  Description:
*  ------------------
*    Provides bank account verification and validation routines
* 
* 
*  Author:
*  ------------------
*     Keith Blows
* 
* 
*  Revision history: 
*  ------------------ 
*     2012-04-12- Skeleton created
*     2012-06-18- Implemented CDV_Perform using Altech NAEDO/TSP web service 'CDVCheck'
* 
* ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Linq;
using System.ServiceModel;

using DevExpress.Xpo;

using Atlas.Domain.Model;
using Atlas.Enumerators;
using AtlasServer.WCF.Interface;
using Atlas.Common.Interface;
using Atlas.Business.BankVerification;
using Atlas.Server.Classes.CustomException;


namespace Atlas.Server.WCF.Implementation.CDV
{
  /// <summary>
  /// Implementation of Bank Account Verification WCF
  /// </summary>
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class AVS : IAVS
  {
    public AVS(ILogging log)
    {
      _log = log;
    }

    /// <summary>
    /// Perform a check-digit verification (CDV) on bank account
    /// </summary>
    /// <param name="accountNumber">Bank account number</param>
    /// <param name="branchCode">Bank branch code</param>
    /// <param name="accountType">Account type: 1-Current, 2- Savings, 3- Transmission (only used for EFT transactions?) </param>
    /// <param name="errorMessage">Any error message</param>
    /// <param name="result">0- Error, 1- Successful, 2- Details are invalid</param>
    /// <returns>bool- true if successfully communicated, false if any communication error</returns>
    /// <comments></comments>
    public bool CDV_Perform(string branchCode, string accountNumber, int accountType, out string errorMessage, out int result)
    {
      errorMessage = string.Empty;
      result = (int)General.WCFCallResult.BadParams;
      var methodName = "CDV_Perform";
      //_log.Information("CDV_Perform: {BranchCode}, {AccountNumber}, {AccountType}", branchCode, accountNumber, accountType);

      try
      {
        #region Check parameters
        if (accountNumber != null)
        {
          accountNumber = accountNumber.Trim();
        }

        if (branchCode != null)
        {
          branchCode = branchCode.Trim();
        }

        if (string.IsNullOrEmpty(accountNumber) || accountNumber.Length < 7 || accountNumber.Length > 11)
        {
          throw new BadParamException($"Invalid bank account number '{accountNumber}'- must be at least 7 digits and not more than 11 digits");
        }

        if (string.IsNullOrEmpty(branchCode) || branchCode.Length != 6)
        {
          throw new BadParamException($"Invalid bank branch code '{branchCode}'- must be exactly 6 digits");
        }

        if (accountType < 1 || accountType > 3)
        {
          throw new BadParamException($"Unknown account type- {accountType}");
        }
        #endregion

        #region  Check branch code uses one of the generic branch codes
        long bankId = 0;
        using (var unitOfWork = new UnitOfWork())
        {
          var bank = unitOfWork.Query<BNK_Bank>().FirstOrDefault(s => s.UniversalCode == branchCode);
          if (bank != null)
          {
            bankId = bank.BankId;
          }
          else
          {
            // Find bank using our list of branch codes
            var bankBranch = unitOfWork.Query<BNK_Branch>().FirstOrDefault(s => s.BranchCode == branchCode);
            if (bankBranch != null)
            {
              bankId = bankBranch.Bank.BankId;
            }
            else
            {
              throw new BadParamException($"Invalid/unknown bank branch code: '{branchCode}'. Please use the universal/generic brank branch code for this bank");             
            }
          }
        }
        #endregion

        var check = new AccountCDV();
        var cdvCheck = check.PerformCDV(bankId, accountType, accountNumber, branchCode);
        if (!cdvCheck)
        {
          //  Try another account type
          cdvCheck = check.PerformCDV(bankId, accountType == 1 ? 2 : 1, accountNumber, branchCode);
        }
        result = cdvCheck ? (int)General.WCFCallResult.OK : (int)General.WCFCallResult.BadParams;
        return true;
      }
      catch (Exception err)
      {
        _log.Error(err, methodName);
        errorMessage = (err is BadParamException) ? err.Message : "Unexpected server error";
        return false;
      }   
    }


    #region Private fields

    // Logging
    private readonly ILogging _log;

    #endregion

  }
}

/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
* 
*  Description:
*  ------------------
*    Atlas ASS CDV
* 
* 
*  Author:
*  ------------------
*     Keith Blows
* 
* 
*  Revision history: 
*  ------------------ 
*     2012-05- Created
* 
* 
* ----------------------------------------------------------------------------------------------------------------- */

using System;

using Atlas.Common.Interface;
using Atlas.Business.BankVerification;


namespace Atlas.Server.WCF.Utils
{  
  /// <summary>
  /// Atlas bank account CDV routine using BankVerification WCF
  /// </summary>
  public static class AtlasCDV
  {
    /// <summary>
    /// Perform Atlas CDV routine
    /// </summary>
    /// <param name="bank">Bank info</param>
    /// <param name="accountType"></param>
    /// <param name="accountNumber"></param>
    /// <param name="branchNumber"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    public static CDVResult PerformCDV(ILogging log, long bankId, int accountType, string accountNumber, string branchNumber, out string errorMessage)
    {
      var methodName = "PerformCDV";
      errorMessage = null;
      var cdvPassed = false;

      if (bankId <= 0)
      {
        errorMessage = "Parameter 'bankId' cannot be null!";
        log.Error(new Exception(errorMessage), methodName);
        return CDVResult.ServerError;
      }

      try
      {
        var cdv = new AccountCDV();
        cdvPassed = cdv.PerformCDV(bankId, accountType, accountNumber, branchNumber);
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        errorMessage = "Unexpected server error";
        return CDVResult.ServerError;
      }

      if (!cdvPassed)
      {
        errorMessage = string.Format("Invalid bank account number (CDV failure for bank '{0}'). Please verify the bank details.", bankId);
        log.Error(new Exception(errorMessage), methodName);
      }

      return cdvPassed ? CDVResult.CDVPassed : CDVResult.CDVFailed;
    }


    #region Enum

    public enum CDVResult { ServerError = 0, CDVPassed = 1, CDVFailed = 2 };

    #endregion

  }
}

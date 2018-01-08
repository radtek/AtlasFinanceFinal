using System;

using Atlas.Server.WCF.Utils;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;


namespace Atlas.Server.WCF.Implementation.TCC
{
  public static class EDOAddContract_Impl
  {
    internal static bool Execute(ILogging log, IConfigSettings config, ICacheServer cache, 
      string atlasBranchNumber, string contractNo, decimal installAmount,
      int totInstalments, string frequency, DateTime startDate, string bankShortName,
      string branchNumber, string accountNumber, string accountName, int accountType, string clientIdentityNumber,
      out int transactionId, out string errorMessage)
    {
      var methodName = "EDOAddContract_Impl.Execute";
      log.Information("{MethodName}- Starting: {@Request}", methodName, new { atlasBranchNumber, contractNo, installAmount, totInstalments, frequency, startDate, bankShortName, branchNumber, accountNumber, accountName, accountType });
      errorMessage = null;
      transactionId = -1;

      try
      {
        #region Check parameters
        atlasBranchNumber = atlasBranchNumber.Trim();
        if (string.IsNullOrEmpty(atlasBranchNumber) || atlasBranchNumber.Length > 3)
        {
          errorMessage = string.Format("EDOAddContract called with invalid atlasBranchNumber '{0}'", atlasBranchNumber);
          return false;
        }
        atlasBranchNumber = atlasBranchNumber.PadLeft(3, '0');

        if (string.IsNullOrEmpty(contractNo))
        {
          errorMessage = "contractNo not specified";
          return false;
        }

        if (string.IsNullOrEmpty(clientIdentityNumber) || clientIdentityNumber.Length < 5)
        {
          errorMessage = "clientIdentityNumber not specified/invalid";
          return false;
        }

        if (installAmount <= 0)
        {
          errorMessage = "installAmount not specified";
          return false;
        }
        if (installAmount > 15000)
        {
          errorMessage = "The instalment amount cannot be more than R15,000.00";
          return false;
        }
                
        if (totInstalments <= 0)
        {
          errorMessage = "totInstalments not specified";
          return false;
        }
        if (totInstalments > 52)
        {
          errorMessage = "totInstalments cannot be more than 52";
          return false;
        }
               
        int frequencyVal;
        switch (frequency)
        {
          case "W":  // Weekly
            frequencyVal = 1;
            break;

          case "F":  // Fortnightly
            frequencyVal = 2;
            break;
       
          case "M":  // Monthly
            frequencyVal = 3;
            break;

          default:
            errorMessage = string.Format("Unknown frequency parameter: '{0}'", frequency);
            return false;
        }

        if (startDate == null || startDate.Subtract(DateTime.Now).TotalDays < 2)
        {
          errorMessage = string.Format("Invalid startDate: '{0:yyyy-MM-dd}'", startDate);
          return false;
        }

        if (string.IsNullOrEmpty(accountNumber))
        {
          errorMessage = "accountNumber not specified";
          return false;
        }

        if (string.IsNullOrEmpty(branchNumber))
        {
          errorMessage = "branchNumber not specified";
          return false;
        }

        if (branchNumber.Length != 6)
        {
          errorMessage = "Invalid bank branch number- use generic bank branch code";
          return false;
        }

        if (string.IsNullOrEmpty(accountName))
        {
          errorMessage = "accountName not specified";
          return false;
        }

        // 1- Current, 2- Savings
        if (accountType != 1 && accountType != 2)
        {
          errorMessage = string.Format("Invalid account type: {0}", accountType);
          return false;
        }

        accountName = Uri.UnescapeDataString(accountName); // isn't this automatic?

        #endregion

        return EDOUtils.CreateContract(log, contractNo, installAmount, totInstalments,
          frequencyVal, startDate, branchNumber, accountNumber, accountName, accountType, clientIdentityNumber, 
          out transactionId, out errorMessage);
      }
      catch (Exception err)
      {
        log.Error(err, "{MethodName}", methodName);
        errorMessage = "Unexpected server error";
        return false;
      }
    }
  }
}

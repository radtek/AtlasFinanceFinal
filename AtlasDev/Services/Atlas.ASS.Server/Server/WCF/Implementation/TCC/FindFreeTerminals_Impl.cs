using System;
using System.Linq;

using Atlas.Common.Interface;

using Atlas.Enumerators;

using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;
using Atlas.Server.Utils;


namespace Atlas.Server.WCF.Implementation.TCC
{
  internal static class FindFreeTerminals_Impl
  {
    internal static string FindFreeTerminals(ILogging log, IConfigSettings config, ICacheServer cache, string branchNumber)
    {
      var requestStarted = DateTime.Now;
      var methodName = "FindFreeTerminals";
      try
      {
        branchNumber = branchNumber.Trim();
        if (string.IsNullOrEmpty(branchNumber) || branchNumber.Length > 3)
        {
          var errorMessage = string.Format("FindFreeTerminals called with invalid branchNumber '{0}'", branchNumber);
          DbGeneralUtils.LogTCCRequest(0, General.TCCLogRequestType.FindFreeBranchTerminals,
              requestStarted, branchNumber, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);

          return string.Empty;
        }
        branchNumber = branchNumber.PadLeft(3, '0');

        // Find all terminals for this branch
        var terminalIds = TccCacheUtils.GetTerminalIdsForBranch(cache, branchNumber);
        if (terminalIds == null || !terminalIds.Any())
        {
          var errorMessage = string.Format("Could not locate *any* TCC terminals assigned to branch '{0}'- please contact Atlas IT", branchNumber);
          DbGeneralUtils.LogTCCRequest(0, General.TCCLogRequestType.FindFreeBranchTerminals,
              requestStarted, branchNumber, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);

          return "0";
        }
        var terminals = cache.Get<TCCTerminal_Cached>(terminalIds);
        // Check terminals are not currently busy with a transaction/out of order
        var freeTerminals = terminals.Where(s => s.Status == 0);
        if (freeTerminals == null || !freeTerminals.Any())
        {
          var errorMessage = string.Format("Could not locate any *currently available* TCC terminals for branch '{0}'- " +
            "please wait for a terminal to become available, or reset the TCC terminal if it not currently in use", branchNumber);
          DbGeneralUtils.LogTCCRequest(0, General.TCCLogRequestType.FindFreeBranchTerminals,
              requestStarted, branchNumber, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);

          return "0";
        }

        var terminalsFree = string.Join(",", freeTerminals.Select(s => s.TerminalId.ToString()).ToArray());

        DbGeneralUtils.LogTCCRequest(0, General.TCCLogRequestType.FindFreeBranchTerminals,
                requestStarted, branchNumber, General.TCCLogRequestResult.Successful, terminalsFree, DateTime.Now);

        return terminalsFree;
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        return string.Empty;
      }
    }

  }

}

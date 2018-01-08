using System;
using System.Linq;

using Atlas.Enumerators;
using Atlas.Common.Interface;

using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;
using Atlas.Server.Utils;


namespace Atlas.Server.WCF.Implementation.TCC
{
  internal static class FindAllTerminals_Impl
  {
    internal static string Execute(ILogging log, IConfigSettings config, ICacheServer cache, string branchNumber)
    {
      var requestStarted = DateTime.Now;

      try
      {
        branchNumber = branchNumber.Trim();
        if (string.IsNullOrEmpty(branchNumber) || branchNumber.Length > 3)
        {
          var errorMessage = string.Format("FindAllTerminals called with invalid branchNumber '{0}'", branchNumber);
          DbGeneralUtils.LogTCCRequest(0, General.TCCLogRequestType.FindAllBranchTerminals,
            requestStarted, branchNumber, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);
          return string.Empty;
        }
        branchNumber = branchNumber.PadLeft(3, '0');

        // Get all terminals for branch
        var terminalIds = TccCacheUtils.GetTerminalIdsForBranch(cache, branchNumber);
        var terminals = cache.Get<TCCTerminal_Cached>(terminalIds);
        // Filter to non status 99
        var terminalsAvail = (terminals != null && terminals.Any()) ? terminals.Where(s => s.Status != 99) : null;
        if (terminalsAvail == null || !terminalsAvail.Any())
        {
          var errorMessage = string.Format("FindAllTerminals could not locate any terminals for branchNumber '{0}'", branchNumber);
          DbGeneralUtils.LogTCCRequest(0, General.TCCLogRequestType.FindAllBranchTerminals,
              requestStarted, branchNumber, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);

          return "0";
        }

        var terminalsFound = string.Join(",", terminalsAvail.Select(s => s.TerminalId.ToString()).ToArray());

        DbGeneralUtils.LogTCCRequest(0, General.TCCLogRequestType.FindAllBranchTerminals,
                requestStarted, branchNumber, General.TCCLogRequestResult.Successful, terminalsFound, DateTime.Now);

        return terminalsFound;
      }
      catch (Exception err)
      {
        var errorMessage = err.Message;
        DbGeneralUtils.LogTCCRequest(0, General.TCCLogRequestType.FindAllBranchTerminals,
                requestStarted, branchNumber, General.TCCLogRequestResult.ApplicationError, errorMessage, DateTime.Now);
        return string.Empty;
      }
    }

  }
}

using System;
using System.Linq;
using System.Collections.Generic;

using Atlas.Cache.Interfaces.Classes;
using Atlas.Cache.Interfaces;


namespace Atlas.Server.Utils
{
  public static class TccCacheUtils
  {
    /// <summary>
    /// Sets a terminal as in use
    /// </summary>
    /// <param name="terminalID">The terminal ID</param>
    /// <param name="requestType">Request the terminal is busy with</param>
    internal static void SetTerminalBusy(ICacheServer cache, long terminalID, string requestType)
    {
      cache.GetAndUpdateLocked<TCCTerminal_Cached>(terminalID, (terminal) =>
        {
          terminal.Status = 1;
          terminal.LastRequestType = requestType;
          terminal.LastRequestDT = DateTime.Now;
          return terminal;
        });
    }


    /// <summary>
    /// Sets terminal status to ready
    /// </summary>
    /// <param name="terminalID">Terminal ID</param>
    /// <param name="lastResult">Result of last operation</param>
    /// <param name="lastPolledResult">The polled result</param>
    internal static void SetTerminalDone(ICacheServer cache, long terminalID, string lastResult = null, string lastPolledResult = null)
    {
      cache.GetAndUpdateLocked<TCCTerminal_Cached>(terminalID, (terminal) =>
      {
        if (!string.IsNullOrEmpty(lastResult))
        {
          terminal.LastRequestResult = lastResult.Substring(0, Math.Min(500, lastResult.Length));
        }
        if (!string.IsNullOrEmpty(lastPolledResult))
        {
          terminal.LastPolledResult = lastPolledResult.Substring(0, Math.Max(500, lastPolledResult.Length));
        }
        terminal.Status = 0;

        return terminal;
      });
    }


    /// <summary>
    /// Sets terminal to error status
    /// </summary>
    /// <param name="terminalID">Terminal ID</param>
    /// <param name="lastResult">Last message from TCC</param>
    internal static void SetTerminalError(ICacheServer cache, long terminalID, string lastResult = null)
    {
      cache.GetAndUpdateLocked<TCCTerminal_Cached>(terminalID, (terminal) =>
      {
        if (!string.IsNullOrEmpty(lastResult))
        {
          terminal.LastRequestResult = lastResult.Substring(0, Math.Min(500, lastResult.Length));
        }
        // Force a handshake ASAP...
        terminal.LastRequestDT = DateTime.Now.Subtract(new TimeSpan(0, 10, 0));
        terminal.Status = 2;

        return terminal;
      });
    }


    /// <summary>
    /// Sets terminal as unavailable
    /// </summary>
    /// <param name="terminalId">The Terminal ID</param>
    /// <param name="responseCode">TCC code returned</param>
    /// <param name="error">Errr code returned</param>
    /// <param name="lastPolled">Last polled date/time</param>
    internal static void SetTerminalFailed(ICacheServer cache, long terminalId, string responseCode, string error, DateTime lastPolled)
    {
      cache.GetAndUpdateLocked<TCCTerminal_Cached>(terminalId, (terminal) =>
      {
        if (terminal.Status == 1)
        {
          return terminal;
        }

        terminal.LastPolledDT = lastPolled;
        terminal.Status = 2;
        terminal.FailedPingCount++;
        terminal.LastPolledResult = responseCode ?? error;
        return terminal;
      });
    }


    /// <summary>
    /// Sets terminal as being available
    /// </summary>
    /// <param name="terminalId">The Terminal ID</param>
    /// <param name="lastPolled">Last polled date/time</param>
    internal static void SetTerminalOnline(ICacheServer cache, long terminalId, DateTime lastPolled)
    {
      cache.GetAndUpdateLocked<TCCTerminal_Cached>(terminalId, (terminal) =>
      {
        if (terminal.Status == 1)
        {
          return terminal;
        }

        terminal.LastPolledDT = lastPolled;
        terminal.Status = 0;
        terminal.SuccessPingCount++;
        terminal.LastPolledResult = string.Empty;
        return terminal;
      });
    }


    /// <summary>
    /// Sets the terminal as being in an unknown state (can't connect to Altech TCC server)
    /// </summary>
    /// <param name="terminalId">The Terminal ID</param>
    /// <param name="lastPolled">Last polled date/time</param>
    internal static void SetTerminalUnknown(ICacheServer cache, long terminalId, DateTime lastPolled)
    {
      cache.GetAndUpdateLocked<TCCTerminal_Cached>(terminalId, (terminal) =>
      {
        if (terminal.Status == 1) // in use!
        {
          return terminal;
        }
        terminal.LastPolledDT = lastPolled;
        terminal.UnknownPingCount++;
        return terminal;
      });
    }


    /// <summary>
    /// Returns next 'active' terminal, excluding the terminals given
    /// </summary>
    /// <param name="ignoreTerminals">Terminal ID's to be ignored</param>
    /// <returns>TerminalDTO of first active terminal, excluding terminals to be ignored</returns>
    internal static TCCTerminal_Cached GetNextActivatedTerminal(ICacheServer cache, List<long> ignoreTerminals)
    {
      var terminals = GetAllActivatedTerminals(cache);
      terminals = terminals.Where(s => !ignoreTerminals.Any(i => i == s)).ToList();
      var info = cache.Get<TCCTerminal_Cached>(terminals);
      return info.FirstOrDefault(s => s.Status != 99);
    }


    /// <summary>
    /// Get list of all terminals activated, irrespective if they are busy or not
    /// </summary>
    /// <param name="cache"></param>
    /// <returns></returns>
    internal static List<long> GetAllActivatedTerminals(ICacheServer cache)
    {
      return cache.GetAll<TCCTerminal_Cached>()
          .Where(s => s.Status != 99)
          .Select(s => s.TerminalId)
          .ToList();
    }


    /// <summary>
    /// Marks a terminal as being online
    /// </summary>
    /// <param name="terminalId">The Terminal ID</param>
    /// <param name="lastPolled">Last polled</param>
    internal static void SetTerminalSuccess(ICacheServer cache, long terminalId, DateTime lastPolled)
    {
      cache.GetAndUpdateLocked<TCCTerminal_Cached>(terminalId, (terminal) =>
      {
        terminal.Status = 0;
        terminal.SuccessPingCount++;
        terminal.LastPolledDT = lastPolled;

        return terminal;
      });
    }


    /// <summary>
    /// Marks a terminal as still being unavailable
    /// </summary>
    /// <param name="terminalId">The Terminal ID</param>
    /// <param name="error">Error message</param>
    /// <param name="lastPolled">Last polled</param>
    internal static void SetTerminalFailure(ICacheServer cache, long terminalId, string error, DateTime lastPolled)
    {
      cache.GetAndUpdateLocked<TCCTerminal_Cached>(terminalId, (terminal) =>
      {
        terminal.Status = 2;
        terminal.FailedPingCount++;
        terminal.LastPolledResult = error;
        terminal.LastPolledDT = lastPolled;

        return terminal;
      });
    }


    /// <summary>
    /// Returns the next 'stuck' terminal
    /// </summary>
    /// <param name="ignoreTerminals">Terminal IDs already processed and to be ignored</param>
    /// <returns>TerminalD</returns>
    internal static TCCTerminal_Cached GetNextStuckTerminal(ICacheServer cache, List<long> ignoreTerminals)
    {
      // No NuPay function should not take more than 7 minutes to complete... add 1 minute to be sure
      var xMinutesAgo = DateTime.Now.AddMinutes(-8);
      var terminal = cache.GetAll<TCCTerminal_Cached>().FirstOrDefault(s =>
            (s.Status == 1 || s.Status == 2 || s.Status == 3) &&
            (s.LastRequestDT <= xMinutesAgo || !s.LastRequestDT.HasValue) &&
            !ignoreTerminals.Contains(s.TerminalId));

      return terminal;
    }
    

    internal static List<long> GetTerminalIdsForBranch(Cache.Interfaces.ICacheServer cache, string branchNumber)
    {
      return cache.GetAll<Cache.Interfaces.Classes.TCCTerminal_Cached>()
          .Where(s => s.Branch.PadLeft(3, '0') == branchNumber.PadLeft(3, '0'))
          .Select(s => s.TerminalId)
          .ToList();
    }
 
  }
  
}

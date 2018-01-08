using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.Enumerators;


namespace Atlas.Data.Repository
{
  public static class TCCData
  {
    /// <summary>
    /// Gets list of all terminals for a branch
    /// </summary>
    /// <param name="branchID">Legacy branch number</param>
    /// <returns>List of branches</returns>
    public static List<TCCTerminalDTO> GetTerminalsForBranch(string branchID)
    {
      using (var unitOfWork = new UnitOfWork())
      {
        var terminals = unitOfWork.Query<TCCTerminal>().Where(s => s.Branch == branchID);
        return AutoMapper.Mapper.Map<IQueryable<TCCTerminal>, List<TCCTerminalDTO>>(terminals);
      }
    }


    /// <summary>
    /// Returns a specific TCC DTO
    /// </summary>
    /// <param name="terminalID">Terminal ID</param>
    /// <returns>TCC DTO, null if not found</returns>
    public static TCCTerminalDTO GetTerminal(long terminalID)
    {
      using (var unitOfWork = new UnitOfWork())
      {
        var terminal = unitOfWork.Query<TCCTerminal>().FirstOrDefault(s => s.TerminalId == terminalID);
        return AutoMapper.Mapper.Map<TCCTerminal, TCCTerminalDTO>(terminal);
      }
    }


    /// <summary>
    /// Sets a terminal as in use
    /// </summary>
    /// <param name="terminalID">The terminal ID</param>
    /// <param name="requestType">Request the terminal is busy with</param>
    public static void SetTerminalBusy(long terminalID, string requestType)
    {
      using (var unitOfWork = new UnitOfWork())
      {
        var terminal = unitOfWork.Query<TCCTerminal>().First(s => s.TerminalId == terminalID);
        terminal.Status = 1;
        terminal.LastRequestType = requestType;
        terminal.LastRequestDT = DateTime.Now;
        terminal.Save();

        unitOfWork.CommitChanges();
      }      
    }


    /// <summary>
    /// Sets terminal status to ready
    /// </summary>
    /// <param name="terminalID">Terminal ID</param>
    /// <param name="lastResult">Result of last operation</param>
    /// <param name="lastPolledResult">The polled result</param>
    public static void SetTerminalDone(long terminalID, string lastResult = null, string lastPolledResult = null)
    {
      using (var unitOfWork = new UnitOfWork())
      {
        var terminal = unitOfWork.Query<TCCTerminal>().First(s => s.TerminalId == terminalID);
        if (!string.IsNullOrEmpty(lastResult))
        {
          terminal.LastRequestResult = lastResult.Substring(0, Math.Min(500, lastResult.Length));
        }

        if (!string.IsNullOrEmpty(lastPolledResult))
        {
          terminal.LastPolledResult = lastPolledResult.Substring(0, Math.Max(500, lastPolledResult.Length));
        }
        terminal.Status = 0;
        terminal.Save();

        unitOfWork.CommitChanges();
      }
    }


    /// <summary>
    /// Sets terminal to error status
    /// </summary>
    /// <param name="terminalID">Terminal ID</param>
    /// <param name="lastResult">Last message from TCC</param>
    public static void SetTerminalError(long terminalID, string lastResult = null)
    {
      using (var unitOfWork = new UnitOfWork())
      {
        var terminal = unitOfWork.Query<TCCTerminal>().First(s => s.TerminalId == terminalID);
        if (!string.IsNullOrEmpty(lastResult))
        {
          terminal.LastRequestResult = lastResult.Substring(0, Math.Min(500, lastResult.Length));
        }
        // Force a handshake ASAP...
        terminal.LastRequestDT = DateTime.Now.Subtract(new TimeSpan(0, 10, 0));
        terminal.Status = 2;
        terminal.Save();

        unitOfWork.CommitChanges();
      }
    }


    /// <summary>
    /// Logs a TCC request
    /// </summary>
    /// <param name="terminalId">The terminal ID</param>
    /// <param name="requestType">The TCC request type</param>
    /// <param name="requestStartDT">Request started date/time</param>
    /// <param name="requestParams">Request parameters</param>
    /// <param name="requestResult">Result of the request</param>
    /// <param name="resultMessage">Message result</param>
    /// <param name="requestEndDT">Rrequest ended date/time</param>
    public static void LogTCCRequest(long terminalId, General.TCCLogRequestType requestType, DateTime requestStartDT, string requestParams,
        General.TCCLogRequestResult requestResult, string resultMessage, DateTime requestEndDT)
    {
      using (var unitOfWork = new UnitOfWork())
      {
        var terminal = unitOfWork.Query<TCCTerminal>().FirstOrDefault(s => s.TerminalId == terminalId);
        if (terminal == null)
        {
          terminal = unitOfWork.Query<TCCTerminal>().First();
        }

        var requestLog = new LogTCCTerminal(unitOfWork)
        {
          StartDT = requestStartDT,
          RequestParam = requestParams,
          RequestType = requestType,
          ResultMessage = resultMessage,
          ResultType = requestResult,
          Terminal = terminal,
          EndDT = requestEndDT
        };
        requestLog.Save();

        unitOfWork.CommitChanges();
      }
    }

  }
}

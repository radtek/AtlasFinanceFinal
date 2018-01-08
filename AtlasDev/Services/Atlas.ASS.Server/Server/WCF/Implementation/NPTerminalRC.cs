/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012-2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Implementation of the NuPay Terminal Cloud Control (TCC) WCF Interface 
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2012-04-17- Basic functionality implemented and tested
 *     
 *     2012-04-20- Updated service ref for new time-out functionality implemented by Altech NuPay
 *     
 *     2012-05-14- Removed TerminalReady calls before AEDO/capture- have to wait 3 seconds between 'handshake' and 
 *                 AEDO/capture ES calls, else the second call fails. 
 *                 
 *     2012-06-04- AEDO- Major overhaul- flexible config-driven TCC display/ handle > 5000 / remove AEDO post check on error
 *     
 *     2012-06-05- AEDO- Where instalment is > 5000 split into 2 (<10000) or 3 (>10000) 'random' 
 *                 contract amounts: R4,550 / R4,560 / remainder
 *                 Major fixes
 *                 
 *     2012-06-06- When AEDO fails, queue an AEDO WS and cancel transactions (Quartz task to perform)- not implemented yet
 *     
 *     2012-06-17- TCC AEDO: Fixed some small, nasty bugs with >5000 instalment amounts 
 *                 TCC AEDO: Simplified > 5000 instalment contract break-up methodology
 *                 TCC AEDO: Added: Where AEDO instalments >5000, AEDO contract amounts must match 
 *                 (no. instalments * instalment = contract)
 *                 TCC AEDO: Added: Display error message on TCC
 *                 
 *     2012-06-22- TCC AEDO: Tweaked result code '108' message
 *     
 *     2012-07-25- Converted from Entity Framework to DataObjects.net
 *     
 *     2012-07-30- Tweaked AEDO to not call FlashMessage if any kind of connection error
 *                 Faster polling
 *     
 *     2012-08-06- Converted from from DataObjects.net to DevEx XPO !
 *     
 *     2012-09-06- Made all calls to TCC, keep the comms objects in memory for the absolute minimum period
 *                 Lots of clean-ups/simplifications *     
 * 
 *     2012-10-10- NuPay changes: AEDO: ClientRef1 to contain branch AEDO merchant ID, not ATFIN
 *     
 *     2012-10-25- More detailed error messages with Free terminal WCF call
 *     
 *     2012-10-29- Added Lee's CDV routines which will require the use of generic branch codes and
 *                 will validate the bank details before hitting TCC
 *                 
 *     2012-11-05- Added PAN check for 16-digit
 *
 *     2013-05-08- Changed to use new WCF-based CDV
 *     
 *     2013-05-10- Simplified AEDO2 and removed throwing exceptions, so only one exception is logged
 *     
 *     
 * -----------------------------------------------------------------------------------------------------------------  */

using System;
using System.ServiceModel;

using AtlasServer.WCF.Interface;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Server.WCF.Implementation.TCC;


namespace AtlasServer.WCF.Implementation
{
  /// <summary>
  /// Implementation of TCC terminal WCF
  /// </summary>
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class NPTerminalRC : INPTerminalRC
  {
    public NPTerminalRC(ILogging log, IConfigSettings config, ICacheServer cache)
    {
      _log = log;
      _config = config;
      _cache = cache;
    }

    #region Strictly DB related

    /// <summary>
    /// Returns a CSV string of all TerminalID's located in a branch
    /// </summary>
    /// <param name="branchNumber">Legacy branch number</param>
    /// <returns>Comma separated string of TCCTerminalIds found for branch, empty if none found</returns>
    public string FindAllTerminals(string branchNumber)
    {
      return FindAllTerminals_Impl.Execute(_log, _config, _cache, branchNumber);
    }


    // Returns a CSV string of TerminalID's which are currently online and free
    public string FindFreeTerminals(string branchNumber)
    {
      return FindAllTerminals_Impl.Execute(_log, _config, _cache, branchNumber);
    }


    // Gets a specific terminal's description
    public string GetTerminalDescription(int terminalID)
    {
      return GetTerminalDescription_Impl.Execute(_log, _config, _cache, terminalID);
    }

    #endregion


    #region Terminal related

    /// <summary>
    /// Performs a check to determine if the card to be swiped is in the BIN table
    /// </summary>
    /// <param name="terminalID">TCCTerminalId to be used</param>
    /// <param name="timeoutSeconds">Time-out, specified in seconds</param>
    /// <param name="errorMessage">Displayable error message</param>
    /// <returns></returns>
    public bool UpfrontBinCheck(int terminalID, int timeoutSeconds, out string errorMessage)
    {
      return UpfrontBinCheck_Impl.Execute(_log, _config, _cache, terminalID, timeoutSeconds, out errorMessage);
    }


    /// <summary>
    /// Authorises an AEDO contract, should fallback to NAEDO 
    /// </summary>
    /// <param name="terminalID">Atlas terminal ID to use for transaction</param>
    /// <param name="contractNo">Atlas contract reference</param>
    /// <param name="installAmount">Instalment amount</param>
    /// <param name="contractAmount">Contract amount</param>
    /// <param name="totInstalments">Total number of instalments</param>
    /// <param name="frequency">Instalment frequency</param>
    /// <param name="startDate">Start date for contract (must be >2 days from today)</param>
    /// <param name="employerCode">The Atlas employer code</param>
    /// <param name="clientIDNumber">Client's ID number</param>
    /// <param name="accountType">Account type- 1= current, 2=savings</param>
    /// <param name="bankShortName">Short, 3 character bank name, i.e. 'NED', 'STD'</param>
    /// <param name="panNumber">Card number</param>
    /// <param name="branchNumber">Bank branch</param>
    /// <param name="accountNumber">Bank account</param>
    /// <param name="clientName">Client's name</param>
    /// <param name="timeoutSeconds">Time-out in seconds</param>
    /// <param name="errorMessage">Error message (output)</param>
    /// <param name="responseCodeOut">TCC response code (output)</param>
    /// <param name="PANNumberOut">Actual card</param>
    /// <param name="transactionIDOut">TCC transaction ID</param>
    /// <param name="contractAmountOut">Contract amount (out)</param>
    /// <param name="accountNumberOut">Account number (out)</param>
    /// <param name="accountTypeOut">Account type (out)</param>
    /// <param name="contractTrackingOut">Contract tracking type (out)</param>
    /// <param name="adjustmentRuleOut">Adjustment rule (out)</param>
    /// <param name="frequencyOut">Payment frequency (out)</param>
    /// <param name="contractTypeOut">contract type  (out): 'A'- AEDO  / 'N'- NAEDO</param>
    /// <returns>true for success, false for failure</returns>
    public bool AEDOTerminalAuthorise2(int terminalID,
          string contractNo, decimal installAmount, decimal contractAmount, int totInstalments, string frequency,
          DateTime startDate, string employerCode, string clientIDNumber,
          string bankShortName, string branchNumber, string accountNumber, int accountType, string panNumber,
          string clientName, // Optionals on TCC 
          int timeoutSeconds,
          out string errorMessage,
          out string responseCodeOut, out string PANNumberOut, out string transactionIDOut, out decimal contractAmountOut, out string accountNumberOut,
          out string accountTypeOut, out string contractTrackingOut, out string adjustmentRuleOut, out string frequencyOut, out string contractTypeOut)
    {
      return AEDOTerminalAuthorise2_Impl.Execute(_log, _config, _cache, terminalID,
        contractNo, installAmount, contractAmount, totInstalments, frequency,
        startDate, employerCode, clientIDNumber,
        bankShortName, branchNumber, accountNumber, accountType, panNumber,
        clientName, // Optionals on TCC 
        timeoutSeconds,
        out errorMessage,
        out responseCodeOut, out PANNumberOut, out transactionIDOut, out contractAmountOut, out accountNumberOut,
        out accountTypeOut, out contractTrackingOut, out adjustmentRuleOut, out frequencyOut, out contractTypeOut);
    }


    // inputType: 1- Alphanumeric, 2- Alpha input, 3- Numeric input  
    public bool CaptureData(int terminalID, int displaySeconds, int timeoutSeconds,
        int inputType, string prompTextline1, string prompTextline2, string prompTextline3, string prompTextline4,
        out string errorMessage, out string capturedData)
    {
      return CaptureData_Impl.Execute(_log, _config, _cache, terminalID, displaySeconds, timeoutSeconds,
        inputType, prompTextline1, prompTextline2, prompTextline3, prompTextline4,
        out errorMessage, out capturedData);
    }


    public bool CheckCard(int terminalID,
      string contractNo,
      string bankShortName, string branchNumber, string accountNumber, int accountType, string panNumber,
      string clientName, string clientIDNumber, int timeoutSeconds,
      out string errorMessage,
      out string responseCodeOut, out string PANNumberOut, out string transactionIDOut, out string accountNumberOut,
      out string accountTypeOut, out string contractTypeOut)
    {
      return CheckCard_Impl.Execute(_log, _config, _cache, terminalID,
        contractNo,
        bankShortName, branchNumber, accountNumber, accountType, panNumber,
        clientName, clientIDNumber, timeoutSeconds,
        out errorMessage,
        out responseCodeOut, out PANNumberOut, out transactionIDOut, out accountNumberOut,
        out accountTypeOut, out contractTypeOut);
    }

    #endregion


    #region AEDO/NAEDO TCC support

    public bool CancelContract(string contractNo, string edoType, string edoReference, out string errorMessage)
    {
      return CancelContract_Impl.Execute(_log, contractNo, edoType, edoReference, out errorMessage);
    }

    #endregion


    #region Contract maintenance

    public bool EDOAlterInstalmentDate(string atlasBranchNumber, int transactionId, string edoType, int installment, DateTime newDate, string frequency,
      out string errorMessage)
    {
      return EDOAlterInstalmentDate_Impl.Execute(_log, _config, _cache, atlasBranchNumber, transactionId, edoType, installment, newDate, frequency, out errorMessage);
    }


    public bool EDOAddContract(string atlasBranchNumber, string contractNo, decimal installAmount, int totInstalments, string frequency,
        DateTime startDate, string bankShortName, string branchNumber, string accountNumber, string accountName, int accountType, string clientIdentityNumber,
        out int transactionId, out string errorMessage)
    {
      return EDOAddContract_Impl.Execute(_log, _config, _cache,
        atlasBranchNumber, contractNo, installAmount, totInstalments,
        frequency, startDate, bankShortName, branchNumber, accountNumber, accountName, accountType, clientIdentityNumber,
        out transactionId, out errorMessage);
    }


    public bool EDOGetFutureTrans(string contractRef, string edoType,
      out DateTime nextInstalmentDate, out decimal nextInstalmentVal, out string errorMessage)
    {
      return EDOGetFutureTrans_Impl.Execute(_log, _config, _cache,
        contractRef, edoType, out nextInstalmentDate, out nextInstalmentVal, out errorMessage);
    }

    #endregion


    private readonly ILogging _log;
    private readonly ICacheServer _cache;
    private readonly IConfigSettings _config;

  }
}

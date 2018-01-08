/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012-2013 Atlas Finance (Pty) Ltd.
* 
* 
*  Description:
*  ------------------
*    Gets the statement for a NuCard
* 
* 
*  Author:
*  ------------------
*     Keith Blows
* 
* 
*  Revision history: 
*  ------------------ 
* 
* 
*  All functions result:
*  -----------------------
*     -1 - Parameters bad
*      0 - No operation performed
*      1 - Successful
*      2 - Can't communicate with the service provider
*      3 - unexpected server error (database, logic error)
*         
* ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Linq;

using Serilog;
using Newtonsoft.Json;

using Atlas.Domain.DTO;
using Atlas.Enumerators;
using Atlas.ThirdParty.XMLRPC.Classes;
using Atlas.ThirdParty.XMLRPC.Utils;
using Atlas.WCF.Implementation;
using Atlas.NuCard.Repository;
using Atlas.NuCard.WCF.Interface;


namespace AtlasServer.WCF.Admin.Implementation
{
  public static class CardStatement_Impl
  {
    /// <summary>
    /// Gets statement for a card
    /// </summary>
    /// <param name="sourceRequest">Source request parameters</param>
    /// <param name="cardNumber">Full card number</param>
    /// <param name="statementResult">Statement lines</param>
    /// <param name="serverTransactionID">Server transaction ID (out)</param>
    /// <param name="errorMessage">Any error message to display to the end-user (out)</param>
    /// <returns>General.WCFCallResult enum (1- success, 0- no operation, etc.)</returns>
    public static int CardStatement(SourceRequest sourceRequest, string cardNumber,
      out StatementResult statementResult,
      out string serverTransactionID, out string errorMessage)
    {
      var methodName = "CardStatement";
      var startDT = DateTime.Now;
      var sourceRequestString = JsonConvert.SerializeObject(new { sourceRequest, cardNumber });
      _log.Information("{MethodName} starting: {@Request}", methodName, sourceRequest);

      errorMessage = string.Empty;
      serverTransactionID = string.Empty;
      statementResult = new StatementResult();

      PER_SecurityDTO swOperator = null;
      COR_AppUsageDTO application = null;
      BRN_BranchDTO branch = null;
      string xmlSent = null;
      string xmlRecv = null;
      try
      {
        #region Check parameters
        if (!WCFUtils.CheckSourceRequest(sourceRequest, out application, out swOperator, out branch, out errorMessage))
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.CardStatement, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        string terminalID;
        string profileNumber;
        string terminalPassword;
        var checkSourceRequest = WCFUtils.GetTutukaFromRequest(sourceRequest, out terminalID, out profileNumber, out terminalPassword);
        if (checkSourceRequest != WCFUtils.CheckSourceRequestResult.ParamsOK)
        {
          errorMessage = WCFUtils.SourceRequestErrorString(checkSourceRequest);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.CardStatement, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        var card = WCFUtils.CheckCard(cardNumber, null, null, null, false, out errorMessage);
        if (card == null)
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.CardStatement, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }
        if (card.NuCardProfile != null)
        {
          terminalID = card.NuCardProfile.TerminalId;
          profileNumber = card.NuCardProfile.ProfileNum;
          terminalPassword = card.NuCardProfile.Password;
        }
        else
        {
          terminalID = branch.DefaultNuCardProfile.TerminalId;
          profileNumber = branch.DefaultNuCardProfile.ProfileNum;
          terminalPassword = branch.DefaultNuCardProfile.Password;
        }
        #endregion

        #region XML-RPC
        var xmlRequest = new Statement_Input()
        {
          cardNumber = cardNumber,
          profileNumber = profileNumber,
          terminalID = terminalID,
          transactionDate = DateTime.Now,
          transactionID = Guid.NewGuid().ToString()
        };

        var xmlResult = NuCardXMLRPCUtils.Statement(Atlas.Server.NuCard.Utils.CachedValues.TutukaEndpoint, terminalPassword, xmlRequest, out xmlSent, out xmlRecv, out errorMessage);
        var endDT = DateTime.Now;
        serverTransactionID = xmlResult.serverTransactionID;

        NuCardDb.LogAdminRequest(sourceRequestString, application, startDT, endDT, NuCard.AdminRequestType.CardStatement,
          xmlResult.resultCode, xmlResult.balanceAmount != null ? (Decimal)xmlResult.balanceAmount.Value / 100M : 0, xmlRequest.cardNumber, string.Empty,
          xmlSent, xmlRecv, xmlRequest.transactionID, xmlResult.serverTransactionID, errorMessage);

        #endregion

        // If a card is stopped/cancelled, we get a 'custom error'/lost card/etc, but we still get some details- treat as successful
        if (xmlResult.statement != null && xmlResult.statement.Any())
        {
          xmlResult.resultCode = (int)NuCard.AdminRequestResult.Successful;
        }

        #region Return result
        if (xmlResult.resultCode != (int)NuCard.AdminRequestResult.Successful)
        {
          errorMessage = NuCardXMLRPCUtils.GetNuCardErrorString(xmlResult.resultCode, xmlResult.resultText);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.CardStatement, new Exception(errorMessage), xmlSent, xmlRecv, sourceRequest);
          return (int)General.WCFCallResult.ServiceProviderCommsError;
        }

        statementResult.BalanceInCents = xmlResult.balanceAmount.HasValue ? (int)xmlResult.balanceAmount : 0;
        statementResult.ExpiryDate = xmlResult.expiryDate.HasValue ? (DateTime)xmlResult.expiryDate : DateTime.MinValue;
        statementResult.ProfileNum = profileNumber;

        #region Copy statement lines
        statementResult.StatementLines = new StatementLine[xmlResult.statement.Length];
        for (var i = 0; i < xmlResult.statement.Length; i++)
        {
          statementResult.StatementLines[i] = new StatementLine();
          statementResult.StatementLines[i].TransactionAmountInCents = xmlResult.statement[i].transactionAmount;
          statementResult.StatementLines[i].TransactionDate = xmlResult.statement[i].transactionDate;
          statementResult.StatementLines[i].TransactionDescription = xmlResult.statement[i].transactionDescription;
          statementResult.StatementLines[i].TransactionType = xmlResult.statement[i].transactionType;
        }
        #endregion

        _log.Information("{0} completed successfully with result: {@StatementResult}", methodName, statementResult);
        return (int)General.WCFCallResult.OK;
        #endregion
      }
      catch (Exception err)
      {        
        Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.CardStatement, err, null, null, sourceRequest);
        errorMessage = Atlas.Server.NuCard.WCF.Implementation.Consts.ERR_SERVER_GENERIC;
        return (int)General.WCFCallResult.ServerError;
      }
    }

    
    #region Logging

    private static readonly ILogger _log = Log.Logger.ForContext<AtlasServer.WCF.Implementation.NuCardAdminServer>();

    #endregion

  }
}

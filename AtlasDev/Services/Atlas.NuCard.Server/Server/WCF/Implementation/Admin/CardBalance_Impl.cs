/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012-2013 Atlas Finance (Pty) Ltd.
* 
* 
*  Description:
*  ------------------
*    Gets the balance for a card
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

#region Using

using System;
using System.Linq;

using Serilog;
using Newtonsoft.Json;

using Atlas.Domain.DTO;
using Atlas.Enumerators;
using Atlas.ThirdParty.XMLRPC.Classes;
using Atlas.ThirdParty.XMLRPC.Utils;
using Atlas.WCF.Implementation;
using Atlas.NuCard.WCF.Interface;
using Atlas.NuCard.Repository;

#endregion


namespace AtlasServer.WCF.Admin.Implementation
{
  public static class CardBalance_Impl
  {
    /// <summary>
    /// Returns a card's balance + additional info (expiry date/expired/stopped reason)
    /// </summary>
    /// <param name="sourceRequest">Source request parameters</param>
    /// <param name="cardNumber"></param>
    /// <param name="transactionID">Source reference ID</param>
    /// <param name="balanceResult">Balance information (out)</param>
    /// <param name="serverTransactionID">Server transaction ID (out)</param>
    /// <param name="errorMessage">Any error message to display to the end-user (out)</param>
    /// <returns>General.WCFCallResult enum (1- success, 0- no operation, etc.)</returns>
    public static int CardBalance(SourceRequest sourceRequest,
        string cardNumber, string transactionID,
      out BalanceResult balanceResult, out string serverTransactionID, out string errorMessage)
    {
      var methodName = "CardBalance";
      var startDT = DateTime.Now;
      var sourceRequestString = JsonConvert.SerializeObject(new { sourceRequest, cardNumber, transactionID });
      _log.Information("{MethodName} starting: {@Request}, {CardNumber}", methodName, sourceRequest, cardNumber);

      errorMessage = string.Empty;
      serverTransactionID = string.Empty;
      balanceResult = new BalanceResult();

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
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.CardBalance, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        string terminalID;
        string profileNumber;
        string terminalPassword;
        var checkSourceRequest = WCFUtils.GetTutukaFromRequest(sourceRequest,
          out terminalID, out profileNumber, out terminalPassword);
        if (checkSourceRequest != WCFUtils.CheckSourceRequestResult.ParamsOK)
        {
          errorMessage = WCFUtils.SourceRequestErrorString(checkSourceRequest);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.CardBalance, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        if (string.IsNullOrEmpty(transactionID))
        {
          errorMessage = "Invalid transaction ID";
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.CardBalance, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        // Lost/stopped cards can still have a balance...
        //var cardStatus = Enum.GetValues(typeof(NuCard.NuCardStatus)).Cast<NuCard.NuCardStatus>()
        //  .Where(s => s != NuCard.NuCardStatus.NotSet).ToList();
        var card = WCFUtils.CheckCard(cardNumber, null, null /* !!!! */, null, false, out errorMessage);
        if (card == null)
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.CardBalance,
            new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        /* !!!!!!!!!!!! */
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
        var xmlRequest = new Balance_Input()
        {
          profileNumber = profileNumber,
          terminalID = terminalID,
          cardNumber = cardNumber,
          transactionDate = DateTime.Now,
          transactionID = transactionID
        };

        _log.Information("[CardBalance] Request: {@XmlRequest}", xmlRequest);

        string error;
        var xmlResult = NuCardXMLRPCUtils.Balance(Atlas.Server.NuCard.Utils.CachedValues.TutukaEndpoint, terminalPassword, xmlRequest, 
          out xmlSent, out xmlRecv, out error);

        _log.Information("[CardBalance] Result: Sent: {XmlSent}, Recv: {XmlRecv}, Result: {@XmlResult}", xmlSent, xmlRecv, xmlResult, sourceRequest);

        serverTransactionID = xmlResult.serverTransactionID;
        var endDT = DateTime.Now;
        NuCardDb.LogAdminRequest(sourceRequestString, application, startDT, endDT, NuCard.AdminRequestType.CardBalance,
          xmlResult.resultCode, xmlResult.balanceAmount != null ? (Decimal)xmlResult.balanceAmount.Value / 100M : 0, xmlRequest.cardNumber, null,
          xmlSent, xmlRecv, xmlRequest.transactionID, xmlResult.serverTransactionID, error);
        #endregion

        #region Return result
        if (xmlResult.resultCode != (int)NuCard.AdminRequestResult.Successful)
        {
          // Special case- Celia uses this to determine card expiry to avoid loading on expired cards
          if (xmlResult.resultCode == (int)NuCard.AdminRequestResult.CardNotAllocated)
          {
            balanceResult = new BalanceResult() { BalanceInCents = 0, ExpiryDate = DateTime.Today.AddYears(2).AddMonths(6), Lost = false, Expired = false, Stolen = false, Stopped = false };
            return (int)General.WCFCallResult.OK;            
          }

          errorMessage = NuCardXMLRPCUtils.GetNuCardErrorString(xmlResult.resultCode, xmlResult.resultText);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.CardBalance, new Exception(errorMessage), xmlSent, xmlRecv, sourceRequest);
          return (int)General.WCFCallResult.ServiceProviderCommsError;
        }

        balanceResult.BalanceInCents = xmlResult.balanceAmount ?? 0;
        balanceResult.Expired = xmlResult.expired.ToLower() == "yes";
        balanceResult.ExpiryDate = xmlResult.expiryDate ?? DateTime.MinValue;
        balanceResult.Lost = xmlResult.lost.ToLower() == "yes";
        balanceResult.Stolen = xmlResult.stolen.ToLower() == "yes";
        balanceResult.Stopped = xmlResult.stopped.ToLower() == "yes";
        balanceResult.ProfileNum = profileNumber;

        _log.Information("{MethodName} completed successfully with result: {@BalanceResult}", methodName, balanceResult);
        return (int)General.WCFCallResult.OK;
        #endregion
      }
      catch (Exception err)
      {
        Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.CardBalance, err, xmlSent, xmlRecv, sourceRequest);
        errorMessage = Atlas.Server.NuCard.WCF.Implementation.Consts.ERR_SERVER_GENERIC;
        return (int)General.WCFCallResult.ServerError;
      }
    }


    #region Logging

    private static readonly ILogger _log = Log.Logger.ForContext<AtlasServer.WCF.Implementation.NuCardAdminServer>();

    #endregion
  }
}

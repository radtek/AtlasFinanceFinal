/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012-2013 Atlas Finance (Pty) Ltd.
* 
* 
*  Description:
*  ------------------
*    Stops a card
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
using Atlas.WCF.XMLRPC.Cashless;
using Atlas.NuCard.Repository;

#endregion


namespace AtlasServer.WCF.Admin.Implementation
{
  public static class StopCard_Impl
  {
    /// <summary>
    /// Stops a card
    /// </summary>
    /// <param name="sourceRequest">Source request parameters</param>
    /// <param name="cardNumber">Full card number</param>
    /// <param name="stopReasonCodeID">Reason code for stopping the card</param>
    /// <param name="transactionID">Source reference ID</param>
    /// <param name="serverTransactionID">Server transaction ID (out)</param>
    /// <param name="errorMessage">Any error message to display to the end-user (out)</param>
    /// <returns>General.WCFCallResult enum (1- success, 0- no operation, etc.)</returns>
    public static int StopCard(SourceRequest sourceRequest,
      string cardNumber, int stopReasonCodeID, string transactionID,
      out int transferredAmountInCents, out string serverTransactionID, out string errorMessage)
    {
      var methodName = "StopCard";
      var startDT = DateTime.Now;
      var sourceRequestString = JsonConvert.SerializeObject(new { sourceRequest, cardNumber, stopReasonCodeID, transactionID });
      _log.Information("{MethodName} starting: {@Request}", methodName, sourceRequest);

      transferredAmountInCents = 0;
      errorMessage = string.Empty;
      serverTransactionID = string.Empty;
      var multiStepTransaction = 0;

      PER_SecurityDTO swOperator = null;
      COR_AppUsageDTO application = null;
      BRN_BranchDTO branch = null;
      string xmlSent = null;
      string xmlRecv = null;
      try
      {
        string terminalID;
        string profileNumber;
        string terminalPassword;

        #region Check parameters
        if (!WCFUtils.CheckSourceRequest(sourceRequest, out application, out swOperator, out branch, out errorMessage))
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.CardStop, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        // If ASS, machine *MUST* match naming convention: <bbb>-00-<ss>  Where <bbb> is branch and <ss> is station number
        if (sourceRequest.MachineName != "HO-SRV-TS-2")
        {
          if (application.Application.AtlasApplication == General.ApplicationIdentifiers.ASS &&
            !System.Text.RegularExpressions.Regex.IsMatch(sourceRequest.MachineName, "[0-9A-Z]{2,3}\\-00\\-[0-9]{2,2}"))
          {
            errorMessage = string.Format("Your machine's name does not conform to Atlas naming standards: '{0}'", sourceRequest.MachineName);
            return (int)General.WCFCallResult.BadParams;
          }
        }

        var checkSourceRequest = WCFUtils.GetTutukaFromRequest(sourceRequest, out terminalID, out profileNumber, out terminalPassword);
        if (checkSourceRequest != WCFUtils.CheckSourceRequestResult.ParamsOK)
        {
          errorMessage = WCFUtils.SourceRequestErrorString(checkSourceRequest);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.CardStop, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }
      
        if (!Enum.IsDefined(typeof(StopReasonCodes), stopReasonCodeID))
        {
          errorMessage = string.Format("Invalid stopReasonCodeID: '{0}'", stopReasonCodeID);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.CardStop, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }
             
        var card = WCFUtils.CheckCard(cardNumber, null, null /* !!!! */, null /* !!!! */, false, out errorMessage); 
        if (card == null)
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.CardStop,
            new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        /* !!!!!!!!!!!!!!!!!!!!!! */
        // Already stopped- assume success
        /*if (card.Status.Type == NuCard.NuCardStatus.Expired || card.Status.Type == NuCard.NuCardStatus.Emergency_Replacement ||
          card.Status.Type == NuCard.NuCardStatus.EXPIR || card.Status.Type == NuCard.NuCardStatus.FAULT ||
          card.Status.Type == NuCard.NuCardStatus.LOST || card.Status.Type == NuCard.NuCardStatus.Stopped_ConsolidateToSingle ||
          card.Status.Type == NuCard.NuCardStatus.Stopped_Lost || card.Status.Type == NuCard.NuCardStatus.Stopped_NoLongerActive ||
          card.Status.Type == NuCard.NuCardStatus.Stopped_OutcomeQuery || card.Status.Type == NuCard.NuCardStatus.Stopped_PIN_Exceeded ||
          card.Status.Type == NuCard.NuCardStatus.Stopped_Stolen || card.Status.Type == NuCard.NuCardStatus.Suspect_Fraud)
        {
          serverTransactionID = Guid.NewGuid().ToString();
          NuCardDb.LogAdminRequest(sourceRequestString, application, startDT, DateTime.Now, NuCard.AdminRequestType.CardStop,
            1, 0, cardNumber, string.Empty, null, null, transactionID, serverTransactionID, "Card already stopped- no action performed");
          
          return (int)General.WCFCallResult.OK;
        }
        */
        // IGNORE status- Try stop irrespective?
        /*if (card.Status.Type != NuCard.NuCardStatus.Active && card.Status.Type != NuCard.NuCardStatus.ISSUE && card.Status.Type != NuCard.NuCardStatus.USE)
        {
          errorMessage = string.Format("Card cannot be stopped due to card status: {0}", card.Status.Description);
          Utils.LogBadRequest(_log, methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.CardStop, new Exception(errorMessage), null, null, sourceRequest);          
          return (int)General.WCFCallResult.BadParams;
        }*/

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

        #region Remove funds from the card
        var balanceTransId = string.Format("{0}/{1}", transactionID, ++multiStepTransaction);
        BalanceResult balance;
        string balanceServerTransId;
        var balResult = CardBalance_Impl.CardBalance(sourceRequest, cardNumber, balanceTransId, out balance, out balanceServerTransId, out errorMessage);
        if (balResult != (int)General.WCFCallResult.OK)
        {
          return balResult;
        }
        
        // If already stopped/expired/lost- nothing to do!
        if (balance.Expired || balance.Lost || balance.Stolen || balance.Stopped)
        {          
          NuCardDb.LogAdminRequest(sourceRequestString, application, startDT, DateTime.Now, NuCard.AdminRequestType.CardStop,
            1, (Decimal)balance.BalanceInCents / 100M, cardNumber, string.Empty, null, null, transactionID, balanceServerTransId, "Card already disabled- no action was performed");

          var setCardStatus = NuCard.NuCardStatus.LOST;
          if (balance.Stolen)
          {
            setCardStatus = NuCard.NuCardStatus.Stopped_Stolen;
          }
          else if (balance.Lost)
          {
            setCardStatus = NuCard.NuCardStatus.Stopped_Lost;
          }
          else if (balance.Stopped)
          {
            setCardStatus = NuCard.NuCardStatus.Stopped_Lost; // ?? //
          }
          else if (balance.Expired)
          {
            setCardStatus = NuCard.NuCardStatus.Expired;
          }
          
          NuCardDb.UpdateCardStatus(cardNumber, profileNumber, branch.LegacyBranchNum, setCardStatus, swOperator);

          return (int)General.WCFCallResult.OK;
        }
        
        if (balance.BalanceInCents > 100)
        {
          var deductTransId = string.Format("{0}/{1}", transactionID, ++multiStepTransaction);
          string deductServerTransId;
          var deductFundsResult = DeductFromCardLoadProfile_Impl.DeductFromCardLoadProfile(sourceRequest, 
            cardNumber, balance.BalanceInCents, deductTransId,
            out transferredAmountInCents, out deductServerTransId, out errorMessage);
          if (deductFundsResult != (int)General.WCFCallResult.OK)
          {
            return deductFundsResult;
          }
        }
        #endregion

        #region XML-RPC
        var xmlRequest = new StopCard_Input()
        {
          cardNumber = cardNumber,
          profileNumber = profileNumber,
          terminalID = terminalID,
          transactionDate = DateTime.Now,
          transactionID = transactionID,
          stopReasonID = stopReasonCodeID          
        };

        var xmlResult = NuCardXMLRPCUtils.StopCard(Atlas.Server.NuCard.Utils.CachedValues.TutukaEndpoint, terminalPassword, xmlRequest, out xmlSent, out xmlRecv, out errorMessage);
        var endDT = DateTime.Now;
        serverTransactionID = xmlResult.serverTransactionID;

        NuCardDb.LogAdminRequest(sourceRequestString, application, startDT, endDT, NuCard.AdminRequestType.CardStop,
          xmlResult.resultCode, 0, xmlRequest.cardNumber, string.Empty,
          xmlSent, xmlRecv, xmlRequest.transactionID, xmlResult.serverTransactionID, errorMessage);

        #endregion

        #region Return result
        if (xmlResult.resultCode != (int)NuCard.AdminRequestResult.Successful &&
          xmlResult.resultCode != (int)NuCard.AdminRequestResult.CardExpired &&
          xmlResult.resultCode != (int)NuCard.AdminRequestResult.CardHasNoValueOrCancelled &&
          xmlResult.resultCode != (int)NuCard.AdminRequestResult.CardLost &&
          xmlResult.resultCode != (int)NuCard.AdminRequestResult.CardStopped &&
          xmlResult.resultCode != (int)NuCard.AdminRequestResult.StolenCard)
        {          
          errorMessage = NuCardXMLRPCUtils.GetNuCardErrorString(xmlResult.resultCode, xmlResult.resultText);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.CardStop, new Exception(errorMessage), xmlSent, xmlRecv, sourceRequest);
          return (int)General.WCFCallResult.ServiceProviderCommsError;
        }

        var cardStatus = NuCard.NuCardStatus.LOST;          
        switch (stopReasonCodeID)
        {
          case (int)StopReasonCodes.CardConsolidation:
            cardStatus = NuCard.NuCardStatus.Stopped_ConsolidateToSingle;
            break;

          case (int)StopReasonCodes.CardEmergencyReplacement:
            cardStatus = NuCard.NuCardStatus.Emergency_Replacement;
            break;

          case (int)StopReasonCodes.CardLost:
            cardStatus = NuCard.NuCardStatus.Stopped_Lost;
            break;

          case (int)StopReasonCodes.CardNoLongerActive:
            cardStatus = NuCard.NuCardStatus.Stopped_NoLongerActive;
            break;

          case (int)StopReasonCodes.CardPendingQuery:
            cardStatus = NuCard.NuCardStatus.Stopped_OutcomeQuery;
            break;

          case (int)StopReasonCodes.CardPINRetriesExceeded:
            cardStatus = NuCard.NuCardStatus.Stopped_PIN_Exceeded;
            break;

          case (int)StopReasonCodes.CardStolen:
            cardStatus = NuCard.NuCardStatus.Stopped_Stolen;
            break;

          case (int)StopReasonCodes.CardSuspectFraud:
            cardStatus = NuCard.NuCardStatus.Suspect_Fraud;
            break;
        }
        
        NuCardDb.UpdateCardStatus(cardNumber, profileNumber, branch.LegacyBranchNum, cardStatus, swOperator);

        _log.Information("[0] completed successfully- transferred: {1:c}", methodName, (Decimal)transferredAmountInCents / 100M);
        return (int)General.WCFCallResult.OK;
        #endregion
      }
      catch (Exception err)
      {
        Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.CardStop, err, xmlSent, xmlRecv, sourceRequest);
        errorMessage = Atlas.Server.NuCard.WCF.Implementation.Consts.ERR_SERVER_GENERIC;
        return (int)General.WCFCallResult.ServerError;
      }
    }


    #region Logging

    private static readonly ILogger _log = Log.Logger.ForContext<AtlasServer.WCF.Implementation.NuCardAdminServer>();

    #endregion
  }
}

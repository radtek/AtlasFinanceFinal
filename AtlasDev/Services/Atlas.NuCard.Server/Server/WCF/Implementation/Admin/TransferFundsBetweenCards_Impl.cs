/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012-2013 Atlas Finance (Pty) Ltd.
* 
* 
*  Description:
*  ------------------
*    Transfers funds from old to a new card- works even if cards have different profiles
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
using System.Collections.Generic;

using Serilog;
using Newtonsoft.Json;

using Atlas.NuCard.Data.Repository;
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
  public static class TransferFundsBetweenCards_Impl
  {
    /// <summary>
    /// Transfers all the funds from one card to another
    /// Pass 0 or less for amountInCents parameter, to transfer all funds from the card
    /// </summary>
    /// <param name="sourceRequest">Source request parameters</param>
    /// <param name="cardNumberFrom">Full card number to use as the source (must be allocated)</param>
    /// <param name="cardNumberTo">Full card number to use as the destination (card must be active)</param>
    /// <param name="amountInCents">Amount to transfer in cents (0 or less for all)</param>
    /// <param name="transactionID">Source reference ID</param>
    /// <param name="stopTheFromCard">Must the source card be stopped?</param>
    /// <param name="stopReasonCodeID">Reason code for stopping the source card</param>
    /// <param name="transferredAmountInCents">Amount in cent which was transferred</param>
    /// <param name="serverTransactionID">Server transaction ID (out)</param>
    /// <param name="errorMessage">Any error message to display to the end-user (out)</param>
    /// <returns>General.WCFCallResult enum (1- success, 0- no operation, etc.)</returns>
    public static int TransferFundsBetweenCards(SourceRequest sourceRequest,
        string cardNumberFrom, string cardNumberTo, int amountInCents, string transactionID,
        bool stopTheFromCard, int stopReasonCodeID, // optional- StopCardReason
        out int transferredAmountInCents,
        out string serverTransactionID, out string errorMessage)
    {
      var methodName = "TransferFundsBetweenCards";
      var startDT = DateTime.Now;
      var sourceRequestString = JsonConvert.SerializeObject(new
      {
        sourceRequest,
        cardNumberFrom,
        cardNumberTo,
        amountInCents,
        transactionID,
        stopTheFromCard,
        stopReasonCodeID
      });
      _log.Information("{MethodName} starting: {@Request}", methodName, sourceRequest);

      errorMessage = string.Empty;
      serverTransactionID = string.Empty;
      transferredAmountInCents = 0;
      string serverLoadTransactionId = null;
      string serverDeductTransactionID = null;
      var multiStepTransaction = 0;

      PER_SecurityDTO swOperator = null;
      COR_AppUsageDTO application = null;
      BRN_BranchDTO branch = null;
      try
      {
        #region Check parameters
        if (!WCFUtils.CheckSourceRequest(sourceRequest, out application, out swOperator, out branch, out errorMessage))
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.TransferCardFunds, new Exception(errorMessage), null, null, sourceRequest);
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
                
        string terminalIDFrom;
        string profileNumberFrom;
        string terminalPasswordFrom;

        string terminalIDTo;
        string profileNumberTo;
        string terminalPasswordTo;

        string terminalID;
        string profileNumber;
        string terminalPassword;
        var checkSourceRequest = WCFUtils.GetTutukaFromRequest(sourceRequest, out terminalID, out profileNumber, out terminalPassword);
        if (checkSourceRequest != WCFUtils.CheckSourceRequestResult.ParamsOK)
        {
          errorMessage = WCFUtils.SourceRequestErrorString(checkSourceRequest);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.TransferCardFunds, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        if (stopTheFromCard)
        {
          if (!Enum.IsDefined(typeof(StopReasonCodes), stopReasonCodeID))
          {
            errorMessage = string.Format("Invalid stopReasonCodeID: '{0}'", stopReasonCodeID);
            Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.TransferCardFunds, new Exception(errorMessage), null, null, sourceRequest);
            return (int)General.WCFCallResult.BadParams;
          }
        }

        #region Check the 'From' card
        // Ensure we have the card in our DB and is linked
        var cardFrom = WCFUtils.CheckCard(cardNumberFrom, null, null /* !!! */, null, false, out errorMessage);
        if (cardFrom == null)
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.TransferCardFunds, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }
        /* !!!!!!!!!!!! */
        if (cardFrom.NuCardProfile != null)
        {
          terminalIDFrom = cardFrom.NuCardProfile.TerminalId;
          profileNumberFrom = cardFrom.NuCardProfile.ProfileNum;
          terminalPasswordFrom = cardFrom.NuCardProfile.Password;
        }
        else
        {
          terminalIDFrom = branch.DefaultNuCardProfile.TerminalId;
          profileNumberFrom = branch.DefaultNuCardProfile.ProfileNum;
          terminalPasswordFrom = branch.DefaultNuCardProfile.Password;
        }        
        #endregion

        #region Check the 'To' card
        // Ensure we have the card in our DB
        var cardTo = WCFUtils.CheckCard(cardNumberTo, new List<NuCard.NuCardStatus>() { NuCard.NuCardStatus.USE, NuCard.NuCardStatus.ISSUE,
          NuCard.NuCardStatus.Active, NuCard.NuCardStatus.InStock, NuCard.NuCardStatus.Linked, NuCard.NuCardStatus.NotSet}, null, null, true, out errorMessage);
        if (cardTo == null)
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.TransferCardFunds, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }
        #endregion

        #region Link destination card
        if (cardTo.NuCardProfile == null)
        {
          var linkTransId = string.Format("{0}/{1}", transactionID, ++multiStepTransaction);
          var linkResult = LinkCard_Impl.LinkCard(sourceRequest, cardNumberTo, linkTransId, 
            out serverTransactionID, out errorMessage);
          if (linkResult != (int)General.WCFCallResult.OK)
          {
            return linkResult;
          }

          // Reload
          cardTo = NuCardDb.FindCard(cardNumberTo);
        }
        #endregion

        #region Allocate destination card
        if (cardTo.AllocatedPerson == null)
        {
          var cellNum = PersonData.GetCellNumForPerson(cardFrom.AllocatedPerson.PersonId);
          var allocateTransId = string.Format("{0}/{1}", transactionID, ++multiStepTransaction);
          var allocateResult = AllocateCard_Impl.AllocateCard(sourceRequest, cardNumberTo, cardFrom.AllocatedPerson.Firstname, cardFrom.AllocatedPerson.Lastname,
            cardFrom.AllocatedPerson.IdNum, cellNum, allocateTransId, out serverTransactionID, out errorMessage);

          if (allocateResult != (int)General.WCFCallResult.OK)
          {
            return allocateResult;
          }

          // Reload
          cardTo = NuCardDb.FindCard(cardNumberTo);
        }
        terminalIDTo = cardTo.NuCardProfile.TerminalId;
        profileNumberTo = cardTo.NuCardProfile.ProfileNum;
        terminalPasswordTo = cardTo.NuCardProfile.Password;
        #endregion

        if (amountInCents > Consts.MAX_LOAD_AMOUNT_IN_CENTS)
        {
          errorMessage = string.Format("Invalid amount requested: {0:c}", (Decimal)amountInCents / 100M);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.TransferCardFunds, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        // Cannot be between the same card...
        if (cardFrom.NuCardId == cardTo.NuCardId)
        {
          errorMessage = "You cannot transfer to the same card";
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.TransferCardFunds, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        if (!Utils.TransactionIdIsUnique(transactionID))
        {
          errorMessage = string.Format("The transfer with reference '{0}' has already been successfully completed", transactionID);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.TransferCardFunds, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        #region Get card balance
        BalanceResult balanceResult;
        var balanceTransId = string.Format("{0}/{1}", transactionID, ++multiStepTransaction);
        string balanceServerTransId;
        var getCardBalance = CardBalance_Impl.CardBalance(sourceRequest, cardNumberFrom, balanceTransId, 
          out balanceResult, out balanceServerTransId, out errorMessage);
        if (getCardBalance != (int)General.WCFCallResult.OK)
        {
          return getCardBalance;
        }
        #endregion

        #region Transfer the full balance, if no amount given
        if (amountInCents <= 0 || amountInCents > balanceResult.BalanceInCents)
        {
          amountInCents = balanceResult.BalanceInCents;
        }

        if (amountInCents == 0)
        {
          errorMessage = string.Format("Insufficient funds on the 'From' card: card balance: {0:c}", (Decimal)amountInCents / 100M);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.TransferCardFunds, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }
        #endregion

        #endregion

        #region XML-RPC / result
        string deductTransId = null;
        string loadTransId = null;
        // If cards are in same profile, can use Transfer, else have to DeductCardLoadProfile/DeductProfileLoadCard...
        if (profileNumberFrom == profileNumberTo)
        {
          var xmlRequest = new TransferFunds_Input()
          {
            cardNumberFrom = cardNumberFrom,
            cardNumberTo = cardNumberTo,
            profileNumber = profileNumberFrom,
            terminalID = terminalIDFrom,
            transactionDate = DateTime.Now,
            transactionID = transactionID,
            requestAmount = amountInCents
          };

          string xmlSent;
          string xmlRecv;
          string error;
          var xmlResult = NuCardXMLRPCUtils.TransferFunds(Atlas.Server.NuCard.Utils.CachedValues.TutukaEndpoint, 
            terminalPasswordFrom, xmlRequest, out xmlSent, out xmlRecv, out error);
          var endDT = DateTime.Now;
          serverTransactionID = xmlResult.serverTransactionID;

          NuCardDb.LogAdminRequest(sourceRequestString, application, startDT, endDT, NuCard.AdminRequestType.TransferCardFunds,
            xmlResult.resultCode, xmlResult.requestAmount != null ? (Decimal)xmlResult.requestAmount.Value / 100M : 0, xmlRequest.cardNumberFrom, xmlRequest.cardNumberTo,
            xmlSent, xmlRecv, xmlRequest.transactionID, xmlResult.serverTransactionID, error);
          if (xmlResult.resultCode != (int)NuCard.AdminRequestResult.Successful)
          {
            errorMessage = NuCardXMLRPCUtils.GetNuCardErrorString(xmlResult.resultCode, xmlResult.resultText);
            Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.TransferCardFunds, new Exception(errorMessage), xmlSent, xmlRecv, sourceRequest);
            return (int)General.WCFCallResult.ServiceProviderCommsError;
          }

          serverDeductTransactionID = serverTransactionID;
          serverLoadTransactionId = serverTransactionID;
          deductTransId = transactionID;
          loadTransId = transactionID;

          transferredAmountInCents = amountInCents;
        }
        else
        {
          deductTransId = string.Format("{0}/{1}", transactionID, ++multiStepTransaction);
          var result = DeductFromCardLoadProfile_Impl.DeductFromCardLoadProfile(sourceRequest, 
            cardNumberFrom, amountInCents, deductTransId,
            out transferredAmountInCents, out serverDeductTransactionID, out errorMessage);
          if (result != (int)General.WCFCallResult.OK)
          {
            return result;
          }

          loadTransId = string.Format("{0}/{1}", transactionID, ++multiStepTransaction);
          result = DeductFromProfileLoadCard_Impl.DeductFromProfileLoadCard(sourceRequest, 
            cardNumberTo, amountInCents, loadTransId,
            out serverLoadTransactionId, out errorMessage);
          if (result != (int)General.WCFCallResult.OK)
          {
            return result;
          }

          // We have two server transactions, pass back the last load server transaction id
          serverTransactionID = serverLoadTransactionId;
          var endDT = DateTime.Now;
          NuCardDb.LogAdminRequest(sourceRequestString, application, startDT, endDT, NuCard.AdminRequestType.TransferCardFunds,
            result == (int)General.WCFCallResult.OK ? (int?)NuCard.AdminRequestResult.Successful : (int?)NuCard.AdminRequestResult.NoOperation,
            (Decimal)transferredAmountInCents / 100M, cardNumberFrom, cardNumberTo, string.Empty, string.Empty, transactionID, serverTransactionID, errorMessage);
        }

        #region Stop the source card
        if (stopTheFromCard)
        {
          var stopTransId = string.Format("{0}/{1}", transactionID, ++multiStepTransaction);
          string stopTransServerId;
          int tempAmount;
          var result = StopCard_Impl.StopCard(sourceRequest, cardNumberFrom, stopReasonCodeID, stopTransId,
            out tempAmount, out stopTransServerId, out errorMessage);
          if (result != (int)General.WCFCallResult.OK)
          {
            errorMessage = String.Format("Successfully transferred funds, but error occurred trying to stop the source card: '{0}'", errorMessage);
            Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.TransferCardFunds, new Exception(errorMessage), null, null, sourceRequest);
            // Basically successful... don't pass back this particular failure......
          }
        }
        #endregion

        // NOTE: Atlas Management system will log- Fabian: 2013-12-20
        if (application.Application.AtlasApplication != General.ApplicationIdentifiers.AtlasManagement)
        {
          Utils.LogTransaction(nuCardId: cardFrom.NuCardId, serverTransactionId: serverDeductTransactionID,
            description: string.Format("Transfer between cards- source card, source transId: {0}", transactionID),
            referenceNum: deductTransId, amount: ((Decimal)transferredAmountInCents / 100M) * -1, loadDate: startDT,
            isPending: false, source: application.Application.AtlasApplication, transactionSource: NuCard.TransactionSourceType.API,
            createdByPersonId: swOperator != null && swOperator.Person != null ? swOperator.Person.PersonId : 0, createdDT: startDT);

          Utils.LogTransaction(nuCardId: cardTo.NuCardId, serverTransactionId: serverLoadTransactionId,
            description: string.Format("Transfer between cards- destination card, source transId: {0}", transactionID),
            referenceNum: loadTransId, amount: (Decimal)transferredAmountInCents / 100M, loadDate: startDT,
            isPending: false, source: application.Application.AtlasApplication, transactionSource: NuCard.TransactionSourceType.API,
            createdByPersonId: swOperator != null && swOperator.Person != null ? swOperator.Person.PersonId : 0, createdDT: startDT);          
        }

        _log.Information("{MethodName} completed successfully and transferred: {Amount}", methodName, (Decimal)transferredAmountInCents / 100M);
        return (int)General.WCFCallResult.OK;
        #endregion
      }
      catch (Exception err)
      {
        Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.TransferCardFunds, err, null, null, sourceRequest);
        errorMessage = Atlas.Server.NuCard.WCF.Implementation.Consts.ERR_SERVER_GENERIC;
        return (int)General.WCFCallResult.ServerError;
      }
    }


    #region Logging

    private static readonly ILogger _log = Log.Logger.ForContext<AtlasServer.WCF.Implementation.NuCardAdminServer>();

    #endregion
  }
}

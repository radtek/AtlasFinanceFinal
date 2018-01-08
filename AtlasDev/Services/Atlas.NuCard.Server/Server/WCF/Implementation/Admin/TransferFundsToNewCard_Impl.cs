/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012-2013 Atlas Finance (Pty) Ltd.
* 
* 
*  Description:
*  ------------------
*    Transfers funds to a new NuCard, from an old NuCard
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
  public static class TransferFundsToNewCard_Impl
  {
    /// <summary>
    /// Links new card to profile, allocates card, transfers funds to new card and then stops the old card 
    /// </summary>
    /// <param name="sourceRequest">Source request parameters</param>
    /// <param name="cardNumberFrom">Full source card number</param>
    /// <param name="cardNumberTo">Full destination card number</param>
    /// <param name="stopReasonCodeID">Stop reason code</param>
    /// <param name="firstName">New cardholder firstname</param>
    /// <param name="lastName">New cardholder surname</param>
    /// <param name="idOrPassportNumber">New cardholder id or passport</param>
    /// <param name="cellPhoneNumber">New cardholder cell phone number</param>
    /// <param name="transferredAmountInCents">Amount in cents, loaded onto destination NuCard</param>
    /// <param name="transactionID">Source reference ID</param>
    /// <param name="serverTransactionID">Server transaction ID (out)</param>
    /// <param name="errorMessage">Any error message to display to the end-user (out)</param>
    /// <returns>General.WCFCallResult enum (1- success, 0- no operation, etc.)</returns>
    public static int TransferFundsToNewCard(SourceRequest sourceRequest,
        string cardNumberFrom, string cardNumberTo, int stopReasonCodeID,
        string firstName, string lastName, string idOrPassportNumber, string cellPhoneNumber, string transactionID,
        out int transferredAmountInCents,
        out string serverTransactionID, out string errorMessage)
    {
      var methodName = "TransferFundsToNewCard";
      var startDT = DateTime.Now;
      var sourceRequestValues = new
        {
          sourceRequest,
          cardNumberFrom,
          cardNumberTo,
          stopReasonCodeID,
          firstName,
          lastName,
          idOrPassportNumber,
          cellPhoneNumber,
          transactionID,
        };
      var sourceRequestString = JsonConvert.SerializeObject(sourceRequestValues);

      _log.Information("{MethodName} starting: {@SourceRequest}", methodName, sourceRequestValues);
      errorMessage = string.Empty;
      serverTransactionID = string.Empty;
      transferredAmountInCents = 0;
      var multiStepTransaction = 0;

      PER_SecurityDTO swOperator = null;
      COR_AppUsageDTO application = null;
      BRN_BranchDTO branch = null;
      try
      {
        #region Check parameters
        if (!WCFUtils.CheckSourceRequest(sourceRequest, out application, out swOperator, out branch, out errorMessage))
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.MultistepProcess, new Exception(errorMessage), null, null, sourceRequest);
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

        string terminalID;
        string profileNumber;
        string terminalPassword;

        string terminalIDFrom;
        string profileNumberFrom;
        string terminalPasswordFrom;

        var checkSourceRequest = WCFUtils.GetTutukaFromRequest(sourceRequest, out terminalID, out profileNumber, out terminalPassword);
        if (checkSourceRequest != WCFUtils.CheckSourceRequestResult.ParamsOK)
        {
          errorMessage = WCFUtils.SourceRequestErrorString(checkSourceRequest);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.MultistepProcess, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        if (string.IsNullOrEmpty(firstName))
        {
          errorMessage = "First name not provided";
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.MultistepProcess, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }
        firstName = firstName.Trim();

        if (string.IsNullOrEmpty(lastName))
        {
          errorMessage = "Last name not provided";
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.MultistepProcess, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }
        lastName = lastName.Trim();

        if (string.IsNullOrEmpty(cellPhoneNumber) || cellPhoneNumber.Length < 10)
        {
          errorMessage = "Valid cell phone number not provided";
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.MultistepProcess, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }
        cellPhoneNumber = cellPhoneNumber.Trim();

        if (string.IsNullOrEmpty(idOrPassportNumber) || idOrPassportNumber.Length < 6)
        {
          errorMessage = "Valid ID or passport not provided";
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.MultistepProcess, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }
        idOrPassportNumber = idOrPassportNumber.Trim();

        if (!Enum.IsDefined(typeof(StopReasonCodes), stopReasonCodeID))
        {
          errorMessage = string.Format("Invalid stopReasonCodeID: '{0}'", stopReasonCodeID);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.MultistepProcess, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        #region Check source card
        cardNumberFrom = cardNumberFrom.Trim();
        var cardFrom = WCFUtils.CheckCard(cardNumberFrom, null, null, null, false, out errorMessage);
        if (cardFrom == null)
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.MultistepProcess, new Exception(errorMessage), null, null, sourceRequest);
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

        #region Check destination card
        cardNumberTo = cardNumberTo.Trim();
        var cardTo = WCFUtils.CheckCard(cardNumberTo, null, null, null, true, out errorMessage);
        if (cardTo == null)
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.MultistepProcess, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        firstName = WCFUtils.UnescapeUriString(firstName);
        if (string.IsNullOrEmpty(firstName))
        {
          errorMessage = "Please provide a first name for the card holder";
          return (int)General.WCFCallResult.BadParams;
        }
        firstName = firstName.Trim();

        lastName = WCFUtils.UnescapeUriString(lastName);
        if (string.IsNullOrEmpty(lastName))
        {
          errorMessage = "Please provide a last name for the card holder";
          return (int)General.WCFCallResult.BadParams;
        }
        lastName = lastName.Trim();

        idOrPassportNumber = WCFUtils.UnescapeUriString(idOrPassportNumber);
        if (string.IsNullOrEmpty(idOrPassportNumber) || idOrPassportNumber.Length < 5 || idOrPassportNumber.Length > 13)
        {
          errorMessage = "Invalid ID or passport- minimum 5 characters and maximum 13 characters";
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.MultistepProcess, new Exception(errorMessage), null, null, sourceRequest);

          return (int)General.WCFCallResult.BadParams;
        }
        idOrPassportNumber = idOrPassportNumber.Trim();

        cellPhoneNumber = WCFUtils.UnescapeUriString(cellPhoneNumber);
        if (string.IsNullOrEmpty(cellPhoneNumber))
        {
          errorMessage = "Invalid cell phone number";
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.MultistepProcess, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        // TODO: temporary fix until proper NuCard system in place...! Please remove, card should be unallocated!
        if (cardTo.NuCardProfile == null)
        {
          // TODO: This is a temporary fix- try allocate to this profile  - ignore error if says already linked to profile      
          var linkTransId = string.Format("{0}/{1}", transactionID, ++multiStepTransaction);
          string linkServerTransId;
          var linkCardResult = LinkCard_Impl.LinkCard(sourceRequest, cardNumberTo, linkTransId, out linkServerTransId, out errorMessage);
          if (linkCardResult != (int)General.WCFCallResult.OK)
          {
            errorMessage = "Failed to link destination card to profile!";
            return (int)General.WCFCallResult.BadParams;
          }
          cardTo = NuCardDb.FindCard(cardNumberFrom);
        }
        #endregion
        #endregion

        #region Get balance on card
        var transferInCents = 0;
        BalanceResult balanceResult;
        var balanceTransId = string.Format("{0}/{1}", transactionID, ++multiStepTransaction);
        string balanceServerTransId;
        var balanceCheck = CardBalance_Impl.CardBalance(sourceRequest, cardNumberFrom, balanceTransId, 
          out balanceResult, out balanceServerTransId, out errorMessage);
        if (balanceCheck != (int)General.WCFCallResult.OK)
        {
          errorMessage = "Failed trying to determine balance on old card";
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.MultistepProcess, new Exception(errorMessage), null, null, sourceRequest);
          return balanceCheck;
        }
        transferInCents = balanceResult.BalanceInCents;
        #endregion

        #region Allocate card to profile
        if (cardTo.NuCardProfile == null)
        {
          var linkCardTransId = string.Format("{0}/{1}", transactionID, ++multiStepTransaction);
          string linkServerTransId;
          var linkCardRequest = LinkCard_Impl.LinkCard(sourceRequest, cardNumberTo, linkCardTransId, out linkServerTransId, out errorMessage);
          if (linkCardRequest != (int)General.WCFCallResult.OK)
          {
            errorMessage = "Failed to link card to profile";
            return linkCardRequest;
          }
        }
        #endregion

        #region Allocate card to person
        var allocateTransId = string.Format("{0}/{1}", transactionID, ++multiStepTransaction);
        string allocateServerTransId;
        var allocateCardRequest = AllocateCard_Impl.AllocateCard(sourceRequest, cardNumberTo, firstName, lastName, 
          idOrPassportNumber, cellPhoneNumber, allocateTransId, out allocateServerTransId, out errorMessage);
        if (allocateCardRequest != (int)General.WCFCallResult.OK)
        {
          errorMessage = "Failed to allocate new card to person";
          return allocateCardRequest;
        }
        #endregion

        #region Transfer funds from old to new card
        if (transferInCents > 0)
        {
          //var transferBetweenTransId = string.Format("{0}/{1}", transactionID, ++multiStepTransaction);
          var loadCardRequest = TransferFundsBetweenCards_Impl.TransferFundsBetweenCards(sourceRequest, cardNumberFrom, 
            cardNumberTo, transferInCents, transactionID, false, stopReasonCodeID,
            out transferredAmountInCents, out serverTransactionID, out errorMessage);
          if (loadCardRequest != (int)General.WCFCallResult.OK)
          {
            errorMessage = "Failed to transfer funds";
            return loadCardRequest;
          }
        }
        #endregion

        #region Stop the 'from' card
        var stopCardTransId = string.Format("{0}/{1}", transactionID, ++multiStepTransaction);
        var amount = 0;
        string stopServerTransId;
        var stopCardRequest = StopCard_Impl.StopCard(sourceRequest, cardNumberFrom, stopReasonCodeID, stopCardTransId, 
          out amount, out stopServerTransId, out errorMessage);
        #endregion

        _log.Information("{MethodName} completed successfully- transferred: {Amount}", methodName, (Decimal)transferredAmountInCents / 100M);

        return (int)General.WCFCallResult.OK;
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

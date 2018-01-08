/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012-2013 Atlas Finance (Pty) Ltd.
* 
* 
*  Description:
*  ------------------
*    Allocates a card to person (+ optionally links)
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

using Atlas.NuCard.Data.Repository;
using Atlas.Domain.DTO;
using Atlas.Enumerators;
using Atlas.ThirdParty.XMLRPC.Classes;
using Atlas.ThirdParty.XMLRPC.Utils;
using Atlas.WCF.Implementation;
using Atlas.NuCard.WCF.Interface;
using System.Collections.Generic;
using Atlas.NuCard.Repository;

#endregion


namespace AtlasServer.WCF.Admin.Implementation
{
  public static class AllocateCard_Impl
  {
    /// <summary>
    /// Allocates a card to a person
    /// </summary>
    /// <param name="sourceRequest">Source request parameters</param>
    /// <param name="cardNumber">Full card number</param>
    /// <param name="firstName">First name of person to link to card</param>
    /// <param name="lastName">Last name/surname of person to link to card</param>
    /// <param name="idOrPassportNumber">ID or passport of person to link to card</param>
    /// <param name="cellPhoneNumber">Cell phone number of person to link to card</param>
    /// <param name="transactionID">Source reference ID</param>
    /// <param name="serverTransactionID">Server transaction ID (out)</param>
    /// <param name="errorMessage">Any error message to display to the end-user (out)</param>
    /// <returns>General.WCFCallResult enum (1- success, 0- no operation, etc.)</returns>
    public static int AllocateCard(SourceRequest sourceRequest,
        string cardNumber, string firstName, string lastName, string idOrPassportNumber,
        string cellPhoneNumber, string transactionID,
        out string serverTransactionID, out string errorMessage)
    {
      var methodName = "AllocateCard";
      var startDT = DateTime.Now;
      var sourceRequestString = JsonConvert.SerializeObject(new
      {
        sourceRequest,
        cardNumber,
        firstName,
        lastName,
        idOrPassportNumber,
        cellPhoneNumber,
        transactionID
      });
      _log.Information("{MethodName} starting: {@Request}", methodName, sourceRequest);

      errorMessage = string.Empty;
      serverTransactionID = string.Empty;

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
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.AllocateCard, new Exception(errorMessage), null, null, sourceRequest);
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
        var checkSourceRequest = WCFUtils.GetTutukaFromRequest(sourceRequest, out terminalID, out profileNumber, out terminalPassword);
        if (checkSourceRequest != WCFUtils.CheckSourceRequestResult.ParamsOK)
        {
          errorMessage = WCFUtils.SourceRequestErrorString(checkSourceRequest);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.AllocateCard, new Exception(errorMessage), xmlSent, xmlRecv, sourceRequest);

          return (int)General.WCFCallResult.BadParams;
        }

        firstName = WCFUtils.UnescapeUriString(firstName);
        if (string.IsNullOrEmpty(firstName))
        {
          errorMessage = "First name missing for the card holder";
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.AllocateCard, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        lastName = WCFUtils.UnescapeUriString(lastName);
        if (string.IsNullOrEmpty(lastName))
        {
          errorMessage = "Last name missing for the card holder";
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.AllocateCard, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        idOrPassportNumber = WCFUtils.UnescapeUriString(idOrPassportNumber);
        if (string.IsNullOrEmpty(idOrPassportNumber) || idOrPassportNumber.Length < 5)
        {
          errorMessage = "Invalid ID or passport- minimum 5 characters";
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.AllocateCard, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        cellPhoneNumber = WCFUtils.UnescapeUriString(cellPhoneNumber);
        if (string.IsNullOrEmpty(cellPhoneNumber))
        {
          errorMessage = "Invalid cell phone number";
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.AllocateCard, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        if (string.IsNullOrEmpty(transactionID))
        {
          errorMessage = "Missing transaction ID";
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.AllocateCard, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        var card = WCFUtils.CheckCard(cardNumber, new List<NuCard.NuCardStatus>() { NuCard.NuCardStatus.InStock, NuCard.NuCardStatus.Linked, 
          NuCard.NuCardStatus.ISSUE, NuCard.NuCardStatus.USE, NuCard.NuCardStatus.Active, NuCard.NuCardStatus.NotSet }, null, null, true, out errorMessage);
        if (card == null)
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.AllocateCard, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        #region If the NuCard profile is blank, try link to the default profile        
        if (card.NuCardProfile == null)
        {
          var linkTransId = string.Format("{0}/1", transactionID);
          var tempErrMsg = string.Empty;
          string linkServerTransId;
          var linkCardResult = LinkCard_Impl.LinkCard(sourceRequest, cardNumber, linkTransId, out linkServerTransId, out tempErrMsg);
          // We can ignore errors that card is already linked to this profile
          if (linkCardResult != (int)General.WCFCallResult.OK)
          {
            errorMessage = "Tutuka error- Failed to link card to branch default profile: " + tempErrMsg;
            Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.AllocateCard, new Exception(errorMessage), null, null, sourceRequest);
            return (int)General.WCFCallResult.ServiceProviderCommsError;
          }

          card = NuCardDb.FindCard(cardNumber);
        }
        #endregion

        terminalID = card.NuCardProfile.TerminalId;
        profileNumber = card.NuCardProfile.ProfileNum;
        terminalPassword = card.NuCardProfile.Password;

        // Automatically create person only if ASS, else error
        var personDto = PersonData.FindPersonDTO(idOrPassportNumber, firstName, lastName, cellPhoneNumber, branch.BranchId, swOperator, 
          application.Application.AtlasApplication == General.ApplicationIdentifiers.ASS ? General.Host.ASS : General.Host.LAS,
          application.Application.AtlasApplication == General.ApplicationIdentifiers.ASS);
        if (personDto == null)
        {
          errorMessage = "Atlas error- Unable to locate/add the person in the Atlas database";
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.AllocateCard, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        // If card already linked...
        if (card.Status.Type == NuCard.NuCardStatus.Active || card.Status.Type == NuCard.NuCardStatus.USE || card.Status.Type == NuCard.NuCardStatus.ISSUE)
        {
          if (card.AllocatedPerson == null)
          {
            errorMessage = "Card is 'active', but not assigned to a person!";
            return (int)General.WCFCallResult.BadParams;
          }

          if (card.AllocatedPerson.PersonId != personDto.PersonId)
          {
            errorMessage = string.Format("This card is already linked to another person named: '{0} {1}'", card.AllocatedPerson.Firstname, card.AllocatedPerson.Lastname);
            Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.AllocateCard, new Exception(errorMessage), null, null, sourceRequest);
            return (int)General.WCFCallResult.BadParams;
          }

          return (int)General.WCFCallResult.OK;
        }
        #endregion

        // Check if transaction is still in progress (Tutuka/branch network time-outs)
        if (Atlas.Server.NuCard.Utils.CachedValues.TransInProgress(transactionID, TimeSpan.FromMinutes(10)))
        {
          errorMessage = string.Format("Simultaneous request: system is still processing transaction '{0}'", transactionID);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.AllocateCard, 
            new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        try
        {
          #region XML-RPC
          var xmlRequest = new AllocateCard_Input()
          {
            profileNumber = profileNumber,
            terminalID = terminalID,
            cardNumber = cardNumber,
            firstName = firstName,
            lastName = lastName,
            idOrPassportNumber = idOrPassportNumber,
            cellPhoneNumber = cellPhoneNumber,
            transactionDate = DateTime.Now,
            transactionID = transactionID
          };

          string error;
          var xmlResult = NuCardXMLRPCUtils.AllocateCard(Atlas.Server.NuCard.Utils.CachedValues.TutukaEndpoint, terminalPassword, xmlRequest,
            out xmlSent, out xmlRecv, out error);
          var endDT = DateTime.Now;
          serverTransactionID = xmlResult.serverTransactionID;

          NuCardDb.LogAdminRequest(sourceRequestString, application, startDT, endDT, NuCard.AdminRequestType.AllocateCard,
            xmlResult.resultCode, 0, xmlRequest.cardNumber, null,
            xmlSent, xmlRecv, xmlRequest.transactionID, xmlResult.serverTransactionID, error);
          #endregion

          #region Card already allocated- ignore error
          // TODO: remove hack once NuCard properly and fully deployed
          if (xmlResult.resultCode == (int)NuCard.AdminRequestResult.CardAlreadyAllocated)
          {
            errorMessage = NuCardXMLRPCUtils.GetNuCardErrorString(xmlResult.resultCode, xmlResult.resultText);
            Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.AllocateCard, new Exception(errorMessage), xmlSent, xmlRecv, sourceRequest);
            xmlResult.resultCode = (int)NuCard.AdminRequestResult.Successful;
          }
          #endregion

          #region Return the result
          if (xmlResult.resultCode != (int)NuCard.AdminRequestResult.Successful)
          {
            errorMessage = NuCardXMLRPCUtils.GetNuCardErrorString(xmlResult.resultCode, xmlResult.resultText);
            Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.AllocateCard, new Exception(errorMessage), xmlSent, xmlRecv, sourceRequest);

            return (int)General.WCFCallResult.ServiceProviderCommsError;
          }

          NuCardDb.AllocateCardToPerson(card.NuCardId, personDto.PersonId, swOperator, branch);

          _log.Information("{MethodName} completed successfully", methodName);
          return (int)General.WCFCallResult.OK;
          #endregion
        }
        finally
        {
          Atlas.Server.NuCard.Utils.CachedValues.TransCompleted(transactionID);
        }
      }
      catch (Exception err)
      {
        Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.AllocateCard, err, xmlSent, xmlRecv);
        errorMessage = Atlas.Server.NuCard.WCF.Implementation.Consts.ERR_SERVER_GENERIC;
        return (int)General.WCFCallResult.ServerError;
      }
    }


    #region Logging

    private static readonly ILogger _log = Log.Logger.ForContext<AtlasServer.WCF.Implementation.NuCardAdminServer>();

    #endregion

  }
}

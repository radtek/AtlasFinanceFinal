/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012-2013 Atlas Finance (Pty) Ltd.
* 
* 
*  Description:
*  ------------------
*    De-links card from its current profile
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
using System.Collections.Generic;
using System.Linq;

using Serilog;
using Newtonsoft.Json;

using DevExpress.Xpo;

using Atlas.NuCard.WCF.Interface;
using Atlas.Domain.DTO.Nucard;
using Atlas.Domain.Model;
using Atlas.Domain.DTO;
using Atlas.Enumerators;
using Atlas.ThirdParty.XMLRPC.Classes;
using Atlas.ThirdParty.XMLRPC.Utils;
using Atlas.WCF.Implementation;
using Atlas.Server.NuCard.Utils;
using Atlas.NuCard.Repository;

#endregion


namespace AtlasServer.WCF.Admin.Implementation
{
  public static class DeLinkCard_Impl
  {
    /// <summary>
    /// De-links a card from its current profile. If not linked to a profile, returns true
    /// </summary>
    /// <param name="sourceRequest">Source request parameters</param>
    /// <param name="cardNumber">Full card number to be de-linked</param>
    /// <param name="transactionID">Source reference ID</param>
    /// <param name="serverTransactionID">Server transaction ID (out)</param>
    /// <param name="errorMessage">Any error message to display to the end-user (out)</param>
    /// <returns>General.WCFCallResult enum (1- success, 0- no operation, etc.)</returns>
    public static int DeLinkCard(SourceRequest sourceRequest, string cardNumber, string transactionID,
      out string serverTransactionID, out string errorMessage)
    {
      var methodName = "DeLinkCard";
      var startDT = DateTime.Now;
      var sourceRequestString = JsonConvert.SerializeObject(new { sourceRequest, cardNumber, transactionID });
      _log.Information("{MethodName} starting: {@Request}, Card: {CardNumber}, TransactionId: {TransactionID}", 
        methodName, sourceRequest, cardNumber, transactionID);

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
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.DeLinkCard, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        string terminalID;
        string profileNumber;
        string terminalPassword;
        var checkSourceRequest = WCFUtils.GetTutukaFromRequest(sourceRequest, out terminalID, out profileNumber, out terminalPassword);
        if (checkSourceRequest != WCFUtils.CheckSourceRequestResult.ParamsOK)
        {
          errorMessage = WCFUtils.SourceRequestErrorString(checkSourceRequest);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.DeLinkCard, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        if (string.IsNullOrEmpty(transactionID))
        {
          errorMessage = "No 'transactionID' given";
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.DeLinkCard, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }
        transactionID = transactionID.Trim();
        var card = WCFUtils.CheckCard(cardNumber, null, null /* !!!!! */, null, true, out errorMessage);
        if (card == null)
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.DeLinkCard, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }
        if (card.Status.Type == NuCard.NuCardStatus.InTransit)
        {
          errorMessage = "The card is still specified as 'stock in transit'- please receipt this card into your branch stock before proceeding";
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.DeLinkCard, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        // Not linked to a profile? Figure out status/profile...
        if (card.NuCardProfile == null)
        {
          List<NUC_NuCardProfileDTO> profiles;
          using(var unitOfWork = new UnitOfWork())
          {
            profiles = AutoMapper.Mapper.Map<List<NUC_NuCardProfileDTO>>(
              unitOfWork.Query<NUC_NuCardProfile>()
              .Where(s => s.ProfileNum != NuCardConsts.NUCARD_PROFILENUM_UNDETERMINED));
          }

          var requestId = 1;
          var foundProfile = false;
          foreach (var profile in profiles)
          {
            CardStatus cardStatus;
            string serverTempTransId;
            string tempTransactionId = string.Format("{0}/{1}", transactionID, requestId++);
            var getCardStatus = GetCardStatus_Impl.GetCardStatus(sourceRequest, cardNumber, tempTransactionId, out cardStatus, out serverTempTransId, out errorMessage);

            if (getCardStatus == (int)General.WCFCallResult.OK) 
            {
              if (cardStatus.Cancelled)
              {
                errorMessage = "Card is not in a usable state- cancelled";               
              }
              else if (cardStatus.Expired)
              {
                errorMessage = "Card is not in a usable state- expired";                
              }
              else  if (cardStatus.Lost)              
              {
                errorMessage = "Card is not in a usable state- lost";                
              }              
              else if (cardStatus.PINBlocked)
              {
                errorMessage = "Card is not in a usable state- PIN blocked";                
              }
              else if (cardStatus.Retired)
              {
                errorMessage = "Card is not in a usable state- retired";                
              }
              else if (cardStatus.Stolen)
              {
                errorMessage = "Card is not in a usable state- stolen";                
              }
              else if (cardStatus.Stopped)
              {
                errorMessage = "Card is not in a usable state- stopped";                
              }
              else if (!cardStatus.Valid)
              {
                errorMessage = "Card is not in a usable state- card not valid";                
              }

              if (!string.IsNullOrEmpty(errorMessage))
              {
                Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.DeLinkCard, new Exception(errorMessage), null, null, sourceRequest);                
              }

              terminalID = profile.TerminalId;
              profileNumber = profile.ProfileNum;
              terminalPassword = profile.Password;
              foundProfile = true;

              break;
            }            
          }

          if (!foundProfile)
          {
            errorMessage = "Unable to determine card's current profile- bad card number or card not currently linked";
            Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.DeLinkCard, new Exception(errorMessage), null, null, sourceRequest);
            return (int)General.WCFCallResult.BadParams;
          }
        }
        #endregion

        #region Perform XML request
        var error = (string)null;
        var xmlRequest = new DeLinkCard_Input()
        {
          profileNumber = profileNumber,
          terminalID = terminalID,
          cardNumber = cardNumber,
          transactionDate = DateTime.Now,
          transactionID = transactionID
        };
        var xmlResult = NuCardXMLRPCUtils.DeLinkCard(Atlas.Server.NuCard.Utils.CachedValues.TutukaEndpoint, terminalPassword, xmlRequest, out xmlSent, out xmlRecv, out error);
        var endDT = DateTime.Now;

        serverTransactionID = xmlResult.serverTransactionID;

        NuCardDb.LogAdminRequest(sourceRequestString, application, startDT, endDT, NuCard.AdminRequestType.DeLinkCard,
          xmlResult.resultCode, 0, xmlRequest.cardNumber, null,
          xmlSent, xmlRecv, xmlRequest.transactionID, xmlResult.serverTransactionID, error);

        #endregion

        #region 'Card already linked'- update status to 'linked'
        // TODO: Remove this hack when this NuCard system has been fully deployed!!
        if (xmlResult.resultCode == (int)NuCard.AdminRequestResult.CardAlreadyLinked)
        {
          errorMessage = NuCardXMLRPCUtils.GetNuCardErrorString(xmlResult.resultCode, xmlResult.resultText);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.DeLinkCard, new Exception(errorMessage), xmlSent, xmlRecv, sourceRequest);
          xmlResult.resultCode = (int)NuCard.AdminRequestResult.Successful;
        }
        #endregion

        #region Return result
        if (xmlResult.resultCode != (int)NuCard.AdminRequestResult.Successful)
        {
          errorMessage = NuCardXMLRPCUtils.GetNuCardErrorString(xmlResult.resultCode, xmlResult.resultText);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.DeLinkCard, new Exception(errorMessage), xmlSent, xmlRecv, sourceRequest);
          return (int)General.WCFCallResult.ServiceProviderCommsError;
        }

        NuCardDb.UpdateCardStatus(cardNumber, profileNumber, sourceRequest.BranchCode, NuCard.NuCardStatus.Linked, swOperator);

        _log.Information("{MethodName} completed successfully", methodName);
        return (int)General.WCFCallResult.OK;

        #endregion
      }
      catch (Exception err)
      {
        Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.DeLinkCard, err, xmlSent, xmlRecv, sourceRequest);
        errorMessage = Atlas.Server.NuCard.WCF.Implementation.Consts.ERR_SERVER_GENERIC;
        return (int)General.WCFCallResult.ServerError;
      }
    }


    #region Logging

    private static readonly ILogger _log = Log.Logger.ForContext<AtlasServer.WCF.Implementation.NuCardAdminServer>();

    #endregion
  }
}

/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012-2013 Atlas Finance (Pty) Ltd.
* 
* 
*  Description:
*  ------------------
*    Links a card to a specific profile
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
  public static class LinkCard_Impl
  {
    /// <summary>
    /// Links card to a profile- if already linked to specified profile, will prematurely exit with success
    /// </summary>
    /// <param name="sourceRequest">Source request parameters</param>
    /// <param name="cardNumber">Full card number to be linked to branch's profile</param>
    /// <param name="transactionID">Source reference ID</param>
    /// <param name="serverTransactionID">Server transaction ID (out)</param>
    /// <param name="errorMessage">Any error message to display to the end-user (out)</param>
    /// <returns>General.WCFCallResult enum (1- success, 0- no operation, etc.)</returns>
    public static int LinkCard(SourceRequest sourceRequest, string cardNumber, string transactionID,
      out string serverTransactionID, out string errorMessage)
    {
      var methodName = "LinkCard";
      var startDT = DateTime.Now;
      var sourceRequestString = JsonConvert.SerializeObject(new { sourceRequest, cardNumber, transactionID });
      _log.Information("{MethodName} starting- {SourceRequest}, Card: {CardNumber}, TransId: {TransactionID}", methodName, sourceRequest, cardNumber, transactionID);

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
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.LinkCard, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        string terminalID;
        string profileNumber;
        string terminalPassword;
        var checkSourceRequest = WCFUtils.GetTutukaFromRequest(sourceRequest, out terminalID, out profileNumber, out terminalPassword);
        if (checkSourceRequest != WCFUtils.CheckSourceRequestResult.ParamsOK)
        {
          errorMessage = WCFUtils.SourceRequestErrorString(checkSourceRequest);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.LinkCard, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        if (string.IsNullOrEmpty(transactionID))
        {
          errorMessage = "No 'transactionID' given";
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.LinkCard, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }
        transactionID = transactionID.Trim();
        var card = WCFUtils.CheckCard(cardNumber, null, null /* !!!!! */, null, true, out errorMessage);
        if (card == null)
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.LinkCard, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }
        if (card.Status.Type == NuCard.NuCardStatus.InTransit)
        {
          errorMessage = "The card is still specified as 'stock in transit'- please receipt this card into your branch stock before proceeding";
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.LinkCard, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        // Already linked? Ignore
        if (card.Status.Type == NuCard.NuCardStatus.Linked)
        {
          if (card.NuCardProfile == null)
          {
            errorMessage = "The specified NuCard is marked as linked to a profile, but no profile has been recorded- please use another card";
            return (int)General.WCFCallResult.BadParams;
          }

          if (card.NuCardProfile.NuCardProfileId != branch.DefaultNuCardProfile.NuCardProfileId)
          {
            errorMessage = "The specified NuCard is already linked to a different profile";
            return (int)General.WCFCallResult.BadParams;
          }

          return (int)General.WCFCallResult.OK;
        }

        if (card.Status.Type != NuCard.NuCardStatus.InStock)
        {
          errorMessage = string.Format("The specified NuCard cannot be linked while in its current status: '{0}'. " +
            "Please have Atlas operations correct the status of this card.", card.Status.Description);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.LinkCard, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }
        #endregion

        #region Perform XML request
        var error = (string)null;
        var xmlRequest = new LinkCard_Input()
        {
          profileNumber = profileNumber,
          terminalID = terminalID,
          cardNumber = cardNumber,
          transactionDate = DateTime.Now,
          transactionID = transactionID
        };
        var xmlResult = NuCardXMLRPCUtils.LinkCard(Atlas.Server.NuCard.Utils.CachedValues.TutukaEndpoint, terminalPassword, xmlRequest, out xmlSent, out xmlRecv, out error);
        var endDT = DateTime.Now;

        serverTransactionID = xmlResult.serverTransactionID;

        NuCardDb.LogAdminRequest(sourceRequestString, application, startDT, endDT, NuCard.AdminRequestType.LinkCard,
          xmlResult.resultCode, 0, xmlRequest.cardNumber, null,
          xmlSent, xmlRecv, xmlRequest.transactionID, xmlResult.serverTransactionID, error);

        #endregion

        #region 'Card already linked'- update status to 'linked'
        // TODO: Remove this hack when this NuCard system has been fully deployed!!
        if (xmlResult.resultCode == (int)NuCard.AdminRequestResult.CardAlreadyLinked)
        {
          errorMessage = NuCardXMLRPCUtils.GetNuCardErrorString(xmlResult.resultCode, xmlResult.resultText);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.LinkCard, new Exception(errorMessage), null, null, sourceRequest);
          xmlResult.resultCode = (int)NuCard.AdminRequestResult.Successful;
        }
        #endregion

        #region Return result
        if (xmlResult.resultCode != (int)NuCard.AdminRequestResult.Successful)
        {
          errorMessage = NuCardXMLRPCUtils.GetNuCardErrorString(xmlResult.resultCode, xmlResult.resultText);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.LinkCard, new Exception(errorMessage), xmlSent, xmlRecv, sourceRequest);
          return (int)General.WCFCallResult.ServiceProviderCommsError;
        }

        NuCardDb.UpdateCardStatus(cardNumber, profileNumber, sourceRequest.BranchCode, NuCard.NuCardStatus.Linked, swOperator);

        _log.Information("{MethodName} completed successfully", methodName);
        return (int)General.WCFCallResult.OK;

        #endregion
      }
      catch (Exception err)
      {
        Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.LinkCard, err, xmlSent, xmlRecv, sourceRequest);
        errorMessage = Atlas.Server.NuCard.WCF.Implementation.Consts.ERR_SERVER_GENERIC;
        return (int)General.WCFCallResult.ServerError;
      }
    }


    #region Logging

    private static readonly ILogger _log = Log.Logger.ForContext<AtlasServer.WCF.Implementation.NuCardAdminServer>();

    #endregion

  }
}

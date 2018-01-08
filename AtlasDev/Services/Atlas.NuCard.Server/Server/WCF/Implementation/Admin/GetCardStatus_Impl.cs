/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012-2013 Atlas Finance (Pty) Ltd.
* 
* 
*  Description:
*  ------------------
*    Gets the status of a NuCard from Tutuka
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

using Newtonsoft.Json;
using Serilog;

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
  public static class GetCardStatus_Impl
  {
    /// <summary>
    /// Get status of card
    /// </summary>
    /// <param name="sourceRequest">Source request parameters</param>
    /// <param name="cardNumber"></param>
    /// <param name="serverTransactionID"></param>
    /// <param name="errorMessage">Any error message to display to the end-user (out)</param>
    /// <returns>General.WCFCallResult enum</returns>
    public static int GetCardStatus(SourceRequest sourceRequest,
      string cardNumber, string transactionID,
      out CardStatus cardStatus,
      out string serverTransactionID, out string errorMessage)
    {
      var methodName = "GetCardStatus";
      var startDT = DateTime.Now;
      var sourceRequestString = JsonConvert.SerializeObject(new { sourceRequest, cardNumber, transactionID });
      _log.Information("{MethodName} starting: {@Request}", methodName, sourceRequest);

      errorMessage = string.Empty;
      serverTransactionID = string.Empty;
      cardStatus = new CardStatus();

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
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.CardStatement, new Exception(errorMessage), null, null, sourceRequest);
          errorMessage = WCFUtils.SourceRequestErrorString(checkSourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        var card = WCFUtils.CheckCard(cardNumber, null, true, null, false, out errorMessage);
        if (card == null)
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.CardStatement, new Exception(errorMessage), null, null, sourceRequest);
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
        var xmlRequest = new Status_Input()
        {
          cardNumber = cardNumber,
          profileNumber = profileNumber,
          terminalID = terminalID,
          transactionDate = DateTime.Now,
          transactionID = transactionID
        };

        var xmlResult = NuCardXMLRPCUtils.Status(Atlas.Server.NuCard.Utils.CachedValues.TutukaEndpoint, terminalPassword, xmlRequest, 
          out xmlSent, out xmlRecv, out errorMessage);
        var endDT = DateTime.Now;
        serverTransactionID = xmlResult.serverTransactionID;

        NuCardDb.LogAdminRequest(sourceRequestString, application, startDT, endDT, NuCard.AdminRequestType.CardStatus,
          xmlResult.resultCode, 0, xmlRequest.cardNumber, string.Empty,
          xmlSent, xmlRecv, xmlRequest.transactionID, xmlResult.serverTransactionID, errorMessage);

        #endregion

        #region Return result
        if (xmlResult.resultCode != (int)NuCard.AdminRequestResult.Successful)
        {
          errorMessage = NuCardXMLRPCUtils.GetNuCardErrorString(xmlResult.resultCode, xmlResult.resultText);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.CardStatus, new Exception(errorMessage), xmlSent, xmlRecv, sourceRequest);
          return (int)General.WCFCallResult.ServiceProviderCommsError;
        }

        cardStatus.Activated = Boolean.Parse(xmlResult.activated);
        cardStatus.Cancelled = Boolean.Parse(xmlResult.cancelled);
        cardStatus.Empty = Boolean.Parse(xmlResult.empty);
        cardStatus.Expired = Boolean.Parse(xmlResult.expired);
        cardStatus.Loaded = Boolean.Parse(xmlResult.loaded);
        cardStatus.Lost = Boolean.Parse(xmlResult.lost);
        cardStatus.PINBlocked = Boolean.Parse(xmlResult.pinBlocked);
        cardStatus.Redeemed = Boolean.Parse(xmlResult.redeemed);
        cardStatus.Retired = Boolean.Parse(xmlResult.retired);
        cardStatus.Stolen = Boolean.Parse(xmlResult.stolen);
        cardStatus.Stopped = Boolean.Parse(xmlResult.stopped);
        cardStatus.Valid = Boolean.Parse(xmlResult.valid);

        _log.Information("{MethodName} completed successfully with result: {@CardStatus}", methodName, cardStatus);
        return (int)General.WCFCallResult.OK;

        #endregion
      }
      catch (Exception err)
      {
        Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.CardStatus, err, xmlSent, xmlRecv, sourceRequest);
        errorMessage = Atlas.Server.NuCard.WCF.Implementation.Consts.ERR_SERVER_GENERIC;
        return (int)General.WCFCallResult.ServerError;
      }
    }


    #region Logging

    private static readonly ILogger _log = Log.Logger.ForContext<AtlasServer.WCF.Implementation.NuCardAdminServer>();

    #endregion

  }
}

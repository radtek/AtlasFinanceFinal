/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012-2013 Atlas Finance (Pty) Ltd.
* 
* 
*  Description:
*  ------------------
*    Resets a cards PIN number
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
  public static class ResetPin_Impl
  {
    /// <summary>
    /// Request PIN reset for a card
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <param name="cardNumber"></param>
    /// <param name="serverTransactionID"></param>
    /// <param name="errorMessage"></param>
    /// <returns>General.WCFCallResult enum</returns>
    public static int ResetPin(SourceRequest sourceRequest, string cardNumber,
      out string serverTransactionID, out string errorMessage)
    {
      var methodName = "ResetPin";
      var startDT = DateTime.Now;
      errorMessage = string.Empty;
      serverTransactionID = string.Empty;
      var sourceRequestString = JsonConvert.SerializeObject(new { sourceRequest, cardNumber });
      _log.Information("{MethodName} starting: {@Request}", methodName, sourceRequest);

      PER_SecurityDTO swOperator = null;
      COR_AppUsageDTO application = null;
      BRN_BranchDTO branch = null;
      try
      {
        #region Check parameters
        if (!WCFUtils.CheckSourceRequest(sourceRequest, out application, out swOperator, out branch, out errorMessage))
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.ResetPin, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        string terminalID;
        string profileNumber;
        string terminalPassword;
        var checkSourceRequest = WCFUtils.GetTutukaFromRequest(sourceRequest, out terminalID, out profileNumber, out terminalPassword);
        if (checkSourceRequest != WCFUtils.CheckSourceRequestResult.ParamsOK)
        {
          errorMessage = WCFUtils.SourceRequestErrorString(checkSourceRequest);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.ResetPin, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        #region Check source card
        cardNumber = cardNumber.Trim();
        var card = WCFUtils.CheckCard(cardNumber, null, true, null, false, out errorMessage);
        if (card == null)
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.ResetPin, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }
        #endregion
        #endregion

        #region XML request
        var transactionID = Guid.NewGuid().ToString();

        var xmlRequest = new ResetPin_Input()
        {
          cardNumber = card.CardNum,

          profileNumber = card.NuCardProfile.ProfileNum,
          terminalID = card.NuCardProfile.TerminalId,

          transactionID = transactionID,
          transactionDate = DateTime.Now
        };

        string xmlSent;
        string xmlRecv;
        var xmlResult = NuCardXMLRPCUtils.ResetPin(Atlas.Server.NuCard.Utils.CachedValues.TutukaEndpoint, terminalPassword, xmlRequest, out xmlSent, out xmlRecv, out errorMessage);
        var endDT = DateTime.Now;
        serverTransactionID = xmlResult.serverTransactionID;

        NuCardDb.LogAdminRequest(sourceRequestString, application, startDT, endDT, NuCard.AdminRequestType.ResetPin,
          xmlResult.resultCode, 0, xmlRequest.cardNumber, string.Empty,
          xmlSent, xmlRecv, xmlRequest.transactionID, xmlResult.serverTransactionID, errorMessage);
        #endregion

        #region Return result
        if (xmlResult.resultCode != (int)NuCard.AdminRequestResult.Successful)
        {
          errorMessage = NuCardXMLRPCUtils.GetNuCardErrorString(xmlResult.resultCode, xmlResult.resultText);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.ResetPin, new Exception(errorMessage), xmlSent, xmlRecv, sourceRequest);
          return (int)General.WCFCallResult.ServiceProviderCommsError;
        }

        _log.Information("{MethodName} completed successfully", methodName);
        return (int)General.WCFCallResult.OK;
        #endregion
      }
      catch (Exception err)
      {
        Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.ResetPin, err, null, null, sourceRequest);
        errorMessage = Atlas.Server.NuCard.WCF.Implementation.Consts.ERR_SERVER_GENERIC;
        return (int)General.WCFCallResult.ServerError;
      }
    }
    

    #region Logging

    private static readonly ILogger _log = Log.Logger.ForContext<AtlasServer.WCF.Implementation.NuCardAdminServer>();

    #endregion
  }

}

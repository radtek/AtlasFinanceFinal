/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012-2013 Atlas Finance (Pty) Ltd.
* 
* 
*  Description:
*  ------------------
*    Attempts to 'PING' NuCard, using a random active card, to check comms/profile/etc. working
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
  public static class TryPingNuCard_Impl
  {
    /// <summary>
    /// Determines if this server can contact the Tutuka server, using the default 
    /// profile for the branch
    /// </summary>
    /// <param name="sourceRequest">Source request parameters</param>
    /// <param name="errorMessage">Any error message to display to the end-user (out)</param>
    /// <returns>General.WCFCallResult enum (1- success, 0- no operation, etc.)</returns>
    public static int TryPingNuCard(SourceRequest sourceRequest, out string errorMessage)
    {
      var methodName = "TryPingNuCard";
      var startDT = DateTime.Now;
      var sourceRequestString = JsonConvert.SerializeObject(sourceRequest);
      _log.Information("{MethodName} starting: {@Request}", methodName, sourceRequest);

      errorMessage = string.Empty;

      PER_SecurityDTO swOperator = null;
      COR_AppUsageDTO application = null;
      BRN_BranchDTO branch = null;
      try
      {
        string terminalID;
        string profileNumber;
        string terminalPassword;

        #region Check parameters
        if (!WCFUtils.CheckSourceRequest(sourceRequest, out application, out swOperator, out branch, out errorMessage))
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.NotSet, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        var checkSourceRequest = WCFUtils.GetTutukaFromRequest(sourceRequest,
          out terminalID, out profileNumber, out terminalPassword);
        if (checkSourceRequest != WCFUtils.CheckSourceRequestResult.ParamsOK)
        {
          errorMessage = WCFUtils.SourceRequestErrorString(checkSourceRequest);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.NotSet, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }
        #endregion

        // Get a random, active card for this branch
        var randomCard = NuCardDb.GetActiveCardForBranch(sourceRequest.BranchCode);
        if (randomCard == null)
        {
          errorMessage = string.Format("Unable to find a currently active card for branch '{0}', to perform ping", sourceRequest.BranchCode);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.NotSet, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        #region Perform the XML request
        var xmlRequest = new Balance_Input()
        {
          cardNumber = randomCard.CardNum,
          profileNumber = profileNumber,
          terminalID = terminalID,
          transactionDate = DateTime.Now,
          transactionID = Guid.NewGuid().ToString()
        };
        string xmlSent;
        string xmlRecv;
        string error;
        var xmlResult = NuCardXMLRPCUtils.Balance(Atlas.Server.NuCard.Utils.CachedValues.TutukaEndpoint, terminalPassword, xmlRequest,
          out xmlSent, out xmlRecv, out error);
        var endDT = DateTime.Now;

        NuCardDb.LogAdminRequest(sourceRequestString, application, startDT, endDT, NuCard.AdminRequestType.CardBalance,
          xmlResult.resultCode, (Decimal)xmlResult.balanceAmount / 100M, randomCard.CardNum, string.Empty, xmlSent, xmlRecv,
          xmlRequest.transactionID, xmlResult.serverTransactionID, error);
        #endregion

        if (xmlResult.resultCode != (int)NuCard.AdminRequestResult.Successful)
        {
          errorMessage = NuCardXMLRPCUtils.GetNuCardErrorString(xmlResult.resultCode, xmlResult.resultText);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.CardBalance, new Exception(errorMessage), xmlSent, xmlRecv, sourceRequest);
          return (int)General.WCFCallResult.ServiceProviderCommsError;
        }

        _log.Information("{MethodName} completed successfully", methodName);
        return (int)General.WCFCallResult.OK;
      }
      catch (Exception err)
      {
        Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.NotSet, err, null, null, sourceRequest);
        errorMessage = Atlas.Server.NuCard.WCF.Implementation.Consts.ERR_SERVER_GENERIC;
        return (int)General.WCFCallResult.ServerError;
      }
    }


    #region Logging

    private static readonly ILogger _log = Log.Logger.ForContext<AtlasServer.WCF.Implementation.NuCardAdminServer>();

    #endregion

  }

}

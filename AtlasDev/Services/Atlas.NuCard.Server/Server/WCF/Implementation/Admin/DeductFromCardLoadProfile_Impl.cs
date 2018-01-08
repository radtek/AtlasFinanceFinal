/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012-2013 Atlas Finance (Pty) Ltd.
* 
* 
*  Description:
*  ------------------
*    Remove funds from a card
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
  /// <summary>
  /// Deducts a certain amount from a card and moves back into the Atlas profile.
  /// Pass 0 or less for amountInCents, to transfer all funds from the card
  /// </summary>
  /// <param name="sourceRequest">Source request parameters</param>
  /// <param name="cardNumber">Full card number</param>
  /// <param name="amountInCents">Amount in cents, to deduct from card (pass 0 or less for all)</param>
  /// <param name="transactionID">Source reference ID</param>
  /// <param name="serverTransactionID">Server transaction ID (out)</param>
  /// <param name="errorMessage">Any error message to display to the end-user (out)</param>
  /// <returns>General.WCFCallResult enum (1- success, 0- no operation, etc.)</returns>
  public static class DeductFromCardLoadProfile_Impl
  {
    public static int DeductFromCardLoadProfile(SourceRequest sourceRequest,
        string cardNumber, int amountInCents, string transactionID,
        out int transferredAmountInCents,
        out string serverTransactionID, out string errorMessage)
    {
      var methodName = "DeductFromCardLoadProfile";
      var startDT = DateTime.Now;
      var sourceRequestString = JsonConvert.SerializeObject(new { sourceRequest, cardNumber, amountInCents, transactionID });
      _log.Information("{MethodName} starting: {@Request}", methodName, sourceRequest);

      transferredAmountInCents = 0;
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
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.DeductCardLoadProfile, new Exception(errorMessage), null, null, sourceRequest);
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
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.DeductCardLoadProfile, new Exception(errorMessage), null, null, sourceRequest);
          errorMessage = WCFUtils.SourceRequestErrorString(checkSourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        //  We should be able to pull out funds of a stopped card...        
        var card = WCFUtils.CheckCard(cardNumber, null, null /* !!!! */, null, false, out errorMessage);
        if (card == null)
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.LoadCardDeductProfile, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }
        
        // !!!!!!!!!! Temporary as card listing out-of-date !!!!!!!!
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

        if (amountInCents > Consts.MAX_LOAD_AMOUNT_IN_CENTS) 
        {
          errorMessage = string.Format("Invalid amount requested: {0:c}", (Decimal)amountInCents / 100M);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.DeductCardLoadProfile, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        if (!Utils.TransactionIdIsUnique(transactionID))
        {
          errorMessage = string.Format("The deduct from card with reference '{0}' has already been successfully completed", transactionID);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.DeductCardLoadProfile, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        #region Get card balance
        BalanceResult balanceResult;
        var multiStepProcess = 0;
        string balanceServerTransId;
        var balanceTransID = string.Format("{0}/{1}", transactionID, multiStepProcess++);
        var getCardBalance = CardBalance_Impl.CardBalance(sourceRequest, cardNumber, balanceTransID, 
          out balanceResult, out balanceServerTransId, out errorMessage);
        if (getCardBalance != (int)General.WCFCallResult.OK)
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.DeductCardLoadProfile, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.ServiceProviderCommsError;
        }
        #endregion

        if (amountInCents <= 0 || amountInCents > balanceResult.BalanceInCents || balanceResult.BalanceInCents < 1000)
        {
          amountInCents = balanceResult.BalanceInCents;
          if (amountInCents <= 0)
          {
            errorMessage = string.Format("Insufficient funds on card: card balance: {0:c}", (Decimal)amountInCents / 100M);
            Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.DeductCardLoadProfile, new Exception(errorMessage), null, null, sourceRequest);
            return (int)General.WCFCallResult.BadParams;
          }
        }
        #endregion

        #region XML-RPC
        var xmlRequest = new DeductCardLoadProfile_Input()
        {
          profileNumber = profileNumber,
          terminalID = terminalID,
          cardNumber = cardNumber,
          transactionDate = DateTime.Now,
          transactionID = transactionID,
          requestAmount = amountInCents
        };
        string error;
        var xmlResult = NuCardXMLRPCUtils.DeductCardLoadProfile(Atlas.Server.NuCard.Utils.CachedValues.TutukaEndpoint, terminalPassword, xmlRequest, out xmlSent, out xmlRecv, out error);

        serverTransactionID = xmlResult.serverTransactionID;
        var endDT = DateTime.Now;
        NuCardDb.LogAdminRequest(sourceRequestString, application, startDT, endDT, NuCard.AdminRequestType.DeductCardLoadProfile,
          xmlResult.resultCode, 0, xmlRequest.cardNumber, null,
          xmlSent, xmlRecv, xmlRequest.transactionID, xmlResult.serverTransactionID, error);
        #endregion

        #region Return result
        if (xmlResult.resultCode != (int)NuCard.AdminRequestResult.Successful)
        {
          errorMessage = NuCardXMLRPCUtils.GetNuCardErrorString(xmlResult.resultCode, xmlResult.resultText);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.DeductCardLoadProfile, new Exception(errorMessage), xmlSent, xmlRecv, sourceRequest);
          return (int)General.WCFCallResult.ServiceProviderCommsError;
        }

        transferredAmountInCents = xmlResult.requestAmount;

        // NOTE: Atlas Management system will log- Fabian: 2013-12-20
        if (application.Application.AtlasApplication != General.ApplicationIdentifiers.AtlasManagement)
        {
          Utils.LogTransaction(nuCardId: card.NuCardId, serverTransactionId: serverTransactionID,
            description: null, referenceNum: transactionID, amount: ((Decimal)transferredAmountInCents / 100M) * -1, loadDate: startDT,
            isPending: false, source: application.Application.AtlasApplication, 
            transactionSource: NuCard.TransactionSourceType.API,
            createdByPersonId: swOperator != null && swOperator.Person != null ? swOperator.Person.PersonId: 0, createdDT: startDT); // TODO: Db logical errors due to duplicate ids... 
        }

        _log.Information("{MethodName} completed successfully- transferred: {TransferredAmountInCents}", methodName, transferredAmountInCents);

        return (int)General.WCFCallResult.OK;
        #endregion
      }
      catch (Exception err)
      {        
        Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.DeductCardLoadProfile, err, xmlSent, xmlRecv, sourceRequest);
        errorMessage = Atlas.Server.NuCard.WCF.Implementation.Consts.ERR_SERVER_GENERIC;
        return (int)General.WCFCallResult.ServerError;
      }
    }


    #region Logging

    private static readonly ILogger _log = Log.Logger.ForContext<AtlasServer.WCF.Implementation.NuCardAdminServer>();

    #endregion

  }


}

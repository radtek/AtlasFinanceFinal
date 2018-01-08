/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012-2013 Atlas Finance (Pty) Ltd.
* 
* 
*  Description:
*  ------------------
*    Update cellular contact details of the NuCard
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
using Atlas.NuCard.Repository;

#endregion


namespace AtlasServer.WCF.Admin.Implementation
{
  public static class UpdateAllocatedCard_Impl
  {
    /// <summary>
    /// Update cell phone details of an allocated card
    /// </summary>
    /// <param name="sourceRequest">Source request parameters</param>
    /// <param name="cardNumber">The card number</param>
    /// <param name="cellphoneNumber">The new cell phone number</param>
    /// <param name="serverTransactionID"></param>
    /// <param name="errorMessage">Any error message to display to the end-user (out)</param>
    /// <returns>General.WCFCallResult enum</returns> 
    public static int UpdateAllocatedCard(SourceRequest sourceRequest,
      string cardNumber, string cellphoneNumber, string transactionID,
       out string serverTransactionID, out string errorMessage)
    {
      var methodName = "UpdateAllocatedCard";
      var startDT = DateTime.Now;
      var sourceRequestString = JsonConvert.SerializeObject(new { sourceRequest, cardNumber, cellphoneNumber, transactionID });
      _log.Information("{MethodName} starting: {@Request}", methodName, sourceRequest);

      errorMessage = string.Empty;
      serverTransactionID = string.Empty;

      PER_SecurityDTO swOperator = null;
      COR_AppUsageDTO application = null;
      BRN_BranchDTO branch = null;
      try
      {
        #region Check parameters
        if (!WCFUtils.CheckSourceRequest(sourceRequest, out application, out swOperator, out branch, out errorMessage))
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.UpdateAllocatedCard, new Exception(errorMessage), null, null, sourceRequest);
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
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.UpdateAllocatedCard, new Exception(errorMessage), null, null, sourceRequest);
          errorMessage = WCFUtils.SourceRequestErrorString(checkSourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        var card = WCFUtils.CheckCard(cardNumber, null, true, null, false, out errorMessage);
        if (card == null)
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.UpdateAllocatedCard, new Exception(errorMessage), null, null, sourceRequest);
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

        #region XML request
        var xmlRequest = new UpdateAllocatedCard_Input()
        {
          cardNumber = card.CardNum,
          cellphoneNumber = cellphoneNumber,

          profileNumber = card.NuCardProfile.ProfileNum,
          terminalID = card.NuCardProfile.TerminalId,

          transactionID = transactionID,
          transactionDate = DateTime.Now
        };

        string xmlSent;
        string xmlRecv;
        var xmlResult = NuCardXMLRPCUtils.UpdateAllocatedCard(Atlas.Server.NuCard.Utils.CachedValues.TutukaEndpoint, terminalPassword, xmlRequest, 
          out xmlSent, out xmlRecv, out errorMessage);
        serverTransactionID = xmlResult.serverTransactionID;
        var endDT = DateTime.Now;

        NuCardDb.LogAdminRequest(sourceRequestString, application, startDT, endDT, NuCard.AdminRequestType.UpdateAllocatedCard,
          xmlResult.resultCode, 0, xmlRequest.cardNumber, string.Empty,
          xmlSent, xmlRecv, xmlRequest.transactionID, xmlResult.serverTransactionID, errorMessage);

        #endregion

        // Link cell phone number to person
        PersonData.FindPersonDTO(card.AllocatedPerson.IdNum, card.AllocatedPerson.Firstname, card.AllocatedPerson.Lastname, cellphoneNumber, branch.BranchId, swOperator,
          application.Application.AtlasApplication == General.ApplicationIdentifiers.ASS ? General.Host.ASS : General.Host.LAS, true);

        _log.Information("{MethodName} completed successfully", methodName);
        return (int)General.WCFCallResult.OK;
      }
      catch (Exception err)
      {
        Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.UpdateAllocatedCard, err, null, null, sourceRequest);
        errorMessage = Atlas.Server.NuCard.WCF.Implementation.Consts.ERR_SERVER_GENERIC;
        return (int)General.WCFCallResult.ServerError;
      }
    }


    #region Logging

    private static readonly ILogger _log = Log.Logger.ForContext<AtlasServer.WCF.Implementation.NuCardAdminServer>();

    #endregion
  }
}

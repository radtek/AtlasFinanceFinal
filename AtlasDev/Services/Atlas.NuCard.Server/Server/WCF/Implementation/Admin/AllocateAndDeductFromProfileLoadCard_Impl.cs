/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012-2013 Atlas Finance (Pty) Ltd.
* 
* 
*  Description:
*  ------------------
*    All-in-one routine: Allocates/links and loads a card
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

using Atlas.Domain.DTO;
using Atlas.Enumerators;
using Atlas.WCF.Implementation;
using Atlas.NuCard.WCF.Interface;

#endregion


namespace AtlasServer.WCF.Admin.Implementation
{
  public static class AllocateAndDeductFromProfileLoadCard_Impl
  {
    /// <summary>
    /// This will link the card to a profile, allocate card to person and then load the funds from the profile
    /// </summary>
    /// <param name="sourceRequest">Source request parameters</param>
    /// <param name="cardNumber">Full card number</param>
    /// <param name="firstName">Cardholder first name</param>
    /// <param name="lastName">Cardholder surname</param>
    /// <param name="idOrPassportNumber">Cardholder passport or SA ID number</param>
    /// <param name="cellPhoneNumber">Cardholder cell phone number</param>
    /// <param name="amountInCents">Amount in cents, to transfer to NuCard (if zero passed, no load)</param>
    /// <param name="transactionID">Source reference ID</param>
    /// <param name="serverTransactionID">Server transaction ID (out)</param>
    /// <param name="errorMessage">Any error message to display to the end-user (out)</param>
    /// <returns>General.WCFCallResult enum (1- success, 0- no operation, etc.)</returns>
    public static int AllocateAndDeductFromProfileLoadCard(SourceRequest sourceRequest,
        string cardNumber, string firstName, string lastName, string idOrPassportNumber, string cellPhoneNumber,
        int amountInCents, string transactionID,
        out string serverTransactionID, out string errorMessage)
    {
      // Can do this: using (Serilog.Context.LogContext.PushProperty("MethodName", "AllocateAndDeductFromProfileLoadCard")) 
      //                                               .PushProperty("Request",    {}
      //

      var methodName = "AllocateAndDeductFromProfileLoadCard";
      var startDT = DateTime.Now;
      var sourceRequestString = JsonConvert.SerializeObject(new { sourceRequest, cardNumber, amountInCents, transactionID });
      _log.Information("{MethodName} starting: {@Request}", methodName, sourceRequest);

      errorMessage = string.Empty;
      serverTransactionID = string.Empty;

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
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.MultistepProcess, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        if (sourceRequest.MachineName != "HO-SRV-TS-2")
        {
          // If ASS, machine *MUST* match naming convention: <bbb>-00-<ss>  Where <bbb> is branch and <ss> is station number
          if (application.Application.AtlasApplication == General.ApplicationIdentifiers.ASS &&
            !System.Text.RegularExpressions.Regex.IsMatch(sourceRequest.MachineName, "[0-9A-Z]{2,3}\\-00\\-[0-9]{2,2}"))
          {
            errorMessage = string.Format("Your machine's name does not conform to Atlas naming standards: '{0}'", sourceRequest.MachineName);
            Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.MultistepProcess, new Exception(errorMessage), null, null, sourceRequest);
            return (int)General.WCFCallResult.BadParams;
          }
        }


        var checkSourceRequest = WCFUtils.GetTutukaFromRequest(sourceRequest, out terminalID, out profileNumber, out terminalPassword);
        if (checkSourceRequest != WCFUtils.CheckSourceRequestResult.ParamsOK)
        {
          errorMessage = WCFUtils.SourceRequestErrorString(checkSourceRequest);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.MultistepProcess, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        if (string.IsNullOrEmpty(transactionID))
        {
          errorMessage = "Missing transaction ID";
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.MultistepProcess, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }
        transactionID = transactionID.Trim();

        // Card must be unallocated
        var card = WCFUtils.CheckCard(cardNumber, new List<NuCard.NuCardStatus>() { NuCard.NuCardStatus.InStock }, null, false, true, out errorMessage);
        if (card == null)
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
        if (string.IsNullOrEmpty(idOrPassportNumber) || idOrPassportNumber.Length < 5)
        {
          errorMessage = "Invalid ID or passport- minimum 5 characters";
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.LoadCardDeductProfile, new Exception(errorMessage), null, null, sourceRequest);

          return (int)General.WCFCallResult.BadParams;
        }
        idOrPassportNumber = idOrPassportNumber.Trim();

        cellPhoneNumber = WCFUtils.UnescapeUriString(cellPhoneNumber);
        if (string.IsNullOrEmpty(cellPhoneNumber) || cellPhoneNumber.Length < 10)
        {
          errorMessage = "Invalid cell phone number";
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.LoadCardDeductProfile, new Exception(errorMessage), null, null, sourceRequest);

          return (int)General.WCFCallResult.BadParams;
        }
        cellPhoneNumber = cellPhoneNumber.Trim();

        #endregion

        var multiStepTransId = 0;

        #region XML-RPC
        if (card.NuCardProfile == null)
        {
          string linkServerTransId;
          var linkcardTransId = string.Format("{0}/{1}", transactionID, ++multiStepTransId);
          var linkCardResult = LinkCard_Impl.LinkCard(sourceRequest, cardNumber, linkcardTransId, out linkServerTransId, out errorMessage);
          if (linkCardResult != (int)General.WCFCallResult.OK &&
            !errorMessage.Contains(((int)NuCard.AdminRequestResult.CardAlreadyLinked).ToString()))
          {
            Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.LoadCardDeductProfile, new Exception(errorMessage), null, null, sourceRequest);
            return linkCardResult;
          }
        }

        var allocateTransId = string.Format("{0}/{1}", transactionID, ++multiStepTransId);
        string allocateServerTransId;
        var allocateCardResult = AllocateCard_Impl.AllocateCard(sourceRequest, cardNumber, firstName, lastName, idOrPassportNumber, cellPhoneNumber,
          allocateTransId, out allocateServerTransId, out errorMessage);
        if (allocateCardResult != (int)General.WCFCallResult.OK)
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.LoadCardDeductProfile, new Exception(errorMessage), null, null, sourceRequest);
          return allocateCardResult;
        }

        if (amountInCents > 0)
        {
          //var deductTransId = string.Format("{0}/{1}", transactionID, ++multiStepTransId);
          var loadCardResult = DeductFromProfileLoadCard_Impl.DeductFromProfileLoadCard(sourceRequest, cardNumber, amountInCents, transactionID,
            out serverTransactionID, out errorMessage);
          if (loadCardResult != (int)General.WCFCallResult.OK)
          {
            Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.LoadCardDeductProfile, new Exception(errorMessage), null, null, sourceRequest);
            return loadCardResult;
          }
          else
          {
            serverTransactionID = allocateServerTransId;
          }
        }
        #endregion

        _log.Information("{MethodName} completed successfully", methodName);
        return (int)General.WCFCallResult.OK;
      }
      catch (Exception err)
      {        
        Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.MultistepProcess, err, null, null, sourceRequest);
        errorMessage = Atlas.Server.NuCard.WCF.Implementation.Consts.ERR_SERVER_GENERIC;
        return (int)General.WCFCallResult.ServerError;
      }
    }


    #region Logging

    private static readonly ILogger _log = Log.Logger.ForContext<AtlasServer.WCF.Implementation.NuCardAdminServer>();

    #endregion

  }
}

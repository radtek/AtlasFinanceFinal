/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012-2014 Atlas Finance (Pty) Ltd.
* 
* 
*  Description:
*  ------------------
*    Load card with funds
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
using Atlas.ThirdParty.XMLRPC.Classes;
using Atlas.ThirdParty.XMLRPC.Utils;
using Atlas.WCF.Implementation;
using Atlas.NuCard.WCF.Interface;
using Atlas.NuCard.Repository;

#endregion


namespace AtlasServer.WCF.Admin.Implementation
{
  public static class DeductFromProfileLoadCard_Impl
  {
    /// <summary>
    /// Loads funds onto a card
    /// </summary>
    /// <param name="sourceRequest">Source request parameters</param>
    /// <param name="cardNumber">Full card number</param>
    /// <param name="amountInCents">Amount in cents, to load into the card</param>
    /// <param name="transactionID">Source reference ID</param>
    /// <param name="serverTransactionID">Server transaction ID (out)</param>
    /// <param name="errorMessage">Any error message to display to the end-user (out)</param>
    /// <returns>General.WCFCallResult enum (1- success, 0- no operation, etc.)</returns>
    public static int DeductFromProfileLoadCard(SourceRequest sourceRequest,
       string cardNumber, int amountInCents, string transactionID,
       out string serverTransactionID, out string errorMessage)
    {
      var methodName = "DeductFromProfileLoadCard";
      var startDT = DateTime.Now;
      var sourceRequestString = JsonConvert.SerializeObject(new { sourceRequest, cardNumber, amountInCents, transactionID });
      _log.Information("{MethodName} starting: {@Request}", methodName, sourceRequest);

      errorMessage = string.Empty;
      serverTransactionID = string.Empty;

      PER_SecurityDTO swOperator = null;
      COR_AppUsageDTO application = null;
      BRN_BranchDTO branch = null;
      var transactionDT = DateTime.Now;
      string xmlSent = null;
      string xmlRecv = null;
      try
      {
        #region Check parameters
        if (!WCFUtils.CheckSourceRequest(sourceRequest, out application, out swOperator, out branch, out errorMessage))
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.LoadCardDeductProfile, new Exception(errorMessage), null, null, sourceRequest);
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

        // Check if transaction is still in progress (Tutuka/branch network time-outs)
        if (Atlas.Server.NuCard.Utils.CachedValues.TransInProgress(transactionID, TimeSpan.FromMinutes(10)))
        {
          errorMessage = string.Format("Simultaneous request: system is still processing transaction '{0}'", transactionID);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.LoadCardDeductProfile, new Exception(errorMessage), null, null, sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        try
        {
          string terminalID;
          string profileNumber;
          string terminalPassword;
          var checkSourceRequest = WCFUtils.GetTutukaFromRequest(sourceRequest, out terminalID, out profileNumber, out terminalPassword);
          if (checkSourceRequest != WCFUtils.CheckSourceRequestResult.ParamsOK)
          {
            errorMessage = WCFUtils.SourceRequestErrorString(checkSourceRequest);
            Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.LoadCardDeductProfile, new Exception(errorMessage), null, null, sourceRequest);
            return (int)General.WCFCallResult.BadParams;
          }

          if (amountInCents <= Consts.MIN_LOAD_AMOUNT_IN_CENTS || amountInCents > Consts.MAX_LOAD_AMOUNT_IN_CENTS)
          {
            errorMessage = string.Format("Invalid amount requested: {0:c}", (Decimal)amountInCents / 100M);
            Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.LoadCardDeductProfile, new Exception(errorMessage), null, null, sourceRequest);
            return (int)General.WCFCallResult.BadParams;
          }

          if (!Utils.TransactionIdIsUnique(transactionID))
          {
            errorMessage = string.Format("The load card with reference '{0}' has already been successfully completed", transactionID);
            Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.LoadCardDeductProfile, new Exception(errorMessage), null, null, sourceRequest);
            return (int)General.WCFCallResult.BadParams;
          }

          // Check limit for the day
          if (Utils.ThisLoadExceedsMaximum((decimal)amountInCents / 100M, cardNumber, branch.LegacyBranchNum))
          {
            errorMessage = "The maximum daily load limit has been reached for this card- please post tomorrow";
            Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.LoadCardDeductProfile, new Exception(errorMessage), null, null, sourceRequest);
            return (int)General.WCFCallResult.BadParams;
          }

          // Card must be linked, allocated and in an active state
          var card = WCFUtils.CheckCard(cardNumber, null,
            //new List<NuCard.NuCardStatus>(){NuCard.NuCardStatus.Active, NuCard.NuCardStatus.USE, NuCard.NuCardStatus.ISSUE}, 
            null /* !!!! */, null /* !!!! */, true, out errorMessage);
          if (card == null)
          {
            Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.LoadCardDeductProfile, new Exception(errorMessage), null, null, sourceRequest);
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

          #region Perform the XML request
          string error;
          var xmlRequest = new LoadCardDeductProfile_Input()
          {
            profileNumber = profileNumber,
            terminalID = terminalID,
            cardNumber = cardNumber,
            requestAmount = amountInCents,
            transactionDate = transactionDT,
            transactionID = transactionID
          };

          var commsSuccessful = false;
          var attempts = 0;
          var xmlLoadResult = new LoadCardDeductProfile_Output { resultCode = null, resultText = null };

          while (attempts++ < 3 && !commsSuccessful)
          {
            try
            {
              xmlLoadResult = NuCardXMLRPCUtils.LoadCardDeductProfile(Atlas.Server.NuCard.Utils.CachedValues.TutukaEndpoint, terminalPassword, xmlRequest,
                out xmlSent, out xmlRecv, out error);
              var endDT = DateTime.Now;
              serverTransactionID = xmlLoadResult.serverTransactionID;

              NuCardDb.LogAdminRequest(sourceRequestString, application, startDT, endDT, NuCard.AdminRequestType.LoadCardDeductProfile,
                xmlLoadResult.resultCode, xmlLoadResult.requestAmount != null ? (Decimal)xmlLoadResult.requestAmount / 100M : 0,
                xmlRequest.cardNumber, "", xmlSent, xmlRecv, xmlRequest.transactionID, xmlLoadResult.serverTransactionID, error);

              // Throw any errors, so can be handled in exception hander...
              if (xmlLoadResult.resultCode == null)
              {
                throw new Exception("Empty response from supplier");
              }

              // Treat the 'duplicate transaction' error as success
              if (xmlLoadResult.resultCode == (int)NuCard.AdminRequestResult.DuplicateTrans)
              {
                xmlLoadResult.resultCode = (int)NuCard.AdminRequestResult.Successful;
              }

              //if (xmlResult.resultCode.Value != (int)NuCard.AdminRequestResult.Successful)
              //{
              //  throw new Exception(NuCardXMLRPCUtils.GetNuCardErrorString(xmlResult.resultCode, xmlResult.resultText));
              //}

              commsSuccessful = true;
            }
            catch (Exception err)
            {
              Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.LoadCardDeductProfile, err, xmlSent, xmlRecv, sourceRequest);
              errorMessage = "Unexpected communications error";

              #region Transfer may have occurred- we got cut-off, double check whether the transaction was successful...
              var transChecked = false;
              var checkCount = 0;
              var xmlCheckRequest = new CheckAuthorisation_Input()
              {
                profileNumber = profileNumber,
                terminalID = terminalID,
                cardNumber = cardNumber,
                requestAmount = amountInCents,
                transactionDate = transactionDT, // Must match request
                transactionID = transactionID    // Must match request
              };

              while (++checkCount < 5 && !transChecked)
              {
                System.Threading.Thread.Sleep(1000);

                try
                {
                  _log.Information("CheckAuthorisation starting: Iteration: {CheckCount}", checkCount);

                  var checkResult = NuCardXMLRPCUtils.CheckAuthorisation(Atlas.Server.NuCard.Utils.CachedValues.TutukaEndpoint, terminalPassword,
                    xmlCheckRequest, out xmlSent, out xmlRecv, out error);

                  NuCardDb.LogAdminRequest(sourceRequestString, application, startDT, DateTime.Now, NuCard.AdminRequestType.CheckLoad,
                    checkResult.resultCode, (Decimal)amountInCents / 100M, xmlRequest.cardNumber, "", xmlSent, xmlRecv, xmlRequest.transactionID, checkResult.serverTransactionID, error);

                  if (checkResult.resultCode != null)
                  {
                    _log.Information("CheckAuthorisation: Received result '{@CheckResult}'", checkResult);
                    transChecked = true;

                    if (checkResult.resultCode.Value == (int)NuCard.AdminRequestResult.Successful) // Load was successful
                    {
                      xmlLoadResult.resultCode = (int)NuCard.AdminRequestResult.Successful;
                      errorMessage = null;
                      commsSuccessful = true;
                    }
                  }
                  else
                  {
                    _log.Warning("CheckAuthorisation: No response from supplier");
                  }
                }
                catch (Exception checkErr)
                {
                  _log.Error("CheckAuthorisation: Error- '{0}'", checkErr);
                }
              }
              #endregion
            }
          }
          #endregion

          #region Comms failure
          if (!commsSuccessful)
          {
            return (int)General.WCFCallResult.ServiceProviderCommsError;
          }

          #region Return error
          if (xmlLoadResult.resultCode != (int)NuCard.AdminRequestResult.Successful)
          {
            errorMessage = NuCardXMLRPCUtils.GetNuCardErrorString(xmlLoadResult.resultCode, xmlLoadResult.resultText);
            Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.LoadCardDeductProfile, new Exception(errorMessage), xmlSent, xmlRecv, sourceRequest);
            return (int)General.WCFCallResult.ServiceProviderCommsError;
          }
          #endregion

          // NOTE: Atlas Management system will log- Fabian: 2013-12-20
          if (application.Application.AtlasApplication != General.ApplicationIdentifiers.AtlasManagement)
          {
            Utils.LogTransaction(nuCardId: card.NuCardId, serverTransactionId: serverTransactionID,
              description: null, referenceNum: transactionID, amount: (Decimal)amountInCents / 100M, loadDate: startDT,
              isPending: false, source: application.Application.AtlasApplication, transactionSource: NuCard.TransactionSourceType.API,
              createdByPersonId: swOperator != null && swOperator.Person != null ? swOperator.Person.PersonId : 0, createdDT: startDT);
          }

          #region Send the loading to messaging bus
          try
          {            
            //var contacts = card.AllocatedPerson?.get
            //if (card.AllocatedPerson != null && card.AllocatedPerson.getContacts != null)
            //{
              // Get most recent active cellular number for client
              //var cellNum = card.AllocatedPerson.Contacts
              //  .Where(s => s.IsActive && s.ContactType.ContactTypeId == (int)Atlas.Enumerators.General.ContactType.CellNo)
              //  .OrderByDescending(s => s.CreatedDT);

              //if (cellNum != null && cellNum.Any())
              //{
              //  var sendTo = cellNum.First().Value;

              //  if (sendTo.Length == 10)
              //  {
              //    Atlas.WCF.Implementation.MessagingBus.SendCoupon(card.AllocatedPerson.Firstname, card.AllocatedPerson.Lastname,
              //      card.AllocatedPerson.IdNum, sendTo, branch.LegacyBranchNum);
              //  }
              //}
            //}
          }
          catch (Exception err)
          {
            _log.Error(err, "{MethodName} Error trying to locate cell details/send the coupon via messaging bus", methodName);
          }
          #endregion

          _log.Information("{MethodName} completed successfully- {Amount} transferred", methodName, (Decimal)amountInCents / 100M);
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
        Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, NuCard.AdminRequestType.LoadCardDeductProfile, err, xmlSent, xmlRecv, sourceRequest);
        errorMessage = Atlas.Server.NuCard.WCF.Implementation.Consts.ERR_SERVER_GENERIC;
        return (int)General.WCFCallResult.ServerError;
      }
    }


    #region Logging

    private static readonly ILogger _log = Log.Logger.ForContext<AtlasServer.WCF.Implementation.NuCardAdminServer>();

    #endregion

  }
}

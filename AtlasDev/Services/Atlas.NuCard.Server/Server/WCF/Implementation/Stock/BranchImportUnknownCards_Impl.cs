/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012-2013 Atlas Finance (Pty) Ltd.
* 
* 
*  Description:
*  ------------------
*    Import unknown cards in the Atlas NuCard stock system for this branch
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
using System.Threading.Tasks;

using Newtonsoft.Json;
using Serilog;
using DevExpress.Xpo;

using Atlas.Common.Utils;
using Atlas.Domain.DTO;
using Atlas.Enumerators;
using Atlas.WCF.Implementation;
using Atlas.NuCard.WCF.Interface;
using Atlas.Domain.DTO.Nucard;
using Atlas.Domain.Model;
using Atlas.Server.NuCard.Utils;
using Atlas.Common.Extensions;
using Atlas.ThirdParty.XMLRPC.Utils;
using Atlas.ThirdParty.XMLRPC.Classes;
using Atlas.NuCard.Repository;

#endregion


namespace AtlasServer.WCF.Stock.Implementation
{
  public static class BranchImportUnknownCards_Impl
  {
    /// <summary>
    /// Default NuCard profile number to link unlinked cards to
    /// </summary>
    private const string DEFAULT_NUCARD_PROFILE = "6835133429";


    public static int BranchImportUnknownCards(SourceRequest sourceRequest, List<NuCardStockItem> cards,
      out List<NuCardStockItem> cardsImported, out string errorMessage)
    {
      var methodName = "BranchImportUnknownCards";
      var startDT = DateTime.Now;
      var sourceRequestString = JsonConvert.SerializeObject(new { sourceRequest, cards });
      _log.Information("{MethodName} starting: {SourceRequest}- cards: {@Cards}", methodName, sourceRequest, cards);

      cardsImported = new List<NuCardStockItem>();
      errorMessage = string.Empty;

      PER_SecurityDTO swOperator = null;
      COR_AppUsageDTO application = null;
      BRN_BranchDTO branch = null;
      try
      {
        #region Check request
        if (!WCFUtils.CheckSourceRequest(sourceRequest, out application, out swOperator, out branch, out errorMessage))
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, null,
            NuCard.StockRequestType.ImportUnknownCard, NuCard.StockRequestResult.BadRequestParameters, new Exception(errorMessage), sourceRequest);

          return (int)General.WCFCallResult.BadParams;
        }
        #endregion

        #region Get default profile
        NUC_NuCardProfileDTO defaultProfile = null;
        using (var unitOfWork = new UnitOfWork())
        {
          // 2014-09-12- There is now only one profile...                  
          defaultProfile = AutoMapper.Mapper.Map<NUC_NuCardProfileDTO>(
                    unitOfWork.Query<NUC_NuCardProfile>()
                    .First(s => s.ProfileNum == DEFAULT_NUCARD_PROFILE));
        }
        #endregion

        foreach (var newCard in cards)
        {
          try
          {
            bool isValid = true;            
            #region Check basic card details check
            if (string.IsNullOrEmpty(newCard.FullCardNumber) ||
              !CCardValidator.Validate(CCardValidator.CardType.NuCard, newCard.FullCardNumber))
            {
              errorMessage += string.Format("Invalid card number- '{0}'\n\r", newCard.FullCardNumber);
              Utils.LogBadRequest(string.Format("{0}- Bad card: {1}", methodName, newCard.FullCardNumber), sourceRequestString,
                application, startDT, newCard.FullCardNumber, NuCard.StockRequestType.ImportUnknownCard,
                NuCard.StockRequestResult.InvalidCardNumber, null);
              isValid = false;
            }

            /*if (string.IsNullOrEmpty(newCard.TrackingNumber))
            {
              errorMessage += string.Format("No tracking number was provided\n\r");
              __log.Warning("BranchImportUnknownCards- No tracking number for card: {0}", newCard.FullCardNumber);
              NuCardDb.LogStockRequest(sourceRequestString, application, startDT, newCard.FullCardNumber,
                NuCard.StockRequestType.ImportUnknownCard, NuCard.StockRequestResult.MissingTrackingNumber, null);
              isValid = false;
            }*/
            #endregion

            // Ensure we are not already processing this card with another overlapping call
            if (isValid && CachedValues.CardImportInProgres(newCard.FullCardNumber, TimeSpan.FromMinutes(5)))
            {
              errorMessage += string.Format("Card already being imported- '{0}'\n\r", newCard.FullCardNumber);
              Utils.LogBadRequest(string.Format("{0}- Card already being imported: {1}", methodName, newCard.FullCardNumber), sourceRequestString,
                application, startDT, newCard.FullCardNumber, NuCard.StockRequestType.ImportUnknownCard,
                NuCard.StockRequestResult.SystemError, null);
              isValid = false;
            }

            if (isValid)
            {
              NUC_NuCardDTO importedCard = null;

              using (var unitOfWork = new UnitOfWork())
              {
                var inStockStatus = unitOfWork.Query<NUC_NuCardStatus>().First(s => s.NuCardStatusId == (int)NuCard.NuCardStatus.InStock);

                #region Add/update card in DB
                var cardInDb = unitOfWork.Query<NUC_NuCard>().FirstOrDefault(s => s.CardNum == newCard.FullCardNumber.Trim());
                if (cardInDb == null)
                {
                  cardInDb = new NUC_NuCard(unitOfWork)
                  {
                    CardNum = newCard.FullCardNumber.Trim(),
                    TrackingNum = newCard.TrackingNumber.Trim(),
                    CreatedDT = DateTime.Now,
                    ExpiryDT = newCard.ExpiryDT > new DateTime(2008, 1, 1) ? newCard.ExpiryDT : DateTime.MinValue,
                    IssueDT = DateTime.Now,
                    Status = inStockStatus
                  };
                }
                else
                {
                  var okStatus = unitOfWork.Query<NUC_NuCardStatus>().Where(s =>
                    s.NuCardStatusId == (int)NuCard.NuCardStatus.InTransit ||
                    s.NuCardStatusId == (int)NuCard.NuCardStatus.InStock);
                  if (!okStatus.Contains(cardInDb.Status))
                  {
                    if (cardInDb.Status == null)
                    {
                      cardInDb.Status = inStockStatus;
                    }
                    errorMessage = string.Format("Card is currently {0}", EnumExtension.ToStringEnum((NuCard.NuCardStatus)cardInDb.Status.NuCardStatusId));
                    continue;
                  }
                }
                #endregion

                if (cardInDb.NuCardProfile == null || cardInDb.ExpiryDT < new DateTime(2008, 1, 1))
                {
                  #region Perform XML request to link card to the default profile- if already linked, treat as successful...
                  var foundExpiryDate = DateTime.MinValue;
                  var isError = false;
                  var attemptCount = 0;
                  string xmlSent = null;
                  string xmlRecv = null;
                  var error = (string)null;
                    
                  do
                  {
                    var linkIn = new LinkCard_Input()
                    {
                      profileNumber = defaultProfile.ProfileNum,
                      terminalID = defaultProfile.TerminalId,
                      cardNumber = newCard.FullCardNumber,
                      transactionDate = DateTime.Now,
                      transactionID = Guid.NewGuid().ToString()
                    };

                    var linkOut = new LinkCardCard_Output();
                    try
                    {
                      linkOut = NuCardXMLRPCUtils.LinkCard(Atlas.Server.NuCard.Utils.CachedValues.TutukaEndpoint, defaultProfile.Password, linkIn, out xmlSent, out xmlRecv, out error);
                    }
                    catch (Exception err)
                    {
                      _log.Error("[LinkCard- Unknown card]- {0}, XML Sent: {1}, XML Recv: {2}", err, xmlSent, xmlRecv);
                      isError = true;
                    }

                    NuCardDb.LogAdminRequest(sourceRequestString, application, startDT, DateTime.Now, NuCard.AdminRequestType.LinkCard,
                      linkOut.resultCode, 0, linkIn.cardNumber, null,
                      xmlSent, xmlRecv, linkIn.transactionID, linkOut.serverTransactionID, error);

                    if (linkOut.resultCode.HasValue && (linkOut.resultCode.Value == (int)NuCard.AdminRequestResult.Successful ||
                      linkOut.resultCode.Value == (int)NuCard.AdminRequestResult.CardAlreadyLinked))
                    {
                      cardInDb.NuCardProfile = unitOfWork.Query<NUC_NuCardProfile>().First(s => s.NuCardProfileId == defaultProfile.NuCardProfileId);
                      _log.Warning("Failed to determine existing profile, but successfully linked card to the default profile");
                      isError = false;
                    }
                    else
                    {
                      _log.Error(new Exception(NuCardXMLRPCUtils.GetNuCardErrorString(linkOut.resultCode, linkOut.resultText)),
                        "Failed to determine card profile and LinkCard failed: {CardNumber}", newCard.FullCardNumber);
                      isError = true;
                    }
                  }
                  while (isError && attemptCount++ < 5);
                  #endregion
                                        
                  #region Try determine card's expiry date
                  isError = false;
                  attemptCount = 0;
                  do
                  {
                    _log.Information("Testing 'Status' with profile {Profile}", defaultProfile);
                    var transactionID = Guid.NewGuid().ToString();
                    xmlSent = null;
                    xmlRecv = null;
                    error = null;

                    #region Try get expiry date via 'balance' enquiry
                    _log.Information("Testing 'Balance' with profile {@Profile}", defaultProfile);

                    transactionID = Guid.NewGuid().ToString();
                    Balance_Output balanceOut = new Balance_Output();
                    var balanceIn = new Balance_Input()
                    {
                      profileNumber = defaultProfile.ProfileNum,
                      terminalID = defaultProfile.TerminalId,
                      cardNumber = newCard.FullCardNumber,
                      transactionDate = DateTime.Now,
                      transactionID = transactionID
                    };

                    _log.Information("[Balance- Unknown card]- Request: {BalanceIn}", balanceIn);
                    try
                    {
                      balanceOut = NuCardXMLRPCUtils.Balance(Atlas.Server.NuCard.Utils.CachedValues.TutukaEndpoint, defaultProfile.Password, balanceIn,
                        out xmlSent, out xmlRecv, out error);
                    }
                    catch (Exception xmlErr)
                    {
                      _log.Error("[Balance- Unknown card] Err: {XMLErr}, XML Sent: {XMLSent}, XML Recv: {XMLRecv}", xmlErr, xmlSent, xmlRecv);
                      isError = true;
                    }

                    NuCardDb.LogAdminRequest(sourceRequestString, application, startDT, DateTime.Now, NuCard.AdminRequestType.CardBalance,
                      balanceOut.resultCode, balanceOut.balanceAmount != null ? (Decimal)balanceOut.balanceAmount.Value / 100M : 0, balanceOut.cardNumber, null,
                      xmlSent, xmlRecv, balanceIn.transactionID, balanceOut.serverTransactionID, error);

                    _log.Information("[Balance- Unknown card]- XML Sent: {XMLSent}, XML Recv: {XMLRecv}", xmlSent, xmlRecv);

                    if (balanceOut.resultCode.HasValue &&
                      (balanceOut.resultCode.Value == (int)NuCard.AdminRequestResult.Successful ||
                      balanceOut.resultCode.Value == (int)NuCard.AdminRequestResult.CardNotAllocated))
                    {
                      _log.Information("Test 'Balance' returned a positive result. Profile: {@Profile}", defaultProfile);
                      if (balanceOut.resultCode.Value == (int)NuCard.AdminRequestResult.Successful)
                      {
                        foundExpiryDate = balanceOut.expiryDate.Value;
                        _log.Information("Test 'Balance' found expiry date for card- {ExpiryDate}. Profile: {@Profile}", foundExpiryDate, defaultProfile);
                      }
                      isError = false;
                    }
                    else
                    {
                      _log.Warning(NuCardXMLRPCUtils.GetNuCardErrorString(balanceOut.resultCode, balanceOut.resultText),
                        "Test 'Balance' failed for profile {@Profile} (not allocated)", defaultProfile);
                      isError = true;
                    }                      
                  }
                  while (attemptCount++ < 5 && isError);
                  #endregion

                  #endregion

                  if (!isError)
                  {
                    cardInDb.NuCardProfile = unitOfWork.Query<NUC_NuCardProfile>().First(s => s.NuCardProfileId == defaultProfile.NuCardProfileId);
                    if (foundExpiryDate > DateTime.MinValue)
                    {
                      cardInDb.ExpiryDT = foundExpiryDate;
                    }
                    else
                    {
                      _log.Warning("Failed to exactly determine the card's expiry date- assuming 2.5 years...");
                      if (cardInDb.ExpiryDT < new DateTime(2008, 1, 1))
                      {
                        cardInDb.ExpiryDT = DateTime.Now.AddYears(2).AddMonths(6);
                      }
                    }

                    cardInDb.LastEditedDT = DateTime.Now;
                    cardInDb.Status = inStockStatus;
                  }
                  else
                  {
                    cardInDb = null;
                  }
                }

                unitOfWork.CommitChanges();
                importedCard = AutoMapper.Mapper.Map<NUC_NuCard, NUC_NuCardDTO>(cardInDb);
              }

              if (importedCard != null)
              {
                var newCardStock = new NuCardStockItem()
                {
                  FullCardNumber = importedCard.CardNum,
                  NuCardId = importedCard.NuCardId,
                  TrackingNumber = !string.IsNullOrEmpty(importedCard.TrackingNum) ? importedCard.TrackingNum : importedCard.CardNum,
                  ExpiryDT = importedCard.ExpiryDT,
                  CardStatus = (int)NuCard.NuCardStatus.InStock
                };

                cardsImported.Add(newCardStock);

                NuCardDb.LogStockRequest(sourceRequestString, application, startDT, importedCard.CardNum,
                  NuCard.StockRequestType.ImportUnknownCard, NuCard.StockRequestResult.Successful, JsonConvert.SerializeObject(newCardStock));
              }
            }
          }
          finally
          {
            CachedValues.CardImportCompleted(newCard.FullCardNumber);
          }
        }

        _log.Information("{MethodName} completed with result: {@CardsImported}", methodName, cardsImported);
        return (int)General.WCFCallResult.OK;
      }
      catch (Exception err)
      {
        Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, null,
          NuCard.StockRequestType.ImportUnknownCard, NuCard.StockRequestResult.SystemError, err, sourceRequest);

        errorMessage = "Unexpected error on server";
        return (int)General.WCFCallResult.ServerError;
      }
    }


    #region Logging

    private static readonly ILogger _log = Log.Logger.ForContext<AtlasServer.WCF.Implementation.NuCardStockServer>();

    #endregion

  }
}

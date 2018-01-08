/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012-2013 Atlas Finance (Pty) Ltd.
* 
* 
*  Description:
*  ------------------
*    Accept a range of cards destined for a particular branch
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

using Newtonsoft.Json;
using Serilog;

using Atlas.Domain.DTO;
using Atlas.Enumerators;
using Atlas.WCF.Implementation;
using Atlas.NuCard.WCF.Interface;
using Atlas.NuCard.Repository;

#endregion


namespace AtlasServer.WCF.Stock.Implementation
{
  public static class BranchAcceptInTransitCards_Impl
  {
    public static int BranchAcceptInTransitCards(SourceRequest sourceRequest, List<NuCardStockItem> cards,
     out List<NuCardStockItem> cardsImported, out string errorMessage)
    {
      var methodName = "BranchAcceptInTransitCards";
      var startDT = DateTime.Now;
      var sourceRequestString = JsonConvert.SerializeObject(new { sourceRequest, cards });
      _log.Information("{0} starting: {1}", methodName, sourceRequestString);

      errorMessage = string.Empty;
      cardsImported = new List<NuCardStockItem>();

      PER_SecurityDTO swOperator = null;
      COR_AppUsageDTO application = null;
      BRN_BranchDTO branch = null;
      try
      {
        #region Check request
        if (!WCFUtils.CheckSourceRequest(sourceRequest, out application, out swOperator, out branch, out errorMessage))
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, null,
            NuCard.StockRequestType.BranchAcceptInTransitCards, NuCard.StockRequestResult.BadRequestParameters, new Exception(errorMessage));
          return (int)General.WCFCallResult.BadParams;
        }
        #endregion

        foreach (var card in cards)
        {
          #region Check details
          var cardFound = WCFUtils.CheckCard(card.FullCardNumber, new List<NuCard.NuCardStatus>() { NuCard.NuCardStatus.InTransit }, null, false, false, out errorMessage);
          if (cardFound != null)
          {
            var moveCard = NuCardDb.MoveCardIntoBranchStock(branch, swOperator, cardFound.NuCardId, cardFound.ClientNum,
              string.Empty, card.TrackingNumber, out errorMessage);
            if (moveCard != null)
            {
              cardsImported.Add(new NuCardStockItem()
              {
                NuCardId = moveCard.NuCardId,
                TrackingNumber = moveCard.TrackingNum,
                FullCardNumber = moveCard.CardNum,
                ExpiryDT = moveCard.ExpiryDT,
                IssueDT = moveCard.IssueDT
              });

              NuCardDb.LogStockRequest(sourceRequestString, application, startDT, null,
                NuCard.StockRequestType.BranchAcceptInTransitCards, NuCard.StockRequestResult.Successful, branch.BranchId.ToString());
            }
            else
            {
              Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, card.FullCardNumber,
                  NuCard.StockRequestType.BranchAcceptInTransitCards, NuCard.StockRequestResult.SystemError, new Exception(errorMessage), sourceRequest);
            }
          }
          else
          {
            Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, card.FullCardNumber,
                  NuCard.StockRequestType.BranchAcceptInTransitCards, NuCard.StockRequestResult.BadRequestParameters, new Exception(errorMessage), sourceRequest);
          }
          #endregion
        }

        if (!NuCardDb.TryCloseInTransitBatchForBranch(branch, swOperator, out errorMessage))
        {
          Utils.LogBadRequest(string.Format("{0}- TryCloseInTransitBatchForBranch", methodName), sourceRequestString, application, startDT, null,
              NuCard.StockRequestType.BranchAcceptInTransitCards, NuCard.StockRequestResult.SystemError, new Exception(errorMessage), sourceRequest);
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

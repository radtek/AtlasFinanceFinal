/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012-2013 Atlas Finance (Pty) Ltd.
* 
* 
*  Description:
*  ------------------
*    Get list of cards destined for a particular branch
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
using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Serilog;

using Atlas.Domain.DTO;
using Atlas.Enumerators;
using Atlas.WCF.Implementation;
using Atlas.NuCard.WCF.Interface;
using Atlas.NuCard.Repository;


namespace AtlasServer.WCF.Stock.Implementation
{
  public static class GetCardsInTransitForBranch_Impl
  {
    public static int GetCardsInTransitForBranch(SourceRequest sourceRequest, out List<NuCardStockItem> cards,
      out string errorMessage)
    {
      var methodName = "GetCardsInTransitForBranch";
      var startDT = DateTime.Now;
      var sourceRequestString = JsonConvert.SerializeObject(new { sourceRequest });
      _log.Information("{MethodName} starting: {@Request}", methodName, sourceRequest);

      errorMessage = string.Empty;
      cards = new List<NuCardStockItem>();

      PER_SecurityDTO swOperator = null;
      COR_AppUsageDTO application = null;
      BRN_BranchDTO branch = null;
      try
      {
        #region Check request
        if (!WCFUtils.CheckSourceRequest(sourceRequest, out application, out swOperator, out branch, out errorMessage))
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, null,
            NuCard.StockRequestType.GetCardsInTransitForBranch, NuCard.StockRequestResult.BadRequestParameters, new Exception(errorMessage), sourceRequest);

          return (int)General.WCFCallResult.BadParams;
        }
        #endregion

        var cardsFound = NuCardDb.GetCardsInTransitForBranch(branch, out errorMessage);
        if (cardsFound == null || cardsFound.Count == 0)
        {
          if (string.IsNullOrEmpty(errorMessage))
          {
            errorMessage = "GetCardsInTransitForBranch returned empty/null result";
          }

          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, null, NuCard.StockRequestType.GetCardsInTransitForBranch,
            NuCard.StockRequestResult.CantLocateCard, new Exception(errorMessage));
          return (int)General.WCFCallResult.ServerError;
        }

        foreach (var cardFound in cardsFound)
        {
          var cardAdded = new NuCardStockItem()
          {
            NuCardId = cardFound.NuCardId,
            TrackingNumber = cardFound.TrackingNum,
            FullCardNumber = cardFound.CardNum,
            ExpiryDT = cardFound.ExpiryDT,
            IssueDT = cardFound.IssueDT,
            CardStatus = cardFound.Status.NuCardStatusId
          };
          cards.Add(cardAdded);

          NuCardDb.LogStockRequest(sourceRequestString, application, startDT, cardFound.CardNum,
            NuCard.StockRequestType.GetCardsInTransitForBranch, NuCard.StockRequestResult.Successful,
            JsonConvert.SerializeObject(cardAdded));
        }

        _log.Information("{MethodName} completed with result: {@Cards}", methodName, cards);

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

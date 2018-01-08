/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012-2013 Atlas Finance (Pty) Ltd.
* 
* 
*  Description:
*  ------------------
*    Send a range of cards to a particular branch
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
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;
using Serilog;

using Atlas.Domain.DTO;
using Atlas.Enumerators;
using Atlas.WCF.Implementation;
using Atlas.NuCard.WCF.Interface;
using Atlas.Domain.DTO.Nucard;
using Atlas.NuCard.Repository;

#endregion


namespace AtlasServer.WCF.Stock.Implementation
{
  public static class SendCardsToBranch_Impl
  {
    public static int SendCardsToBranch(SourceRequest sourceRequest, NuCardBatchToDispatch batchDetails,
      out List<NuCardStockItem> cardsTransferred,
      out string errorMessage)
    {
      var methodName = "SendCardsToBranch";
      var startDT = DateTime.Now;
      var sourceRequestString = JsonConvert.SerializeObject(new { sourceRequest, batchDetails });
      _log.Information("{0} starting: {1}", methodName, sourceRequestString);

      errorMessage = string.Empty;
      cardsTransferred = new List<NuCardStockItem>();

      PER_SecurityDTO swOperator = null;
      COR_AppUsageDTO application = null;
      BRN_BranchDTO branch = null;
      try
      {
        #region Check request
        if (!WCFUtils.CheckSourceRequest(sourceRequest, out application, out swOperator, out branch, out errorMessage))
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, null,
            NuCard.StockRequestType.SendCardsToBranch, NuCard.StockRequestResult.BadRequestParameters, new Exception(errorMessage), sourceRequest);

          return (int)General.WCFCallResult.BadParams;
        }

        if (batchDetails.Cards == null || batchDetails.Cards.Count == 0)
        {
          errorMessage = "No cards in batch";
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, null, NuCard.StockRequestType.SendCardsToBranch,
            NuCard.StockRequestResult.BadRequestParameters, new Exception(errorMessage), sourceRequest);

          return (int)General.WCFCallResult.BadParams;
        }

        if (string.IsNullOrEmpty(batchDetails.CourierOrPersonReference))
        {
          errorMessage = "No courier/person reference given- this is required";
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, null, NuCard.StockRequestType.SendCardsToBranch,
            NuCard.StockRequestResult.BadRequestParameters, new Exception(errorMessage), sourceRequest);

          return (int)General.WCFCallResult.BadParams;
        }

        var sendTo = Atlas.Data.Repository.AtlasData.FindBranch(batchDetails.DestinationBranchNum);
        if (sendTo == null)
        {
          errorMessage = string.Format("Invalid destination branch: ", batchDetails.DestinationBranchNum);
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, null, NuCard.StockRequestType.SendCardsToBranch,
            NuCard.StockRequestResult.BadRequestParameters, new Exception(errorMessage), sourceRequest);

          return (int)General.WCFCallResult.BadParams;
        }
        #endregion

        var cardsToSend = batchDetails.Cards.Select(s => new NUC_NuCardDTO()
        {
          NuCardId = s.NuCardId,
          CardNum = s.FullCardNumber,
          TrackingNum = s.TrackingNumber
        }).ToList();
        var sent = NuCardDb.SendCardsToBranch(swOperator, sendTo, batchDetails.CourierOrPersonReference, batchDetails.Comment, cardsToSend, out errorMessage);
        if (sent == null || sent.Count == 0)
        {
          if (string.IsNullOrEmpty(errorMessage))
          {
            errorMessage = "SendCardsToBranch returned an empty result set";
          }
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, null,
            NuCard.StockRequestType.SendCardsToBranch, NuCard.StockRequestResult.SystemError, new Exception(errorMessage), sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        _log.Information("{MethodName} completed with result: {@CardsTransferred}", methodName, cardsTransferred);

        NuCardDb.LogStockRequest(sourceRequestString, application, startDT, null,
          NuCard.StockRequestType.SendCardsToBranch, NuCard.StockRequestResult.Successful, JsonConvert.SerializeObject(cardsTransferred));

        return (int)General.WCFCallResult.OK;
      }
      catch (Exception err)
      {
        Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, null,
          NuCard.StockRequestType.ImportUnknownCard, NuCard.StockRequestResult.SystemError, err, sourceRequest);

        errorMessage = Atlas.Server.NuCard.WCF.Implementation.Consts.ERR_SERVER_GENERIC;
        return (int)General.WCFCallResult.ServerError;
      }   
    }

    #region Logging

    private static readonly ILogger _log = Log.Logger.ForContext<AtlasServer.WCF.Implementation.NuCardStockServer>();

    #endregion

  }
}

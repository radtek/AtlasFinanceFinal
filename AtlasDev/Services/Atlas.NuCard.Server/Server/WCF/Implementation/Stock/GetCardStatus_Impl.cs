using System;
using System.Linq;

using Serilog;
using Newtonsoft.Json;

using Atlas.Domain.DTO;
using Atlas.WCF.Implementation;
using Atlas.NuCard.WCF.Interface;
using Atlas.Enumerators;
using Atlas.NuCard.Repository;


namespace AtlasServer.WCF.Stock.Implementation
{
  public static class GetCardStatus_Impl
  {
    public static int GetCardStatus(SourceRequest sourceRequest, NuCardStockItem card, out int cardStatus, out string errorMessage)
    {
      var methodName = "GetCardStatus";
      var startDT = DateTime.Now;
      var sourceRequestString = JsonConvert.SerializeObject(new { sourceRequest, card },
        Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
      _log.Information("{MethodName} starting: {@Request}", methodName, sourceRequest);

      cardStatus = -1;
      errorMessage = string.Empty;
      PER_SecurityDTO swOperator = null;
      COR_AppUsageDTO application = null;
      BRN_BranchDTO branch = null;
      try
      {
        #region Check request
        if (!WCFUtils.CheckSourceRequest(sourceRequest, out application, out swOperator, out branch, out errorMessage))
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, card.FullCardNumber,
            NuCard.StockRequestType.GetCardStatus, NuCard.StockRequestResult.BadRequestParameters, new Exception(errorMessage), sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }

        var cardFound = WCFUtils.CheckCard(card.FullCardNumber, Enum.GetValues(typeof(NuCard.NuCardStatus)).Cast<NuCard.NuCardStatus>().Where(s => s != NuCard.NuCardStatus.NotSet).ToList(),
          null, null, false, out errorMessage);
        if (cardFound == null)
        {
          Utils.LogBadRequest(methodName, sourceRequestString, application, startDT, card.FullCardNumber,
            NuCard.StockRequestType.GetCardStatus, NuCard.StockRequestResult.BadRequestParameters, new Exception(errorMessage), sourceRequest);
          return (int)General.WCFCallResult.BadParams;
        }
        #endregion

        NuCardDb.LogStockRequest(sourceRequestString, application, startDT, card.FullCardNumber,
            NuCard.StockRequestType.GetCardStatus, NuCard.StockRequestResult.Successful,
            JsonConvert.SerializeObject(cardFound, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));

        _log.Information("{MethodName}]completed successfully with result: {Description}", methodName, cardFound.Status.Description);
        cardStatus = (int)cardFound.Status.NuCardStatusId;
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

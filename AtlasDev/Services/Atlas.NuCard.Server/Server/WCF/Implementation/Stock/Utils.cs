using System;

using Serilog;

using Atlas.Domain.DTO;
using Atlas.Enumerators;
using Atlas.NuCard.Repository;
using Atlas.NuCard.WCF.Interface;


namespace AtlasServer.WCF.Stock.Implementation
{
  public static class Utils
  {
    /// <summary>
    /// Logs a bad request
    /// </summary>
    /// <param name="request">Source request parameters</param>
    /// <param name="errorText">NuCard XML-RPC method called</param>
    public static void LogBadRequest(string methodName, string sourceRequestString, COR_AppUsageDTO appUsage, DateTime startDT, string cardNum,
      NuCard.StockRequestType requestType, NuCard.StockRequestResult result, Exception error, SourceRequest request = null)
    {
      Log.Error(error, "{MethodName}- {@Request}", methodName, request);

      NuCardDb.LogStockRequest(requestParams: sourceRequestString, appUsage: appUsage, startDT: startDT, cardNum: cardNum,
        requestType: requestType, requestResult: result, resultText: error != null ? error.Message : null);
    }    
   
  }
}

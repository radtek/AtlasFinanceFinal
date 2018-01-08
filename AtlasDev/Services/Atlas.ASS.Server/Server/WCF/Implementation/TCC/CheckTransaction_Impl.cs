using System;
using System.ServiceModel;

using Atlas.Cache.Interfaces;
using Atlas.Domain.DTO;

using Atlas.Common.Extensions;
using Atlas.Enumerators;
using Atlas.Server.WCF.Utils;
using Atlas.Common.Interface;
using Atlas.Server.Utils;


namespace Atlas.Server.WCF.Implementation.TCC
{
  public static class CheckTransaction_Impl
  {
    /// <summary>
    /// Returns true if transaction exists for client with specific ID and contract number
    /// </summary>
    /// <param name="terminal">Terminal DTO</param>
    /// <param name="clientIDNumber">Clients Identity number</param>
    /// <param name="clientContractNumber">Contract number</param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    internal static tranIDResp Execute(ILogging log, IConfigSettings config, ICacheServer cache,
      TimeSpan sendTimeout, TimeSpan receiveTimeout,
      TCCTerminalDTO terminal, string clientIDNumber, string clientContractNumber, out string errorMessage)
    {
      var requestStarted = DateTime.Now;

      errorMessage = null;
      var responseCode = string.Empty;
      var methodName = "CheckTransaction";
      TccCacheUtils.SetTerminalBusy(cache, terminal.TerminalId, "CheckTransaction");

      try
      {
        try
        {
          tranIDResp query = null;
          var binding = new BasicHttpBinding("TermRCSoap") { SendTimeout = sendTimeout, ReceiveTimeout = receiveTimeout };
          var endpointAddress = TccSoapUtils.NPTerminalRC_EP(config);
          new TermRCSoapClient(binding, endpointAddress).Using(client =>
          {
            query = client.tranIDQuery(DateTime.Now.ToString("yyyyMMdd"), terminal.MerchantId, clientIDNumber, clientContractNumber);
          });

          if (query != null && !string.IsNullOrEmpty(query.responseCode))
          {
            if (query.responseCode.Trim().StartsWith("00"))
            {
              DbGeneralUtils.LogTCCRequest(terminal.TerminalId, General.TCCLogRequestType.TranIDQuery,
                  requestStarted, string.Empty, General.TCCLogRequestResult.Successful, null, DateTime.Now);

              return query;
            }
            else
            {
              errorMessage = ErrorCodes.GetTranIDQueryErrorString(log, query.responseCode);
              responseCode = query.responseCode;
              DbGeneralUtils.LogTCCRequest(terminal.TerminalId, General.TCCLogRequestType.TranIDQuery,
                  requestStarted, string.Empty, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);

              return null;
            }
          }
          else
          {
            errorMessage = "TCC Server did not return a response";
            DbGeneralUtils.LogTCCRequest(terminal.TerminalId, General.TCCLogRequestType.TranIDQuery,
                  requestStarted, string.Empty, General.TCCLogRequestResult.CommunicationsError, errorMessage, DateTime.Now);
            return null;
          }
        }
        catch (Exception err)
        {
          log.Error(err, methodName);
          errorMessage = err.Message;
          DbGeneralUtils.LogTCCRequest(terminal.TerminalId, General.TCCLogRequestType.TranIDQuery,
                      requestStarted, string.Empty, General.TCCLogRequestResult.ApplicationError, errorMessage, DateTime.Now);

          return null;
        }
      }
      finally
      {
        TccCacheUtils.SetTerminalDone(cache, terminal.TerminalId, responseCode);
      }
    }

  }
}

using System;
using System.ServiceModel;

using Atlas.Common.Interface;

using Atlas.Enumerators;
using Atlas.Common.Extensions;

using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;


namespace Atlas.Server.WCF.Implementation.TCC
{
  internal static class FlashMessage_Impl
  {
    internal static void Execute(ILogging log, IConfigSettings config, ICacheServer cache, 
      int terminalID, int displaySeconds, string[] lines)
    {
      var methodName = "FlashMessage";
      var requestStarted = DateTime.Now;
      var inputRequest = string.Format("Terminal: {0}, DisplaySeconds: {1}, Lines: {2}",
          terminalID, displaySeconds, string.Join(", ", lines));

      #region Check terminal is ready
      var terminal = cache.Get<TCCTerminal_Cached>(terminalID);
      if (terminal == null || terminal.Status != 0)
      {
        return;
      }
      #endregion

      if (lines.Length != 6)
      {
        var errorMessage = "FlashMessage called with an invalid number of display lines- parameter 'lines'";
        log.Warning(errorMessage);

        Atlas.Server.Utils.DbGeneralUtils.LogTCCRequest(terminalID, General.TCCLogRequestType.FlashMessage,
            requestStarted, inputRequest, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);

        return;
      }

      if (displaySeconds > 500 || displaySeconds < 1)
      {
        var errorMessage = "FlashMessage called with an invalid 'displaySeconds' parameter";
        log.Warning(errorMessage);

        Atlas.Server.Utils.DbGeneralUtils.LogTCCRequest(terminalID, General.TCCLogRequestType.FlashMessage,
            requestStarted, inputRequest, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);

        return;
      }

      Atlas.Server.Utils.TccCacheUtils.SetTerminalBusy(cache, terminalID, "FlashMessage");
      try
      {
        try
        {
          var timeout = new TimeSpan(0, 0, displaySeconds + 1);
          var binding = new BasicHttpBinding("TermRCSoap") { SendTimeout = timeout, ReceiveTimeout = timeout };
          var endpointAddress = Server.Utils.TccSoapUtils.NPTerminalRC_EP(config);
          new TermRCSoapClient(binding, endpointAddress).Using(client =>
          {
            client.displayMessage(terminal.MerchantId, terminal.SupplierTerminalId,
                lines[0], lines[1], lines[2], lines[3], lines[4], lines[5], 'm', displaySeconds);
          });

          Atlas.Server.Utils.DbGeneralUtils.LogTCCRequest(terminalID, General.TCCLogRequestType.FlashMessage,
                requestStarted, inputRequest, General.TCCLogRequestResult.Successful, null, DateTime.Now);
        }
        catch (Exception err)
        {
          log.Error(err, methodName);

          Atlas.Server.Utils.DbGeneralUtils.LogTCCRequest(terminalID, General.TCCLogRequestType.FlashMessage,
              requestStarted, inputRequest, General.TCCLogRequestResult.ApplicationError, err.Message, DateTime.Now);
        }
      }
      finally
      {
        Atlas.Server.Utils.TccCacheUtils.SetTerminalDone(cache, terminalID);
      }
    }

  }
}

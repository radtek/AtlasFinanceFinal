using System;
using System.ServiceModel;

using Atlas.Common.Extensions;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;

using Atlas.Cache.Interfaces.Classes;


namespace Atlas.Server.WCF.Implementation.TCC
{
  internal static class UpfrontBinCheck_Impl
  {
    internal static bool Execute(ILogging log, IConfigSettings config, ICacheServer cache, 
      int terminalID, int timeoutSeconds, out string errorMessage)
    {     
      errorMessage = null;
      var methodName = "UpfrontBinCheck";

      log.Information("{MethodName} started: {TerminalId} {TimeoutSeconds}", methodName, terminalID, timeoutSeconds);

      #region Check terminal is ready
      var terminal = cache.Get<TCCTerminal_Cached>(terminalID);
      if (terminal == null)
      {
        errorMessage = "Specified terminal could not found in system";
        log.Warning(new Exception(errorMessage), methodName);

        return false;
      }
      if (terminal.Status != 0)
      {
        errorMessage = "Terminal is not ready- if currently not in use, please reset";
        log.Warning(new Exception(errorMessage), methodName);

        return false;
      }
      #endregion

      #region Check parameters
      if (timeoutSeconds <= 0)
      {
        errorMessage = "timeoutSeconds not specified";
        return false;
      }
      if (timeoutSeconds > 600)
      {
        errorMessage = "timeoutSeconds cannot exceed 600";
        return false;
      }
      #endregion

      BinCheck binCheck = null;
      Atlas.Server.Utils.TccCacheUtils.SetTerminalBusy(cache, terminalID, "UpfrontBinCheck");
      try
      {
        timeoutSeconds = timeoutSeconds.RoundOff(60);
        var upfrontBinCheckTimeout = new TimeSpan(0, 0, timeoutSeconds - 3);
        try
        {
          var binding = new BasicHttpBinding("TermRCSoap") { SendTimeout = upfrontBinCheckTimeout, ReceiveTimeout = upfrontBinCheckTimeout };
          var endpointAddress = Server.Utils.TccSoapUtils.NPTerminalRC_EP(config);
          new TermRCSoapClient(binding, endpointAddress).Using(client =>
            {
              binCheck = client.UpfrontBinCheck(terminal.MerchantId, terminal.SupplierTerminalId, timeoutSeconds);
            });

          if (binCheck != null && !string.IsNullOrEmpty(binCheck.ResponseCode))
          {
            if (binCheck.ResponseCode.Trim().StartsWith("00"))
            {
              return true;
            }
            else
            {
              errorMessage = string.Format("'{0}'- {1}", binCheck.result, Atlas.Server.WCF.Utils.ErrorCodes.GetUpfrontBinCheckErrorString(log, binCheck.ResponseCode));
              return false;
            }
          }
          else
          {
            errorMessage = "TCC did not return a value"; ; ;
            return false;
          }
        }
        catch (Exception err)
        {
          log.Error(err, methodName);
          errorMessage = "Unexpected error communicating with terminal cloud control";
          return false;
        }
      }
      finally
      {
        Atlas.Server.Utils.TccCacheUtils.SetTerminalDone(cache, terminalID, binCheck != null ? binCheck.result : string.Empty);
      }
    }
    
  }
}

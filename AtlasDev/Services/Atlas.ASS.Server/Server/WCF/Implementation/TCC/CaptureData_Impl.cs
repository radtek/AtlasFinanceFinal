using System;
using System.ServiceModel;

using Atlas.Common.Extensions;
using Atlas.Enumerators;
using Atlas.Common.Interface;

using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;
using Atlas.Server.Utils;


namespace Atlas.Server.WCF.Implementation.TCC
{
  internal static class CaptureData_Impl
  {
    internal static bool Execute(ILogging log, IConfigSettings config, ICacheServer cache,
      int terminalID, int displaySeconds, int timeoutSeconds,
      int inputType, string prompTextline1, string prompTextline2, string prompTextline3, string prompTextline4,
      out string errorMessage, out string capturedData)
    {
      var methodName = "CaptureData";
      log.Information("{MethodName} started: {TerminalId}, {TimeoutSeconds}", terminalID, timeoutSeconds);

      var requestStarted = DateTime.Now;
      var inputRequest = string.Format("Terminal: {0}, DisplaySeconds: {1}, TimeoutSeconds: {2}, InputType: {3}, Line1: {4}, " +
          "Line2: {5}, Line3: {6}, Line4: {7}", terminalID, displaySeconds, timeoutSeconds, inputType, prompTextline1,
          prompTextline2, prompTextline3, prompTextline4);

      var result = false;
      errorMessage = null;
      capturedData = null;

      #region Check parameters
      if (timeoutSeconds < 10 || timeoutSeconds > 600)
      {
        errorMessage = string.Format("Invalid value for timeoutSeconds (10-600): {0}", timeoutSeconds);
        DbGeneralUtils.LogTCCRequest(terminalID, General.TCCLogRequestType.CaptureData,
            requestStarted, inputRequest, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);

        return false;
      }
      timeoutSeconds = timeoutSeconds.RoundOff(60);

      if (prompTextline1.Length > 0)
      {
        prompTextline1 = Uri.UnescapeDataString(prompTextline1);
      }
      if (prompTextline2.Length > 0)
      {
        prompTextline2 = Uri.UnescapeDataString(prompTextline2);
      }
      if (prompTextline3.Length > 0)
      {
        prompTextline3 = Uri.UnescapeDataString(prompTextline3);
      }
      if (prompTextline4.Length > 0)
      {
        prompTextline4 = Uri.UnescapeDataString(prompTextline4);
      }
      if (prompTextline1.Length == 0 && prompTextline2.Length == 0 && prompTextline3.Length == 0 && prompTextline4.Length == 0)
      {
        errorMessage = "No text to display!";
        DbGeneralUtils.LogTCCRequest(terminalID, General.TCCLogRequestType.CaptureData,
            requestStarted, inputRequest, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);

        return false;
      }
      // Text cannot contain the word PIN
      var checkPIN = new System.Text.RegularExpressions.Regex(@"\b(PIN|pin|Pin)\b");
      if (checkPIN.IsMatch(prompTextline1) || checkPIN.IsMatch(prompTextline2) || checkPIN.IsMatch(prompTextline3) || checkPIN.IsMatch(prompTextline4))
      {
        errorMessage = "Text to display contains the word 'PIN'- this is not permitted";
        DbGeneralUtils.LogTCCRequest(terminalID, General.TCCLogRequestType.CaptureData,
            requestStarted, inputRequest, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);

        return false;
      }

      if (inputType < 1 || inputType > 4)
      {
        errorMessage = string.Format("Unknown input type: '{0}'", inputType);
        log.Warning("CaptureData called with invalid inputType parameter: {InputType}", inputType);
        DbGeneralUtils.LogTCCRequest(terminalID, General.TCCLogRequestType.CaptureData,
            requestStarted, inputRequest, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);

        return false;
      }
      #endregion

      #region Check terminal is ready
      var terminal = cache.Get<TCCTerminal_Cached>(terminalID);
      if (terminal == null)
      {
        errorMessage = "Specified terminal not in system";
        DbGeneralUtils.LogTCCRequest(terminalID, General.TCCLogRequestType.CaptureData,
            requestStarted, inputRequest, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);

        return false;
      }

      if (terminal.Status != 0)
      {
        switch (terminal.Status)
        {
          case 1:
            errorMessage = "The TCC Terminal is currently in use with another function";
            break;

          case 2:
            errorMessage = "The TCC Terminal is currently unresponsive- please reset TCC unit";
            break;

          default:
            errorMessage = "The TCC Terminal is not configured appropriately";
            break;
        }

        DbGeneralUtils.LogTCCRequest(terminalID, General.TCCLogRequestType.CaptureData,
            requestStarted, inputRequest, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);

        return false;
      }
      #endregion

      genDataCaptRsp requestResult = null;
      TccCacheUtils.SetTerminalBusy(cache, terminalID, "genericDataCaptureConfirm");
      try
      {
        try
        {
          var captureDataTimeout = new TimeSpan(0, 0, timeoutSeconds - 3);
          var binding = new BasicHttpBinding("TermRCSoap") { SendTimeout = captureDataTimeout, ReceiveTimeout = captureDataTimeout };
          var endpointAddress = TccSoapUtils.NPTerminalRC_EP(config);
          new TermRCSoapClient(binding, endpointAddress).Using(client =>
          {
            requestResult = client.genericDataCaptureConfirm(merchant_ID: terminal.MerchantId,
                term_ID: terminal.SupplierTerminalId,
                inputType: inputType,
                promptTextLine1: prompTextline1,
                prompTextline2: prompTextline2,
                prompTextline3: prompTextline3,
                prompTextline4: prompTextline4,
                GDCTimeout: timeoutSeconds);
          });

          if (requestResult != null && !string.IsNullOrEmpty(requestResult.responseCode))
          {
            if (requestResult.responseCode.Trim().StartsWith("00"))
            {
              capturedData = requestResult.dataCaptured;
              DbGeneralUtils.LogTCCRequest(terminalID, General.TCCLogRequestType.CaptureData,
                  requestStarted, inputRequest, General.TCCLogRequestResult.Successful, null, DateTime.Now);

              result = true;
            }
            else
            {
              errorMessage = "No data was entered at the terminal";
              DbGeneralUtils.LogTCCRequest(terminalID, General.TCCLogRequestType.CaptureData,
                  requestStarted, inputRequest, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);
            }
          }
          else
          {
            errorMessage = Atlas.Server.WCF.Utils.ErrorCodes.GetGenericDataCapture(log, requestResult != null ? requestResult.responseCode : "No response");
            DbGeneralUtils.LogTCCRequest(terminalID, General.TCCLogRequestType.CaptureData,
                requestStarted, inputRequest, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);
          }
        }
        catch (Exception err)
        {
          log.Error(err, methodName);

          errorMessage = "Unexpected error communicating with terminal cloud control";
          DbGeneralUtils.LogTCCRequest(terminalID, General.TCCLogRequestType.CaptureData,
              requestStarted, inputRequest, General.TCCLogRequestResult.ApplicationError, err.Message, DateTime.Now);
        }
      }
      finally
      {
        TccCacheUtils.SetTerminalDone(cache, terminalID, requestResult != null ? requestResult.responseCode : string.Empty);
      }

      return result;
    }

  }
}

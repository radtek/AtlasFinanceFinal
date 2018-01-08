using System;
using System.Globalization;
using System.ServiceModel;
using System.Threading.Tasks;

using Atlas.Common.Extensions;
using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;
using Atlas.Common.Interface;
using Atlas.Domain.DTO;
using Atlas.Enumerators;
using Atlas.Server.WCF.Utils;


namespace Atlas.Server.Utils
{
  /// <summary>
  /// TCC SOAP utilities
  /// </summary>
  public static class TccSoapUtils
  {
    /// <summary>
    /// Determines via a handshake, that the terminal is up
    /// If terminal is busy processing a request, will return false.
    /// Use checkEvenIfBusy to check status even if in the middle of a transaction. This should only be done if
    /// trying to determine if terminal status is 'stuck' and only after 10 minutes since last request...
    /// else can cause unit to freeze
    /// </summary>
    /// <param name="terminal">Terminal DTO</param>
    /// <param name="checkEvenIfBusy">Force a check even if terminal is busy</param>
    /// <param name="errorMessage">Error message from request</param>
    /// <returns>true of obtained normal response, false if got unexpected/bad response</returns>
    internal static bool TerminalReady(ICacheServer cache, IConfigSettings config, ILogging log,
      TCCTerminal_Cached terminal, bool checkEvenIfBusy, out string errorMessage)
    {
      errorMessage = string.Empty;
      var result = false;
      var requestStarted = DateTime.Now;
      var logResult = General.TCCLogRequestResult.Successful;
      var methodName = "TerminalReady";

      if (checkEvenIfBusy || terminal.Status == 0) // !!!! DO NOT TRY ANY web method, if we are busy with this terminal  !!!
      {
        HandShakeRsp requestResult = null;
        TccCacheUtils.SetTerminalBusy(cache, terminal.TerminalId, "handshake");
        try
        {
          try
          {
            var binding = new BasicHttpBinding("TermRCSoap") { SendTimeout = HandshakeTimeout, ReceiveTimeout = HandshakeTimeout };
            new TermRCSoapClient(binding, NPTerminalRC_EP(config)).Using(client =>
            {
              requestResult = client.handshake(terminal.MerchantId, terminal.SupplierTerminalId);
            });

            if (requestResult != null && !string.IsNullOrEmpty(requestResult.status))
            {
              if (requestResult.status.StartsWith("00"))
              {
                result = true;
              }
              else
              {
                errorMessage = ErrorCodes.GetHandShakeErrorString(log, requestResult.status);
                logResult = General.TCCLogRequestResult.Unsuccessful;
              }
            }
            else
            {
              errorMessage = "No response from TCC server";
              logResult = General.TCCLogRequestResult.CommunicationsError;
            }
          }
          catch (Exception err)
          {
            log.Warning(err, methodName);
            errorMessage = "Unexpected error communicating with terminal cloud control";
            logResult = General.TCCLogRequestResult.ApplicationError;
          }
        }
        finally
        {
          TccCacheUtils.SetTerminalDone(cache, terminal.TerminalId, null);//requestResult.status); // Handshake must not override last response- hides issues                                                       
        }
      }
      else
      {
        errorMessage = "Terminal currently busy";
        logResult = General.TCCLogRequestResult.Unsuccessful;
      }

      DbGeneralUtils.LogTCCRequest(terminal.TerminalId, General.TCCLogRequestType.Handshake,
        requestStarted, string.Empty, logResult, errorMessage, DateTime.Now);

      return result;
    }


    /// <summary>
    /// Checks the poll status of the terminal- fast and safe to call at any time- 
    /// merely an extract from Altech SQL table
    /// </summary>
    /// <param name="terminal"></param>
    /// <param name="lastComms"></param>
    /// <param name="responseCode"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    internal static TCCStatus TerminalReadySafe(ILogging log, IConfigSettings config,
      TCCTerminalDTO terminal, out DateTime lastComms, out string responseCode, out string errorMessage)
    {
      var requestStarted = DateTime.Now;

      lastComms = DateTime.MinValue;
      errorMessage = null;
      responseCode = null;
      var methodName = "TerminalReadySafe";
      TermStatus status = null;
      try
      {
        var binding = new BasicHttpBinding("TermRCSoap") { SendTimeout = TerminalReadySafeTimeout, ReceiveTimeout = TerminalReadySafeTimeout };
        new TermRCSoapClient(binding, NPTerminalRC_EP(config)).Using(client =>
        {
          status = client.TermStatusCheck(terminal.MerchantId, terminal.SupplierTerminalId);
        });

        if (status != null && !string.IsNullOrEmpty(status.responseCode))
        {
          DateTime.TryParseExact(status.lastUpdated, "yyyy/MM/dd HH:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out lastComms);
          responseCode = status.responseCode;

          if (status.responseCode.Trim().StartsWith("00"))
          {
            // status.status = 'F' = ?  'O' = Online ?            
            DbGeneralUtils.LogTCCRequest(terminal.TerminalId, General.TCCLogRequestType.TermStatusCheck,
                requestStarted, string.Empty, General.TCCLogRequestResult.Successful,
                status.status.Trim().ToUpper(), DateTime.Now);

            return status.status.Trim().ToUpper().Equals("O") ? TCCStatus.Online : TCCStatus.Offline;
          }
          else
          {
            errorMessage = ErrorCodes.GetHandShakeErrorString(log, status.responseCode);
            DbGeneralUtils.LogTCCRequest(terminal.TerminalId, General.TCCLogRequestType.TermStatusCheck,
                requestStarted, string.Empty, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);

            return TCCStatus.Unknown;
          }
        }
        else
        {
          errorMessage = "TCC service did not return with a response";
          DbGeneralUtils.LogTCCRequest(terminal.TerminalId, General.TCCLogRequestType.TermStatusCheck,
              requestStarted, string.Empty, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);

          return TCCStatus.Unknown;
        }
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        errorMessage = "Unexpected error communicating with terminal cloud control";
        DbGeneralUtils.LogTCCRequest(terminal.TerminalId, General.TCCLogRequestType.TermStatusCheck,
                            requestStarted, string.Empty, General.TCCLogRequestResult.ApplicationError, err.Message, DateTime.Now);

        return TCCStatus.Unknown;
      }
    }


    /// <summary>
    /// Aynchronously checks the poll status of the terminal- fast and safe to call at any time- 
    /// merely an extract from Altech SQL table
    /// </summary>
    /// <param name="terminal"></param>
    /// <param name="lastComms"></param>
    /// <param name="responseCode"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    internal static async Task<TCCCheckReadyResult> TerminalReadySafeAsync(ILogging log, IConfigSettings config, TCCTerminal_Cached terminal)
    {
      var requestStarted = DateTime.Now;
      var methodName = "TerminalReadySafeAsync";
      TermStatus status = null;
      try
      {
        var binding = new BasicHttpBinding("TermRCSoap") { SendTimeout = TerminalReadySafeTimeout, ReceiveTimeout = TerminalReadySafeTimeout };

        var client = new TermRCSoapClient(binding, NPTerminalRC_EP(config));
        try
        {
          status = await client.TermStatusCheckAsync(terminal.MerchantId, terminal.SupplierTerminalId);
        }
        catch (CommunicationException)
        {
          client.Abort();
          throw;
        }
        catch (TimeoutException)
        {
          client.Abort();
          throw;
        }
        catch (Exception)
        {
          client.Abort();
          throw;
        }

        if (status != null && !string.IsNullOrEmpty(status.responseCode))
        {
          DateTime lastComms;
          DateTime.TryParseExact(status.lastUpdated, "yyyy/MM/dd HH:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out lastComms);
          var responseCode = status.responseCode;

          if (status.responseCode.Trim().StartsWith("00"))
          {
            // status.status = 'F' = ?  'O' = Online ?            
            DbGeneralUtils.LogTCCRequest(terminal.TerminalId, General.TCCLogRequestType.TermStatusCheck,
                requestStarted, string.Empty, General.TCCLogRequestResult.Successful,
                status.status.Trim().ToUpper(), DateTime.Now);

            return new TCCCheckReadyResult { ResponseCode = responseCode, Status = status.status.Trim().ToUpper().Equals("O") ? TCCStatus.Online : TCCStatus.Offline, LastComms = lastComms };
          }
          else
          {
            var errorMessage = ErrorCodes.GetHandShakeErrorString(log, status.responseCode);
            DbGeneralUtils.LogTCCRequest(terminal.TerminalId, General.TCCLogRequestType.TermStatusCheck,
                requestStarted, string.Empty, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);

            return new TCCCheckReadyResult { ErrorMessage = errorMessage, Status = TCCStatus.Unknown };
          }
        }
        else
        {
          var errorMessage = "TCC service did not return with a response";
          DbGeneralUtils.LogTCCRequest(terminal.TerminalId, General.TCCLogRequestType.TermStatusCheck,
              requestStarted, string.Empty, General.TCCLogRequestResult.Unsuccessful, errorMessage, DateTime.Now);

          return new TCCCheckReadyResult { ErrorMessage = errorMessage, Status = TCCStatus.Unknown };
        }
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        var errorMessage = "Unexpected error communicating with terminal cloud control";
        DbGeneralUtils.LogTCCRequest(terminal.TerminalId, General.TCCLogRequestType.TermStatusCheck,
                            requestStarted, string.Empty, General.TCCLogRequestResult.ApplicationError, err.Message, DateTime.Now);

        return new TCCCheckReadyResult { ErrorMessage = errorMessage, Status = TCCStatus.Unknown };
      }
    }
        

    /// <summary>
    /// TCC 'Terminal ready' time-out
    /// </summary>
    private static readonly TimeSpan TerminalReadySafeTimeout = new TimeSpan(0, 0, 5);


    // Time-outs
    /// <summary>
    /// TCC 'Handshake. call time-out
    /// </summary>
    private static readonly TimeSpan HandshakeTimeout = new TimeSpan(0, 0, 20);

    
    // End point address
    internal static EndpointAddress NPTerminalRC_EP(IConfigSettings config)
    {
      return new EndpointAddress(config.GetCustomSetting("", "NPTerminalURL", false));
    }
    

    #region Enums

    internal enum TCCStatus { Online, Busy, Offline, Unknown };

    #endregion

    
    internal class TCCCheckReadyResult
    {
      public TCCStatus Status { get; set; }

      public DateTime LastComms { get; set; }
      public string ResponseCode { get; set; }
      public string ErrorMessage { get; set; }
    }
    
  }
}

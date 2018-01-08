using System;

using Atlas.Enumerators;
using Atlas.Common.Interface;
using Atlas.Common.OTP;
using Atlas.Common.Utils;
using AtlasServer.WCF.Interface;
using Atlas.Server.Classes.CustomException;


namespace Atlas.Server.WCF.Implementation.ASS
{
  internal static class SendOTP_Impl
  {
    internal static int Execute(ILogging log,
      SourceRequest sourceRequest,
      string cellNumber, string otpPrefixText, string otpSuffixText, out string otpReference, out string errorMessage)
    {
      otpReference = string.Empty;
      errorMessage = string.Empty;
      var methodName = "SendOTP";
      try
      {
        #region Check params
        if (string.IsNullOrEmpty(sourceRequest.BranchCode))
        {
          throw new BadParamException("Missing branch number");
        }

        if (string.IsNullOrEmpty(sourceRequest.UserIDOrPassport))
        {
          throw new BadParamException("Missing operator ID");
        }

        if (string.IsNullOrEmpty(sourceRequest.MachineUniqueID))
        {
          throw new BadParamException("Missing machine fingerprint");
        }

        if (string.IsNullOrEmpty(sourceRequest.AppVer))
        {
          errorMessage = "Missing application version";
          return (int)General.WCFCallResult.BadParams;
        }

        if (string.IsNullOrEmpty(cellNumber))
        {
          throw new BadParamException("Missing cellNumbner parameter");
        }
        #endregion

        // Generate a timed OTP that lasts 15 mins
        otpReference = StringUtils.RandomBase32();
        var otp = new TOTP(otpReference, 900, 5).Now().ToString();
        var smsMessage = $"{otpPrefixText}{otp}{otpSuffixText}";

        log.Information("Sending SMS to: {CellNumber}- '{SMSMessage}'", cellNumber, smsMessage);
        return SendSMS_Impl.Execute(log, sourceRequest, new string[] { cellNumber }, new string[] { smsMessage }, out errorMessage);
      }
      catch (Exception err)
      {
        errorMessage = (err is BadParamException) ? errorMessage = err.Message : "Unexpected server error";
        log.Error(err, methodName);
        return (err is BadParamException) ? (int)General.WCFCallResult.BadParams : (int)General.WCFCallResult.ServerError;
      }
    }

  }
}

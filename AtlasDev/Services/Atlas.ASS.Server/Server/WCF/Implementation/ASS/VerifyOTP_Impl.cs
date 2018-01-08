using System;

using Atlas.Common.Interface;
using Atlas.Enumerators;
using Atlas.Common.OTP;
using AtlasServer.WCF.Interface;
using Atlas.Server.Classes.CustomException;


namespace Atlas.Server.WCF.Implementation.ASS
{
  internal static class VerifyOTP_Impl
  {
    internal static int Execute(ILogging log,
      SourceRequest sourceRequest, string otpReference, int otpEntered, out string errorMessage)
    {
      errorMessage = string.Empty;
      var methodName = "VerifyOTP";
      try
      {
        #region Check parameters
        if (string.IsNullOrEmpty(sourceRequest.BranchCode))
        {
          throw new BadParamException("Missing branch number");
        }

        if (string.IsNullOrEmpty(sourceRequest.UserIDOrPassport))
        {
          throw new BadParamException($"Missing operator ID");         
        }

        if (string.IsNullOrEmpty(sourceRequest.MachineUniqueID))
        {
          throw new BadParamException("Missing machine fingerprint");       
        }

        if (string.IsNullOrEmpty(sourceRequest.AppVer))
        {
          throw new BadParamException("Missing application version");
        }

        if (string.IsNullOrEmpty(otpReference))
        {
          throw new BadParamException("Missing otpReference parameter");
        }

        if (otpEntered <= 0)
        {
          throw new BadParamException("Missing otpEntered parameter");
        }
        #endregion

        log.Information("Verifying OTP {0} with reference '{1}'", otpEntered, otpReference);
        var otpVerified = new TOTP(otpReference, 900).Verify(otpEntered);

        if (!otpVerified)
        {
          errorMessage = "The supplied OTP does not match/has expired";
          log.Warning("Mismatched OTP: Entered: {OTPEntered}, Reference: {OTPReference}", otpEntered, otpReference);
        }

        return otpVerified ? (int)General.WCFCallResult.OK : (int)General.WCFCallResult.BadParams;
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        errorMessage = (err is BadParamException) ? err.Message : "Unexpected server error";
        return (err is BadParamException) ? (int)General.WCFCallResult.BadParams : (int)General.WCFCallResult.ServerError;
      }
    }

  }
}

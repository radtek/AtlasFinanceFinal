using System;

using Atlas.Integration.Interface;
using Atlas.Server.Implementation.Token;
using Atlas.Common.Interface;


namespace Atlas.Server.Implementation
{
  public class SendOTPViaSMS_Impl
  {
    public static SendOTPResult SendOTPViaSMS(ILogging log, Atlas.Server.Implementation.MessageBus.IMessageBusHandler messageBus, string loginToken, SendOTPRequest request)
    {
      try
      {
        log.Information("[SendOTPViaSMS]- {LoginToken}-{@request}", loginToken, request);

        #region Basic validation
        if (string.IsNullOrEmpty(loginToken))
        {
          return new SendOTPResult() { Error = "Token cannot be empty- login first!" };
        }
        string userId;
        string branch;
        if (!UserToken.TryGetUserInfo(loginToken, out userId, out branch))
        {
          return new SendOTPResult() { Error = "Login token invalid/has expired" };
        }

        if (request == null)
        {
          return new SendOTPResult() { Error = "Parameter 'request' cannot be empty" };
        }

        if (string.IsNullOrEmpty(request.CellularNumber) || request.CellularNumber.Length != 10)
        {
          return new SendOTPResult() { Error = "Parameter 'request.CellularNumber' must be exactly 10-digits" };
        }

        if (string.IsNullOrEmpty(request.MessageTemplate))
        {
          return new SendOTPResult() { Error = "Parameter 'request.MessageTemplate'cannot be empty" };
        }

        if (string.IsNullOrEmpty(request.OtpTemplateId))
        {
          return new SendOTPResult() { Error = "Parameter 'request.OtpTemplateId' cannot be empty" };
        }

        if (!request.MessageTemplate.Contains(request.OtpTemplateId))
        {
          return new SendOTPResult() { Error = "Parameter 'request.OtpTemplateId' not found in request.MessageTemplate" };
        }
        #endregion
                
        log.Information("[SendOTPViaSMS]- Passed basic validation");

        #region A very simple OTP generation method
        string otp = null;
        using (var generator = new System.Security.Cryptography.RNGCryptoServiceProvider())
        {
          var buffer = new byte[260];
          generator.GetBytes(buffer);
          otp = string.Format("{0}{1}{2}{3}", buffer[buffer[256]] % 9, buffer[buffer[257]] % 9, buffer[buffer[258]] % 9, buffer[buffer[259]] % 9);
        }
        #endregion

        var message = request.MessageTemplate.Replace(request.OtpTemplateId, otp);
        if (message.Length > 160)
        {
          return new SendOTPResult() { Error = "Final message exceeds 160 characters" };
        }
        
        #region Send OTP
        log.Information("Sending OTP {OTP} to {Cell}", otp, request.CellularNumber);
        messageBus.SendSMS(request.CellularNumber, message);

        log.Information("Added to queue");
        #endregion

        // TODO: Get result of messagequeue 
        return new SendOTPResult() { OTP = otp, ResultId = 1 };
      }
      catch (Exception err)
      {
        log.Error(err, "SendOTPViaSMS");
        return new SendOTPResult() { Error = "Unexpected server error" };
      }
    }

  }
}
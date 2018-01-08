using System;

using Atlas.Integration.Interface;
using Atlas.Server.Implementation.Token;
using Atlas.Common.Interface;


namespace Atlas.Server.Implementation
{
  public class SendSMS_Impl
  {
    public static SendSMSResult SendSMS(ILogging log, Atlas.Server.Implementation.MessageBus.IMessageBusHandler messageBus, string loginToken, SendSMSRequest request)
    {
      try
      {
        log.Information("[SendOTPViaSMS]- {LoginToken}-{@request}", loginToken, request);

        #region Basic validation
        if (string.IsNullOrEmpty(loginToken))
        {
          return new SendSMSResult() { Error = "Token cannot be empty- login first!" };
        }
        string userId;
        string branch;
        if (!UserToken.TryGetUserInfo(loginToken, out userId, out branch))
        {
          return new SendSMSResult() { Error = "Login token invalid/has expired" };
        }

        if (request == null)
        {
          return new SendSMSResult() { Error = "Parameter 'request' cannot be empty" };
        }

        if (string.IsNullOrEmpty(request.CellularNumber) || request.CellularNumber.Length != 10)
        {
          return new SendSMSResult() { Error = "Parameter 'request.CellularNumber' must be exactly 10-digits" };
        }

        if (string.IsNullOrEmpty(request.Message))
        {
          return new SendSMSResult() { Error = "Parameter 'request.Message'cannot be empty" };
        }
        if (request.Message.Length > 160)
        {
          return new SendSMSResult() { Error = "Parameter 'request.Message' exceed 160 characters" };
        }
        #endregion

        messageBus.SendSMS(request.CellularNumber, request.Message);

        return new SendSMSResult() { ResultId = 1 };
      }
      catch (Exception err)
      {
        log.Error(err, "SendSMS");
        return new SendSMSResult() { Error = "Unexpected server error" };
      }
    }
  }
}
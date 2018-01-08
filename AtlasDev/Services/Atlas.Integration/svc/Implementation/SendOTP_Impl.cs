using Atlas.Integration.Interface;
using Atlas.Common.Interface;


namespace Atlas.Server.Implementation
{
  public class SendOTP_Impl
  {
    public static SendOTPResult SendOTP(ILogging log, Atlas.Server.Implementation.MessageBus.IMessageBusHandler messageBus,
      string loginToken, Atlas.Integration.Interface.SendOTPRequest request)
    {
      return SendOTPViaSMS_Impl.SendOTPViaSMS(log, messageBus, loginToken, request);
    }
  }
}
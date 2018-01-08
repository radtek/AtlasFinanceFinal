using System;
using System.Threading.Tasks;

using Atlas.Common.Interface;
using Atlas.NotificationENQ.Dto;


namespace NotificationServerENQ.Senders
{
  public interface ISmsSender
  {
    Task<Tuple<long, string>> Send(ILogging log, IConfigSettings config, SendSmsMessageRequest message);

  }

}

using System;

using Atlas.Common.Interface;
using System.Threading.Tasks;
using Atlas.NotificationENQ.Dto;


namespace NotificationServerENQ.Senders
{ 
  internal interface IEmailSender
  {
    Task<Tuple<bool, string>> Send(ILogging log, IConfigSettings config, SendEmailMessageRequest message);
  }

}
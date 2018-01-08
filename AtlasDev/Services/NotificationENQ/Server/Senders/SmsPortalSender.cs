using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.XPath;

using Atlas.NotificationENQ.Dto;
using Atlas.Common.Interface;


namespace NotificationServerENQ.Senders
{
  internal class SmsPortalSender : ISmsSender
  {
    /// <summary>
    /// Async SMS send via SMSPortal HTTP Post
    /// </summary>
    /// <param name="log"></param>
    /// <param name="config"></param>
    /// <param name="message">The SMS message</param>
    /// <returns>Tuple containing if send was successful (if true) and error message</returns>
    public async Task<Tuple<long, string>> Send(ILogging log, IConfigSettings config, SendSmsMessageRequest message)
    {
      var sendId = 0L;
      string error = null;
      try
      {
        var username = config.GetCustomSetting(null, "sms.smsportal.username", false) ?? "Atlas";   
        var password = config.GetCustomSetting(null, "sms.smsportal.password", false) ?? "atlas123"; 
        var attempts = 0;

        while (sendId <= 0 && attempts++ < 3)
        {
          log.Information("[SMS Sender] Sending: {@SMS}", message);
          using (var client = new HttpClient())
          using (HttpContent content = new StringContent(""))
          {
            client.Timeout = TimeSpan.FromSeconds(15);
            var result = await client.PostAsync(
              "http://www.mymobileapi.com/api5/http5.aspx?Type=sendparam" +
              $"&username={username}&password={password}&numto={message.To}&data1={Uri.EscapeDataString(message.Body)}", null).ConfigureAwait(false);

            var response = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            log.Information("[SMS Sender] SendSMS: Received: {Response}", response);
            if (!string.IsNullOrEmpty(response))
            {
              var doc = System.Xml.Linq.XDocument.Parse(response);
              long.TryParse(doc.XPathSelectElement("api_result/send_info/eventid")?.Value ?? "0", out sendId);
           
              //wasSent = doc.XPathSelectElement("api_result/call_result/result")?.Value.ToUpper().Trim() == "TRUE";
              error = doc.XPathSelectElement("api_result/send_info/error")?.Value;
            }
            else
            {
              error = "Empty response was received";
            }
          }
        }
        //  http://www.mymobileapi.com/api5/http5.aspx?Type=sendparam&username=Atlas&password=atlas123&numto=0837947058&data1=Test
      }
      catch (Exception err)
      {
        log.Error(err, "[SMS Sender] SmsPortalSender.Send()");
        error = err.Message;
      }

      return new Tuple<long, string>(sendId, error);
    }
  }
}

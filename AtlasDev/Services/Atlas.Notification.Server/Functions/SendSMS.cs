using System;
using System.Configuration;
using System.Xml;
using Atlas.Notification.Server.Structures;

namespace Atlas.Notification.Server.Functions
{
  public static class SendSMS
  {
    public static void Send(string to, string body)
    {
      var panacea = new Atlas.ThirdParty.PanaceaMobile.PanaceaApi(ConfigurationManager.AppSettings["panacea.user"], ConfigurationManager.AppSettings["panacea.password"]);
      var result = panacea.message_send(to, body);
    }

    public static long? SendPortalSms(string to, string body)
    {
      using (var client = new SMSPortal.APISoapClient())
      {
        var data = new senddata();
        var settings = new senddataSettings()
        {
          default_senderid = string.Empty,
          default_date = DateTime.Now.ToString("dd/MMM/yyyy"),
          default_time = DateTime.Now.AddMinutes(+5).ToString("hh:MM")
        };

        data.Items = new Object[2];

        var entry = new senddataEntries()
        {
          numto = to,
          customerid = Guid.NewGuid().ToString().Replace("-", ""),
          senderid = "Atlas",
          time = DateTime.Now.AddMinutes(+5).ToString("hh:MM"),
          data1 = body
        };

        data.Items[0] = settings;
        data.Items[1] = entry;
        var result = client.Send_STR_STR(ConfigurationManager.AppSettings["smsportal.user"], ConfigurationManager.AppSettings["smsportal.password"], Atlas.Common.Utils.Xml.Serialize<senddata>(data));
        var document = new XmlDocument();
        document.LoadXml(result);
        long eventId;
        Int64.TryParse(document.SelectSingleNode("/api_result/send_info/eventid").InnerText, out eventId);
        return eventId == 0 ? (long?)null : eventId;
      }    
    }
  }
}
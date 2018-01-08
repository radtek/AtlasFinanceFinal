using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using System.Net.Mail;
using Atlas.Common.Utils;
using Newtonsoft.Json;

namespace Atlas.Notification.Server.Functions
{
  public static class SendMail
  {
    public static void SendMailgun(string from, string to, string cc, string bcc, string subject, string body, bool isHTML, List<Tuple<string,string,string>> attachments)
    {
      var client = new RestClient();
      client.BaseUrl = new Uri(ConfigurationManager.AppSettings["server"]);
      client.Authenticator = new HttpBasicAuthenticator("api", ConfigurationManager.AppSettings["api.key"]);
      var request = new RestRequest();
      request.AddParameter("domain", "atlasonline.co.za", ParameterType.UrlSegment);
      request.Resource = "{domain}/messages";
      request.AddParameter("from", from);

      if (attachments != null)
        foreach (var attachment in attachments)
        {
          var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Attachments");
          if (!Directory.Exists(filePath))
            Directory.CreateDirectory(filePath);

          var fileName = attachment.Item1;
          var extension = attachment.Item2;
          var fileData = attachment.Item3;

          File.WriteAllBytes(string.Format("{0}/{1}{2}", filePath, fileName, extension), Atlas.Common.Utils.Base64.DecodeString(fileData));
          request.AddFile("attachment", string.Format("{0}/{1}{2}", filePath, fileName, extension));
        }

      #region To

      if (to.Contains(";"))
      {
        List<string> tos = Regex.Split(to, ";").ToList();
        foreach (var itm in tos)
        {
          request.AddParameter("to", itm);
        }
      }
      else
      {
        request.AddParameter("to", to);
      }

      #endregion

      //#region Cc

      //if (cc.Contains(";"))
      //{
      //  List<string> ccs = Regex.Split(cc, ";").ToList();
      //  foreach (var itm in ccs)
      //  {
      //    request.AddParameter("cc", itm);
      //  }
      //}
      //else
      //{
      //  request.AddParameter("cc", cc);
      //}

      //#endregion

      //#region Bcc

      //if (bcc.Contains(";"))
      //{
      //  List<string> bccs = Regex.Split(bcc, ";").ToList();
      //  foreach (var itm in bcc)
      //  {
      //    request.AddParameter("bcc", itm);
      //  }
      //}
      //else
      //{
      //  request.AddParameter("bcc", bcc);
      //}

      //#endregion

      request.AddParameter("subject", subject);
      request.AddParameter(isHTML ? "html" : "text", body);
      request.Method = Method.POST;

      var response = client.Execute(request);
      if (response.StatusCode != HttpStatusCode.OK)
      {
        throw new Exception($"{JsonConvert.SerializeObject(response)}");
      }
    }

      public static void SendEmailSmtp(string from, string to, string cc, string bcc, string subject, string body, bool isHTML, List<Tuple<string, string, string>> attachments)
      {
            using (var mailMessage = new MailMessage(new MailAddress(from), new MailAddress(to)))
            {
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = isHTML;
                mailMessage.Subject = subject;
                mailMessage.From = new MailAddress(from);
                mailMessage.To.Add(new MailAddress(to.Replace(";", ",").Replace(" ", "")));

                if (attachments?.Count > 0)
                {
                    foreach (var attachment in attachments)
                    {
                        mailMessage.Attachments.Add(
                            new Attachment(new MemoryStream(Base64.DecodeString(attachment.Item3)),
                                attachment.Item1+attachment.Item2, "application/octet-stream"));
                    }
                }

                using (var client = new SmtpClient())
                {
                    client.Timeout = 120000; // milliseconds
                    client.Host = "mail.atcorp.co.za";
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Send(mailMessage);
                }
            }
        }
  }
}

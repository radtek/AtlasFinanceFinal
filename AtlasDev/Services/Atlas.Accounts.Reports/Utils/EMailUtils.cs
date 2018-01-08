using System;
using System.Collections.Generic;
using System.Net.Mail;

using Atlas.Common.Interface;


namespace Atlas.Accounts.Reports.Utils
{
  public class EMailUtils
  {
    // TODO: Use notification server
    public static void SendEMail(ILogging log, string subject, string body, List<string> recipients, string attachmentFileName)
    {
      using (var message = new MailMessage())
      {
        message.Subject = subject;
        message.Body = body;
        message.IsBodyHtml = false;
        message.From = new MailAddress("it@atcorp.co.za");
        foreach (var recipient in recipients)
        {
          message.To.Add(new MailAddress(recipient));
        }

        using (var attachment = new Attachment(attachmentFileName))
        {
          message.Attachments.Add(attachment);

          using (var client = new SmtpClient())
          {
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Host = "mail.atcorp.co.za";
            client.UseDefaultCredentials = false;

            client.Timeout = 120000;

            var attempts = 0;
            var successful = false;
            while (attempts++ < 10 && !successful)
            {
              try
              {
                log.Information("Sending '{Subject}' to {@Address}", subject, message.To);
                client.Send(message);
                successful = true;
                log.Information("Successfully sent");
              }
              catch (Exception err)
              {
                log.Error(err, "Failed attempt {Attempt}", attempts);
              }
            }
          }
        }
      }
    }

  }
}

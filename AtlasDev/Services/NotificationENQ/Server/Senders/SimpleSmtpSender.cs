using System;
using System.Threading.Tasks;
using System.Net.Mail;
using System.IO;

using Atlas.NotificationENQ.Dto;
using Atlas.Common.Interface;


namespace NotificationServerENQ.Senders
{
  internal class SimpleSmtpSender : IEmailSender
  {
    public async Task<Tuple<bool, string>> Send(ILogging log, IConfigSettings config, SendEmailMessageRequest message)
    {
      var wasSent = false;
      string error = null;

      try
      {
        log.Information("[E-mail sender] Sending email: {@Details}", new { message.From, message.To, message.Subject });

        using (var mailMessage = new MailMessage(new MailAddress(message.From), new MailAddress(message.To)))
        {
          mailMessage.Body = message.Body;
          mailMessage.IsBodyHtml = message.BodyIsHtml;
          mailMessage.Subject = message.Subject;
          mailMessage.From = new MailAddress(message.From);
          mailMessage.To.Add(new MailAddress(message.To));

          if (message.Attachments?.Count > 0)
          {
            foreach(var attachment in message.Attachments)
            {
              mailMessage.Attachments.Add(new Attachment(new MemoryStream(attachment.Item2), attachment.Item1));
            }
          }

          using (var client = new SmtpClient())
          {
            client.Timeout = 120000; // milliseconds
            client.Host = config.GetSmtpServer();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            await client.SendMailAsync(mailMessage).ConfigureAwait(false);
          }
        }
        wasSent = true;
        log.Information("[E-mail sender] Message sent");
      }
      catch (Exception err)
      {
        log.Error(err, "[E-mail sender] SmtpSender.Send()");
        wasSent = false;
        error = err.Message;
      }

      return new Tuple<bool, string>(wasSent, error);
    }
  }
}

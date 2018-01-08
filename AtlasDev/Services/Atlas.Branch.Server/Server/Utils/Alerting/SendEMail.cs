using Serilog;
using System;


namespace ASSSyncClient.Utils.Alerting
{
  internal class SendEMail
  {
    internal static void SendError(ILogger log, string subject, string body, bool isBodyHtml)
    {
      var methodName = "SendEMail.SendError";
      try
      {
        using (var smtpClient = new System.Net.Mail.SmtpClient("mail.atcorp.co.za", 25) { UseDefaultCredentials = false, DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network, Timeout = 30000 })
        using (var message = new System.Net.Mail.MailMessage("it@atcorp.co.za", "it@atcorp.co.za") { Subject = subject, Body = body, IsBodyHtml = isBodyHtml, Priority = System.Net.Mail.MailPriority.High })
        {
          log.Information("{MethodName} Sending error e-mail message with {Subject}...", methodName, subject);
          smtpClient.Send(message);
          log.Information("{MethodName} Successfully sent error e-mail message", methodName);
        }
      }
      catch (Exception err)
      {
        log.Error(err, "{MethodName}", methodName);
      }
    }
  }
}

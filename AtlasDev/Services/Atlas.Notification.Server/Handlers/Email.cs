using System;
using System.Linq;
using Atlas.Common.Extensions;
using Atlas.Domain.Model;
using Atlas.Notification.Server.Cache;
using Atlas.Notification.Server.Functions;
using Atlas.RabbitMQ.Messages.Notification;
using DevExpress.Xpo;
using log4net;

namespace Atlas.Notification.Server.Handlers
{
  public static class EmailHandle
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(EmailHandle));

    public static void Send(EmailNotifyMessage messg)
    {
      _log.Info(string.Format("EmailNotifyMessage Received - To: {0}, From: {1}, Priority: {2}", messg.To, messg.From, messg.Priority.ToStringEnum()));

      using (var uow = new UnitOfWork())
      {
        var notification = new NTF_Notification(uow)
        {
          ActionDate = messg.ActionDate,
          Body = messg.Body,
          CreateDate = messg.CreatedAt,
          From = messg.From,
          IsHTML = messg.IsHTML,
            //Edited by Prashant
            //Priority = new XPQuery<NTF_Priority>(uow).FirstOrDefault(p => p.Type == messg.Priority),
            //Status = new XPQuery<NTF_Status>(uow).FirstOrDefault(p => p.Type == Enumerators.Notification.NotificationStatus.New),
            //Type = new XPQuery<NTF_Type>(uow).FirstOrDefault(p => p.Type == Enumerators.Notification.NotificationType.Email)
            Priority = new XPQuery<NTF_Priority>(uow).FirstOrDefault(p => p.PriorityId == (int)messg.Priority),
            Status = new XPQuery<NTF_Status>(uow).FirstOrDefault(p => p.StatusId == (int)Enumerators.Notification.NotificationStatus.New),
            Type = new XPQuery<NTF_Type>(uow).FirstOrDefault(p => p.TypeId  == (int)Enumerators.Notification.NotificationType.Email),
            Subject = messg.Subject,
          To = messg.To,            
        };
               
        uow.CommitChanges();

        _log.Info(string.Format("EmailNotifyMessage Saved - NotificationId {0}", notification.NotificationId));

        if (messg.Priority == Enumerators.Notification.NotificationPriority.High)
        {
          _log.Info(string.Format("EmailNotifyMessage Sending High Priority - NotificationId {0}", notification.NotificationId));

          try
          {
            if (messg.From.Contains("atlasonline.co.za"))
            {
              SendMail.SendMailgun(messg.From, messg.To, messg.Cc, messg.Bcc, messg.Subject, messg.Body, messg.IsHTML,
                messg.Attachments);
            }
            else
            {
              SendMail.SendEmailSmtp(messg.From, messg.To, messg.Cc, messg.Bcc, messg.Subject, messg.Body, messg.IsHTML,
                messg.Attachments);
            }
                        //Edited by Prashant
                        //notification.Status = new XPQuery<NTF_Status>(uow).FirstOrDefault(p => p.Type == Enumerators.Notification.NotificationStatus.Sent);
                        notification.Status = new XPQuery<NTF_Status>(uow).FirstOrDefault(p => p.StatusId == (int)Enumerators.Notification.NotificationStatus.Sent);
                        _log.Info(string.Format("EmailNotifyMessage Sent High Priority - NotificationId {0}", notification.NotificationId));
          }
          catch(Exception ex)
          {
            _log.Fatal(string.Format("EmailNotifyMessage Sending Failed High Priority - NotificationId {0} - Exception {1}", notification.NotificationId,ex.Message ));
                        //Edited by Prashant
                        //notification.Status = new XPQuery<NTF_Status>(uow).FirstOrDefault(p => p.Type == Enumerators.Notification.NotificationStatus.Failed);
                        notification.Status = new XPQuery<NTF_Status>(uow).FirstOrDefault(p => p.StatusId == (int)Enumerators.Notification.NotificationStatus.Failed);
                    }

          if (notification.Status.Type == Enumerators.Notification.NotificationStatus.Failed)
            _log.Info(string.Format("EmailNotifyMessage Sending Failed High Priority - NotificationId {0}", notification.NotificationId));

          uow.CommitChanges();
        }
        else
        {
          PendingCache.Push(notification.NotificationId, messg.Priority);
        }
      }
    }
  }
}
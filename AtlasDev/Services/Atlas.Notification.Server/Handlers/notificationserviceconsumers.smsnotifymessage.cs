using System;
using System.Linq;
using Atlas.Common.Extensions;
using Atlas.Domain.Model;
using Atlas.Notification.Server.Cache;
using Atlas.Notification.Server.EasyNetQ;
using Atlas.Notification.Server.Functions;
using Atlas.RabbitMQ.Messages.Notification;
using DevExpress.Xpo;
using log4net;


namespace Atlas.Notification.Server.Handlers
{
  public partial class NotificationServiceConsumers
  {
    private readonly ILog _log = LogManager.GetLogger(typeof(NotificationServiceConsumers));

    public void Consume(SMSNotifyMessage message)
    {
      _log.Info(string.Format("SMSNotifyMessage Received - To: {0}, Priority: {1}", message.To,
        message.Priority.ToStringEnum()));

      using (var uow = new UnitOfWork())
      {
        var notification = new NTF_Notification(uow)
        {
          ActionDate = message.ActionDate,
          Body = message.Body,
          CreateDate = message.CreatedAt,
            //Edited By Prashant
            //Priority = new XPQuery<NTF_Priority>(uow).FirstOrDefault(p => p.Type == message.Priority),
            //Status =
            //  new XPQuery<NTF_Status>(uow).FirstOrDefault(
            //    p => p.Type == Enumerators.Notification.NotificationStatus.New),
            //To = message.To,
            //Type =
            //  new XPQuery<NTF_Type>(uow).FirstOrDefault(
            //    p => p.Type == Enumerators.Notification.NotificationType.SMS)
            Priority = new XPQuery<NTF_Priority>(uow).FirstOrDefault(p => p.PriorityId == (int)message.Priority),
            Status =
            new XPQuery<NTF_Status>(uow).FirstOrDefault(
              p => p.StatusId == (int)Enumerators.Notification.NotificationStatus.New),
            To = message.To,
            Type =
            new XPQuery<NTF_Type>(uow).FirstOrDefault(
              p => p.TypeId == (int)Enumerators.Notification.NotificationType.SMS)
        };
        uow.CommitChanges();

        _log.Info(string.Format("SMSNotifyMessage Saved - NotificationId {0}", notification.NotificationId));

        if (message.Priority == Enumerators.Notification.NotificationPriority.High)
        {
          _log.Info(string.Format("SMSNotifyMessage Sending - NotificationId {0}",
            notification.NotificationId));

          try
          {
            if (message.Provider == Provider.EUROCOM || message.Provider == Provider.BLUELABEL)
              message.Provider = Provider.SMSPORTAL;

            switch (message.Provider)
            {
              case Provider.EUROCOM:
                ServiceLocator.Get<AtlasOnlineServiceBus>()
                  .GetServiceBus()
                  .Publish<EurocomSMSRequestMessage>(new EurocomSMSRequestMessage()
                  {
                    CampaignId = null,
                    CorrelationId = Guid.NewGuid(),
                    CellNo = message.To,
                    Message = message.Body,
                    NotificationId = notification.NotificationId
                  });
                break;

              case Provider.BLUELABEL:
                SendSMS.Send(message.To, message.Body);
                break;

              case Provider.SMSPORTAL:
                var eventId = SendSMS.SendPortalSms(message.To, message.Body);

                if (eventId != null)
                  notification.EventId = (long)eventId;

                break;
              default:
                SendSMS.SendPortalSms(message.To, message.Body);
                break;
            }

            notification.Status =
              new XPQuery<NTF_Status>(uow).FirstOrDefault(
                p => p.Type == Enumerators.Notification.NotificationStatus.Sent);
          }
          catch
          {
            _log.Fatal(string.Format("SMSNotifyMessage Sending Failed - NotificationId {0}",
              notification.NotificationId));
            notification.Status =
              new XPQuery<NTF_Status>(uow).FirstOrDefault(
                p => p.Type == Enumerators.Notification.NotificationStatus.Failed);
          }

          if (notification.Status.Type == Enumerators.Notification.NotificationStatus.Sent)
            _log.Info(string.Format("SMSNotifyMessage Sent - NotificationId {0}",
              notification.NotificationId));

          notification.Save();
          uow.CommitChanges();
        }
        else
        {
          PendingCache.Push(notification.NotificationId, message.Priority);
        }
      }
    }
  }
}
using System.Linq;
using Atlas.Common.Extensions;
using Atlas.Domain.Model;
using Atlas.RabbitMQ.Messages.Notification;
using DevExpress.Xpo;


namespace Atlas.Notification.Server.Handlers
{
  public   partial class NotificationServiceConsumers 
    {
        public  void Consume(SMSNotifyUpdateWithStatus message)
      {
              _log.Info(string.Format("SMSNotifyUpdateWithStatus Received - Notification Id: {0}, Status: {1}",
                  message.NotificationId, message.Status.ToStringEnum()));

              using (var uow = new UnitOfWork())
              {
                  var notification =
                      new XPQuery<NTF_Notification>(uow).FirstOrDefault(p => p.NotificationId == message.NotificationId);

                  if (notification != null)
                  {
                      notification.Status = new XPQuery<NTF_Status>(uow).FirstOrDefault(p => p.Type == message.Status);
                      uow.CommitChanges();
                  }

                  _log.Info(string.Format("SMSNotifyUpdateWithStatus Saved - Notification Id: {0}, Status: {1}",
                      message.NotificationId, message.Status.ToStringEnum()));
              }
      }
  }
}
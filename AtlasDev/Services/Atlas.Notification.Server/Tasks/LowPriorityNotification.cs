using System;
using System.Linq;
using Atlas.Domain.Model;
using Atlas.Notification.Server.Cache;
using Atlas.Notification.Server.Functions;
using DevExpress.Xpo;
using log4net;
using Quartz;

namespace Atlas.Notification.Server.Tasks
{
  public sealed class LowPriorityNotification : IJob
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(LowPriorityNotification));

    public void Execute(IJobExecutionContext context)
    {
      try
      {       

        var normalPriortyCollection = PendingCache.GetItems(Enumerators.Notification.NotificationPriority.Low).ToList();

        using (var uow = new UnitOfWork())
        {
          // Get only the items that are stored in the cache.
          var normalPriority = new XPQuery<NTF_Notification>(uow).Where(o => normalPriortyCollection.ToArray()
                                  .Contains(o.NotificationId) && o.Status.Type
                                              == Enumerators.Notification.NotificationStatus.New).ToList();

          if (normalPriority.Count > 0)
            _log.Info("LowPriorityNotification Task Started...");

          foreach (var item in normalPriority)
          {
            switch (item.Type.Type)
            {
              case Atlas.Enumerators.Notification.NotificationType.Email:
                SendMail.SendEmailSmtp(item.From, item.To,item.Cc, item.Bcc, item.Subject, item.Body, item.IsHTML, null);
                break;
              case Atlas.Enumerators.Notification.NotificationType.SMS:
                SendSMS.Send(item.To, item.Body);
                break;
              default:
                break;
            }
            // Remove item from cache.
            PendingCache.Pop(item.NotificationId);
          }
          if (normalPriority.Count > 0)
            _log.Info("LowPriorityNotification Task Ended");
        }        
      }
      catch (Exception err)
      {
        _log.Error("LowPriorityNotification - Execute", err);
      }
    }
  }
}
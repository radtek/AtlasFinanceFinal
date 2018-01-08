using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Domain.Model;
using Atlas.Notification.Server.Cache;
using Atlas.Notification.Server.Functions;
using DevExpress.Xpo;
using log4net;

namespace Atlas.Notification.Server.Tasks
{
  public static class CachePurgeNotification 
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(CachePurgeNotification));

    public static void Execute(List<long> cacheItems)
    {
      try
      {
        using (var uow = new UnitOfWork())
        {
          // Get only the items that are stored in the cache.
          var instantPriority = new XPQuery<NTF_Notification>(uow).Where(o => cacheItems.ToArray()
                                  .Contains(o.NotificationId) && o.Status.Type
                                              == Enumerators.Notification.NotificationStatus.New).ToList();

          if (instantPriority.Count == 0)
          {
            PendingCache.Clear();
            PendingCache.Write();
          }

          if (instantPriority.Count > 0)
            _log.Info("CachePurgeNotification Task Started...");

          foreach (var item in instantPriority)
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

            // Update item
            item.Status = new XPQuery<NTF_Status>(uow).FirstOrDefault(p => p.Type == Enumerators.Notification.NotificationStatus.Sent);
            item.StatusDate = DateTime.Now;
            item.Save();
            uow.CommitChanges();
          }

          if (instantPriority.Count > 0)
            _log.Info("CachePurgeNotification Task Ended");
        }
      }
      catch (Exception err)
      {
        _log.Error("CachePurgeNotification - Execute", err);
      }
    }
  }
}
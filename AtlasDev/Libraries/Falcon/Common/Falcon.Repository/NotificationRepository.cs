using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using Atlas.Domain.Model;
using Falcon.Common.Structures;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Common.Interfaces.Structures;


namespace Falcon.Common.Repository
{
  public sealed class NotificationRepository : INotificationRepository
  {
    /// <summary>
    /// Generic query function for notification table
    /// </summary>
    /// <param name="func">Predicate functor</param>
    /// <returns>List Of SmsNotification</returns>
    public List<ISmsNotification> GetSms(Func<NTF_Notification, bool> func)
    {
      var items = new List<ISmsNotification>();

      using (var uow = new UnitOfWork())
      {
        var queryAble = func != null ? new XPQuery<NTF_Notification>(uow).Where(func).ToList() : new XPQuery<NTF_Notification>(uow).ToList();

        items.AddRange(queryAble.Select(item => new SmsNotification
        {
          Body = item.Body,
          CreateDate = item.CreateDate,
          EventId = item.EventId,
          Reply = item.Reply,
          ReplyId = item.ReplyId,
          Status = item.Status.Description,
          To = item.To,
          NotificationId = item.NotificationId
        }));
      }

      return items;
    }
  }
}

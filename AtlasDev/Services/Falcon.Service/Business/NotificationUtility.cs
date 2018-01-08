using System;
using System.Collections.Generic;
using System.Linq;
using Falcon.Service.Core;
using Falcon.Common.Structures;
using Magnum;
using Newtonsoft.Json;

namespace Falcon.Service.Business
{
  public static class NotificationUtility
  {
    public static string RedisNotificationDataKey = "FalconNotificationData.id.{0}.user.id{1}";
    public static string RedisNotificationDataUserKey = "FalconNotificationDataUser.user.id.{0}";

    public static Notification AddNewNotification(string actionUrl, string title, string description, int priorityLevel, long userId)
    {
      var notificationData = new Notification()
      {
        ActionUrl = actionUrl,
        Title = title,
        Description = description,
        NotificationDate = DateTime.Now,
        NotificationId = CombGuid.Generate().ToString("D"),
        PriorityLevel = priorityLevel
      };

      var conn = RedisConnection.Current.GetDatabase();
      var notificationKey = string.Format(RedisNotificationDataKey, notificationData.NotificationId, userId);

      conn.StringSet(notificationKey, JsonConvert.SerializeObject(notificationData));

      conn.KeyExpire(notificationKey, new TimeSpan(24,0,0)); // 86400 = 24hours

      // Store notification id to user
      var userString = conn.StringGet(string.Format(RedisNotificationDataUserKey, userId));
      var userNotifications = new List<string>();
      if (!string.IsNullOrEmpty(userString))
        userNotifications = JsonConvert.DeserializeObject<List<string>>(userString);

      userNotifications.Add(notificationKey);

      conn.StringSet(string.Format(RedisNotificationDataUserKey, userId), JsonConvert.SerializeObject(userNotifications));

      return notificationData;
    }

    public static List<Notification> GetNotifications(long? branchId, long? userId)
    {
      var notifications = new List<Notification>();

      var conn = RedisConnection.Current.GetDatabase();
      var userString = conn.StringGet(string.Format(RedisNotificationDataUserKey, userId));
      var userNotifications = new List<string>();
      var toRemove = new List<string>();
      if (!string.IsNullOrEmpty(userString))
        userNotifications = JsonConvert.DeserializeObject<List<string>>(userString);

      foreach (var userNotificationKey in userNotifications)
      {
        var notificationString = conn.StringGet(userNotificationKey);
        if (string.IsNullOrEmpty(notificationString))
        {
          toRemove.Add(notificationString);
        }
        else
        {
          var notification = JsonConvert.DeserializeObject<Notification>(notificationString);
          if (notification != null)
            notifications.Add(notification);
        }
      }

      notifications = notifications.OrderByDescending(t => t.NotificationDate).ToList();

      userNotifications.RemoveAll(n => toRemove.Contains(n));

      conn.StringSet(string.Format(RedisNotificationDataUserKey, userId),
        JsonConvert.SerializeObject(userNotifications));

      return notifications;
    }

    public static Notification GetNotification(long userId, string notificationId)
    {
      var conn = RedisConnection.Current.GetDatabase();
      var notificationKey = string.Format(RedisNotificationDataKey, notificationId, userId);
      var notificationString = conn.StringGet(notificationKey);
      if (!string.IsNullOrEmpty(notificationString))
        return JsonConvert.DeserializeObject<Notification>(notificationString);
      else
        return null;
    }

    public static void MarkNotificationAsRead(long userId, string notificationId)
    {
      var conn = RedisConnection.Current.GetDatabase();
      var notificationKey = string.Format(RedisNotificationDataKey, notificationId, userId);

      conn.KeyDelete(notificationKey);

      var userString = conn.StringGet(string.Format(RedisNotificationDataUserKey, userId));
      var userNotifications = new List<string>();
      if (!string.IsNullOrEmpty(userString))
        userNotifications = JsonConvert.DeserializeObject<List<string>>(userString);
      userNotifications.RemoveAll(n => n == notificationKey);

      conn.StringSet(string.Format(RedisNotificationDataUserKey, userId), JsonConvert.SerializeObject(userNotifications));
    }
  }
}

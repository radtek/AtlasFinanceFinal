using System.Collections.Generic;
using System.Linq;
using Falcon.Service.Business;
using Falcon.Common.Structures;
using Serilog;
using XSockets.Core.Common.Socket.Event.Arguments;
using XSockets.Core.XSocket;
using XSockets.Core.XSocket.Helpers;

namespace Falcon.Service.Controllers
{
  public class NotificationController : XSocketController
  {
    private static readonly ILogger _log = Log.Logger.ForContext<Notification>();

    private long _userId { get; set; }
    public long UserId
    {
      get
      {
        return _userId;
      }
      set
      {
        SendNewNotification(value);
        _userId= value;
      }
    }

    public NotificationController()
    {
      OnOpen += Notification_OnOpen;
    }
    
    void Notification_OnOpen(object sender, OnClientConnectArgs e)
    {
       _log.Information("[FalconService][NotificationController] - Opened Connection");
    }

    public void Notify(int priorityLevel, string title, string description, string actionUrl, long? userId, long? branchId)
    {
      var users = UserUtility.GetUsers(branchId, userId).Select(t => t.PersonId).ToList();

      foreach (var user in users)
      {
        NotificationUtility.AddNewNotification(actionUrl, title, description, priorityLevel, user);
        SendNewNotification(user);
      }
    }

    private void SendNewNotification(long userId)
    {
      if (userId > 0)
      {
        var notification = new Notifications()
        {
          UserId = userId,
          NoteCount = 0,
          Notes = new List<Notification>()
        };

        var userNotifications = NotificationUtility.GetNotifications(null, userId);
        notification.Notes = userNotifications.Take(10).ToList();
        notification.NoteCount = userNotifications.Count;

        this.SendTo(n => userId == n._userId && n._userId != 0, notification, "new_notification");
      }
    }
  }
}
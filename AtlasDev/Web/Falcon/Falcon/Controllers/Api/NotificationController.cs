
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace Falcon.Controllers.Api
{
  [Authorize(Roles = "Admin")]
  public sealed class NotificationController : AppApiController
  {
    [HttpGet]
    //[WebApiAntiForgeryToken]
    public HttpResponseMessage GetNotifications(long branchId, long userId)
    {
      var notifications = Services.WebServiceClient.Notification_Gets(branchId, userId);

      return Request.CreateResponse(HttpStatusCode.OK, new { notifications });
    }


    [HttpGet]
    public HttpResponseMessage GetNotification(long userId, string detailId)
    {
      var notification = Services.WebServiceClient.Notification_Get(userId, detailId);

      return Request.CreateResponse(HttpStatusCode.OK, new { notification });
    }


    [HttpPost]
    public HttpResponseMessage MarkAsRead(long userId, string notificationId)
    {
      Services.WebServiceClient.Notification_MarkAsRead(userId, notificationId);

      return Request.CreateResponse(HttpStatusCode.OK, new { });
    }
  }
}

using System;
using System.Web.Mvc;
using Falcon.Base;
using Falcon.Common;

namespace Falcon.Areas.User.Controllers
{
  [Authorize]

  public class NotificationController : AppController
  {
    public ActionResult Index(long id = 0, long userId = 0)
    {
      var user = new UserCommon();
      ViewBag.PersonId = user.GetPersonId();
      ViewBag.BranchId = user.GetBranchId();
      return View();
    }

    public ActionResult Detail(long userId, Guid notificationId)
    {
      ViewBag.userId = userId;
      ViewBag.notificationId = notificationId;
      return View();
    }
  }
}
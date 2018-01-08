using System.Web.Mvc;

namespace Falcon.Areas.UserTracking
{
  public class UserTrackingAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get
      {
        return "UserTracking";
      }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {
      context.MapRoute(
          "UserTrackingManagement",
          "UserTracking/{controller}/{action}/{id}",
          new { action = "Index", id = UrlParameter.Optional }
      );
    }
  }
}
using System.Web.Mvc;

namespace Falcon.Areas.Stream
{
  public class StreamAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get { return "Stream"; }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {
      context.MapRoute(
        "Stream",
        "Stream/{controller}/{action}/{id}",
        new {action = "Index", id = UrlParameter.Optional}
        );
    }
  }
}
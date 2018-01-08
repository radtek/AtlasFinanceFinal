using System.Web.Mvc;

namespace Falcon.Areas.Target
{
  public class TargetAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get { return "Target"; }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {
      context.MapRoute(
        "Target",
        "Target/{controller}/{action}/{id}",
        new {action = "Index", id = UrlParameter.Optional}
        );
    }
  }
}
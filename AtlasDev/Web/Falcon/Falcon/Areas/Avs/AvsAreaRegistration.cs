using System.Web.Mvc;

namespace Falcon.Areas.Avs
{
  public class AvsAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get
      {
        return "Avs";
      }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {
      context.MapRoute(
          "AvsManagement",
          "Avs/{controller}/{action}/{id}",
          new { action = "Index", id = UrlParameter.Optional }
      );
    }
  }
}
using System.Web.Mvc;

namespace Falcon.Areas.Naedo
{
  public class NaedoManagementAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get
      {
        return "Naedo";
      }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {
      context.MapRoute(
          "Naedo",
          "Naedo/{controller}/{action}/{id}",
          new { action = "Index", id = UrlParameter.Optional }
      );
    }
  }
}
using System.Web.Mvc;

namespace Falcon.Areas.Leads
{
  public class LeadsAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get
      {
        return "Leads";
      }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {
      context.MapRoute(
          "Leads",
          "Leads/{controller}/{action}/{id}",
          new { action = "Index", id = UrlParameter.Optional }
      );
    }
  }
}
using System.Web.Mvc;

namespace Falcon.Areas.Batch
{
  public class BatchManagementAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get
      {
        return "Batch";
      }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {
      context.MapRoute(
          "Batch",
          "Batch/{controller}/{action}/{id}",
          new { action = "Index", id = UrlParameter.Optional }
      );
    }
  }
}
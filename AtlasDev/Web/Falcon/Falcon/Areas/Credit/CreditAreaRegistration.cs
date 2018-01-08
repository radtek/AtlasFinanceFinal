using System.Web.Mvc;

namespace Falcon.Areas.Credit
{
  public class CreditManagementAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get
      {
        return "Credit";
      }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {
      context.MapRoute(
          "Credit",
          "Credit/{controller}/{action}/{id}",
          new { action = "Index", id = UrlParameter.Optional }
      );
    }
  }
}
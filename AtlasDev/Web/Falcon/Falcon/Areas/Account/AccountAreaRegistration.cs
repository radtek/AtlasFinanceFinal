using System.Web.Mvc;

namespace Falcon.Areas.Account
{
  public class AccountAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get
      {
        return "Account";
      }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {
      context.MapRoute(
          "Account",
          "Account/{controller}/{action}/{id}",
          new { action = "Index", id = UrlParameter.Optional }
      );
    }
  }
}
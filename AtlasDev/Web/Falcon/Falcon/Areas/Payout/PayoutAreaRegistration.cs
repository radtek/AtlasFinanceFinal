using System.Web.Mvc;

namespace Falcon.Areas.Payout
{
  public class PayoutAreaRegistration:AreaRegistration
  {
    public override string AreaName
    {
      get
      {
        return "Payout";
      }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {
      context.MapRoute(
          "Payout",
          "Payout/{controller}/{action}/{id}",
          new { action = "Index", id = UrlParameter.Optional }
      );
    }
  }
}
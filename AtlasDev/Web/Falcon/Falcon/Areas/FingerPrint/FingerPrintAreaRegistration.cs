using System.Web.Mvc;

namespace Falcon.Areas.FingerPrint
{
  public class FingerPrintAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get
      {
        return "FingerPrint";
      }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {
      context.MapRoute(
          "FingerPrint",
          "FingerPrint/{controller}/{action}/{id}",
          new { action = "Index", id = UrlParameter.Optional }
      );
    }
  }
}